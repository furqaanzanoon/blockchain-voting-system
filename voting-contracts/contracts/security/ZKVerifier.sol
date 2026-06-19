// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract ZKVerifier {

    // ─── BN254 constant ──────────────────────────────────────
    uint256 constant PRIME_Q =
        21888242871839275222246405745257275088696311157297823662689037894645226208583;

    // ─── Types ───────────────────────────────────────────────
    struct G1Point {
        uint256 x;
        uint256 y;
    }

    struct G2Point {
        uint256[2] x;
        uint256[2] y;
    }

    struct Proof {
        G1Point a;
        G2Point b;
        G1Point c;
    }

    struct PublicSignals {
        uint256 merkleRoot;
        uint256 nullifierHash;
        uint256 ballotId;
        uint256 voteCommitment;
    }

    struct VerifyingKey {
        G1Point   alpha1;
        G2Point   beta2;
        G2Point   gamma2;
        G2Point   delta2;
        G1Point[] ic;
    }

    // ─── State ───────────────────────────────────────────────
    address public immutable owner;
    bool    public vkSet;

    VerifyingKey private vk;

    mapping(uint256 => uint256) public ballotRoots;
    mapping(uint256 => uint256) public nullifierUsed; // stores ballotId + 1
    mapping(uint256 => uint256[]) public voteCommitments;

    // ─── Events ──────────────────────────────────────────────
    event VerifyingKeySet(address indexed by);
    event BallotRootSet(uint256 indexed ballotId, uint256 merkleRoot);
    event VoteVerified(
        uint256 indexed ballotId,
        uint256 indexed nullifierHash,
        uint256         voteCommitment
    );
    event NullifierSpent(uint256 indexed nullifierHash, uint256 indexed ballotId);

    // ─── Modifiers ───────────────────────────────────────────
    modifier onlyOwner() {
        require(msg.sender == owner, "Not owner");
        _;
    }

    // ─── Constructor ─────────────────────────────────────────
    constructor() {
        owner = msg.sender;
        vkSet = false; // Key must be set explicitly via setVerifyingKey() before any verification
    }

    // ─── Setup ───────────────────────────────────────────────
    function setVerifyingKey(
        uint256[2]    calldata _alpha1,
        uint256[2][2] calldata _beta2,
        uint256[2][2] calldata _gamma2,
        uint256[2][2] calldata _delta2,
        uint256[2][]  calldata _ic
    ) external onlyOwner {
        require(!vkSet, "VK already set");
        require(_ic.length >= 2, "IC must have >= 2 points");

        vk.alpha1 = G1Point(_alpha1[0], _alpha1[1]);
        vk.beta2  = _toG2(_beta2);
        vk.gamma2 = _toG2(_gamma2);
        vk.delta2 = _toG2(_delta2);

        delete vk.ic;
        uint256 len = _ic.length;
        for (uint256 i = 0; i < len;) {
            vk.ic.push(G1Point(_ic[i][0], _ic[i][1]));
            unchecked { i++; }
        }

        vkSet = true;
        emit VerifyingKeySet(msg.sender);
    }

    function setBallotRoot(uint256 _ballotId, uint256 _merkleRoot)
        external onlyOwner
    {
        require(_merkleRoot != 0, "Empty root");
        ballotRoots[_ballotId] = _merkleRoot;
        emit BallotRootSet(_ballotId, _merkleRoot);
    }

    // ─── Core verify + record ────────────────────────────────
    function verifyAndVote(
        Proof         calldata proof,
        PublicSignals calldata signals
    ) external {
        require(vkSet, "Verifying key not set");
        require(
            ballotRoots[signals.ballotId] == signals.merkleRoot,
            "Unknown Merkle root"
        );
        require(nullifierUsed[signals.nullifierHash] == 0, "Nullifier already spent");
        require(signals.voteCommitment != 0, "Empty vote commitment");

        require(_verify(proof, signals), "Invalid zk proof");

        nullifierUsed[signals.nullifierHash] = signals.ballotId + 1;
        voteCommitments[signals.ballotId].push(signals.voteCommitment);

        emit NullifierSpent(signals.nullifierHash, signals.ballotId);
        emit VoteVerified(
            signals.ballotId,
            signals.nullifierHash,
            signals.voteCommitment
        );
    }

    // ─── View helpers ─────────────────────────────────────────
    function getVoteCount(uint256 _ballotId)
        external view returns (uint256)
    {
        return voteCommitments[_ballotId].length;
    }

    // getVoteCommitments remains same
    function getVoteCommitments(uint256 _ballotId)
        external view returns (uint256[] memory)
    {
        return voteCommitments[_ballotId];
    }

    function isNullifierSpent(uint256 _nullifier)
        external view returns (bool)
    {
        return nullifierUsed[_nullifier] != 0;
    }

    // ─── Groth16 internals ───────────────────────────────────
    function _verify(
        Proof         memory proof,
        PublicSignals memory signals
    ) internal view returns (bool) {
        require(vk.ic.length >= 2, "VK not initialized");

        // Compute the linear combination: vk_x = vk.ic[0] + sum(signals[i] * vk.ic[i+1])
        G1Point memory vk_x = vk.ic[0];
        uint256[4] memory sigArr = [
            signals.merkleRoot,
            signals.nullifierHash,
            signals.ballotId,
            signals.voteCommitment
        ];
        for (uint256 i = 0; i < sigArr.length;) {
            require(sigArr[i] < PRIME_Q, "Signal out of range");
            vk_x = _add(vk_x, _scalar_mul(vk.ic[i + 1], sigArr[i]));
            unchecked { i++; }
        }

        // Groth16 pairing check:
        // e(proof.A, proof.B) * e(-vk.alpha1, vk.beta2) * e(-vk_x, vk.gamma2) * e(-proof.C, vk.delta2) == 1
        return _pairingCheck(
            _negate(proof.a), proof.b,
            _negate(vk.alpha1), vk.beta2,
            _negate(vk_x), vk.gamma2,
            _negate(proof.c), vk.delta2
        );
    }

    function _add(G1Point memory p1, G1Point memory p2)
        internal view returns (G1Point memory r)
    {
        uint256[4] memory input = [p1.x, p1.y, p2.x, p2.y];
        uint256[2] memory output;
        bool success;
        assembly {
            success := staticcall(gas(), 6, input, 0x80, output, 0x40)
        }
        require(success, "BN254: add failed");
        r = G1Point(output[0], output[1]);
    }

    function _scalar_mul(G1Point memory p, uint256 s)
        internal view returns (G1Point memory r)
    {
        uint256[3] memory input = [p.x, p.y, s];
        uint256[2] memory output;
        bool success;
        assembly {
            success := staticcall(gas(), 7, input, 0x60, output, 0x40)
        }
        require(success, "BN254: scalar_mul failed");
        r = G1Point(output[0], output[1]);
    }

    function _pairingCheck(
        G1Point memory a1, G2Point memory a2,
        G1Point memory b1, G2Point memory b2,
        G1Point memory c1, G2Point memory c2,
        G1Point memory d1, G2Point memory d2
    ) internal view returns (bool) {
        uint256[24] memory input;

        input[0]  = a1.x;    input[1]  = a1.y;
        input[2]  = a2.x[0]; input[3]  = a2.x[1];
        input[4]  = a2.y[0]; input[5]  = a2.y[1];

        input[6]  = b1.x;    input[7]  = b1.y;
        input[8]  = b2.x[0]; input[9]  = b2.x[1];
        input[10] = b2.y[0]; input[11] = b2.y[1];

        input[12] = c1.x;    input[13] = c1.y;
        input[14] = c2.x[0]; input[15] = c2.x[1];
        input[16] = c2.y[0]; input[17] = c2.y[1];

        input[18] = d1.x;    input[19] = d1.y;
        input[20] = d2.x[0]; input[21] = d2.x[1];
        input[22] = d2.y[0]; input[23] = d2.y[1];

        uint256[1] memory out;
        bool success;
        assembly {
            success := staticcall(gas(), 8, input, 0x300, out, 0x20)
        }
        require(success, "BN254: pairing failed");
        return out[0] == 1;
    }

    function _negate(G1Point memory p)
        internal pure returns (G1Point memory)
    {
        if (p.x == 0 && p.y == 0) return G1Point(0, 0);
        return G1Point(p.x, PRIME_Q - (p.y % PRIME_Q));
    }

    function _toG2(uint256[2][2] calldata v)
        internal pure returns (G2Point memory)
    {
        return G2Point([v[0][0], v[0][1]], [v[1][0], v[1][1]]);
    }
}