// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./VoterRegistry.sol";

contract Voting {
    // ─── Types ───────────────────────────────────────────────
    struct Candidate {
        string name;
        uint256 voteCount;
    }

    // ─── State ───────────────────────────────────────────────
    address public owner;
    bool public votingOpen;
    address public zkVerifier;

    VoterRegistry public registry;

    Candidate[] public candidates;
    mapping(address => bool) public hasVoted;

    // EIP-712 Meta-Transactions
    bytes32 public DOMAIN_SEPARATOR;
    bytes32 public constant VOTE_TYPEHASH = keccak256("Vote(address voter,uint256 candidateId,uint256 nonce)");
    mapping(address => uint256) public nonces;

    // ─── Events ──────────────────────────────────────────────
    event CandidateAdded(uint256 indexed id, string name);
    event Voted(bytes32 indexed anonymousHash, uint256 indexed candidateId);
    event VotingStarted();
    event VotingEnded();
    event OwnershipTransferred(
        address indexed previousOwner,
        address indexed newOwner
    );

    // ─── Modifiers ───────────────────────────────────────────
    modifier onlyOwner() {
        require(msg.sender == owner, "Not the owner");
        _;
    }

    modifier whenOpen() {
        require(votingOpen, "Voting is not open");
        _;
    }

    // ─── Constructor ─────────────────────────────────────────
    constructor(address _registry, address _zkVerifier) {
        owner = msg.sender;
        registry = VoterRegistry(_registry);
        zkVerifier = _zkVerifier;
        DOMAIN_SEPARATOR = keccak256(
            abi.encode(
                keccak256("EIP712Domain(string name,string version,uint256 chainId,address verifyingContract)"),
                keccak256(bytes("Decentralized Voting System")),
                keccak256(bytes("1.0.0")),
                block.chainid,
                address(this)
            )
        );
    }

    // ─── Ownership ───────────────────────────────────────────
    function transferOwnership(address _newOwner) external onlyOwner {
        require(_newOwner != address(0), "Zero address");
        emit OwnershipTransferred(owner, _newOwner);
        owner = _newOwner;
    }

    // ─── Owner functions ─────────────────────────────────────
    function addCandidate(string calldata _name) external onlyOwner {
        require(!votingOpen, "Cannot add candidates while voting is open");
        candidates.push(Candidate({name: _name, voteCount: 0}));
        emit CandidateAdded(candidates.length - 1, _name);
    }

    function startVoting() external onlyOwner {
        require(candidates.length > 0, "No candidates added");
        require(!votingOpen, "Voting already open");
        votingOpen = true;
        emit VotingStarted();
    }

    function endVoting() external onlyOwner whenOpen {
        votingOpen = false;
        emit VotingEnded();
    }

    // ─── Standard vote ───────────────────────────────────────
    function vote(
        address voter,
        uint256 _candidateId
    ) external whenOpen {
        _executeVote(voter, _candidateId);
    }

    // ─── Gasless vote via EIP-712 Signature ───────────────────
    function voteWithSignature(
        address voter,
        uint256 _candidateId,
        uint256 nonce,
        bytes calldata signature
    ) external whenOpen {
        require(nonce == nonces[voter], "Invalid nonce");

        bytes32 structHash = keccak256(
            abi.encode(VOTE_TYPEHASH, voter, _candidateId, nonce)
        );

        bytes32 digest = keccak256(
            abi.encodePacked("\x19\x01", DOMAIN_SEPARATOR, structHash)
        );

        address signer = recoverSigner(digest, signature);
        require(signer == voter, "Invalid signature");

        nonces[voter]++;
        _executeVote(voter, _candidateId);
    }

    function _executeVote(
        address voter,
        uint256 _candidateId
    ) internal {
        require(
            registry.isEligible(address(this), voter),
            "Not eligible to vote"
        );

        require(!hasVoted[voter], "Already voted");

        uint256 len = candidates.length;
        require(_candidateId < len, "Invalid candidate");

        hasVoted[voter] = true;

        candidates[_candidateId].voteCount += 1;

        emit Voted(keccak256(abi.encodePacked(voter, address(this))), _candidateId);
    }

    function recoverSigner(
        bytes32 digest,
        bytes memory signature
    ) public pure returns (address) {
        if (signature.length != 65) {
            return address(0);
        }
        bytes32 r;
        bytes32 s;
        uint8 v;
        assembly {
            r := mload(add(signature, 32))
            s := mload(add(signature, 64))
            v := byte(0, mload(add(signature, 96)))
        }
        return ecrecover(digest, v, r, s);
    }

    // ─── View functions ──────────────────────────────────────
    function getCandidateCount() external view returns (uint256) {
        return candidates.length;
    }

    function getCandidate(
        uint256 _id
    ) external view returns (string memory name, uint256 voteCount) {
        uint256 len = candidates.length;
        require(_id < len, "Invalid candidate");
        Candidate storage c = candidates[_id];
        return (c.name, c.voteCount);
    }

    function getWinner()
        external
        view
        returns (
            uint256[] memory winnerIds,
            string[] memory winnerNames,
            uint256 topVotes
        )
    {
        uint256 len = candidates.length;
        require(len > 0, "No candidates");

        uint256 count;
        for (uint256 i = 0; i < len;) {
            uint256 v = candidates[i].voteCount;
            if (v > topVotes) {
                topVotes = v;
                count = 1;
            } else if (v == topVotes) {
                count++;
            }
            unchecked { i++; }
        }

        winnerIds = new uint256[](count);
        winnerNames = new string[](count);
        uint256 idx;
        for (uint256 i = 0; i < len;) {
            if (candidates[i].voteCount == topVotes) {
                winnerIds[idx] = i;
                winnerNames[idx] = candidates[i].name;
                idx++;
            }
            unchecked { i++; }
        }
    }
}
