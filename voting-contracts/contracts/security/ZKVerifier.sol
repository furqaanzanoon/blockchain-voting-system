// SPDX-License-Identifier: MIT
pragma solidity >=0.7.0 <0.9.0;

contract ZKVerifier {
    // Scalar field size
    uint256 constant r    = 21888242871839275222246405745257275088548364400416034343698204186575808495617;
    // Base field size
    uint256 constant q   = 21888242871839275222246405745257275088696311157297823662689037894645226208583;

    // Verification Key data (hardcoded from SnarkJS setup)
    uint256 constant alphax  = 9479011908317757449771268006727657252186364675778279022593253493118893596180;
    uint256 constant alphay  = 11161606022540599640371990300864124926716916211638161094369293260105077482200;
    uint256 constant betax1  = 9368920958051573222251593875096882081858877172747427365610547171169934624193;
    uint256 constant betax2  = 3146252841964947059705273269186567899413608578747173667407457286639311175056;
    uint256 constant betay1  = 8898996437361148566971930876873809064812212093110076877258541228884903190128;
    uint256 constant betay2  = 12650069053675464224186500897982421680506276668797864176219102514711546938700;
    uint256 constant gammax1 = 11559732032986387107991004021392285783925812861821192530917403151452391805634;
    uint256 constant gammax2 = 10857046999023057135944570762232829481370756359578518086990519993285655852781;
    uint256 constant gammay1 = 4082367875863433681332203403145435568316851327593401208105741076214120093531;
    uint256 constant gammay2 = 8495653923123431417604973247489272438418190587263600148770280649306958101930;
    uint256 constant deltax1 = 15790613952356016714154134991848863119324750852367151287803783607128307802547;
    uint256 constant deltax2 = 2553601380392842272426565889876790628191924681965954972894705887668803488145;
    uint256 constant deltay1 = 7449404803463547266238375401373375667979039271531598323290718264745137743736;
    uint256 constant deltay2 = 19123978109244789422213318599742099156836978416590011861945273247863988255881;

    uint256 constant IC0x = 9290996658723626895141007438834636230886871156019656155218800505647732014871;
    uint256 constant IC0y = 16032053147168857171520608271403508651743743582725995358761818832396072296676;
    
    uint256 constant IC1x = 3713115868481445977792481257511704094394160903985302264757397451396333719340;
    uint256 constant IC1y = 15900157898686486697333909610879463506550569486306184980412229938987529990044;
    
    uint256 constant IC2x = 4652462146242929551040576409073879208317473946510218247114382682675421286962;
    uint256 constant IC2y = 15801777923389012583123793364072305141871378869560814055038778723987044454548;
    
    uint256 constant IC3x = 5384251055432339794976961964469979205869280740963812628588152430563110353495;
    uint256 constant IC3y = 10090896530831456808411562356639808211857912496707210556760103650719485529106;
    
    uint256 constant IC4x = 3754490212941429326157811104986390657956781864038406518177257529659653652374;
    uint256 constant IC4y = 20892663830915566046517361586830209003108159259422614509534494654019909668107;

    // Memory data
    uint16 constant pVk = 0;
    uint16 constant pPairing = 128;
    uint16 constant pLastMem = 896;

    // ─── Structs matching generated API definitions ──────────
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

    // ─── State ───────────────────────────────────────────────
    address public immutable owner;
    mapping(uint256 => uint256) public ballotRoots;
    mapping(uint256 => uint256) public nullifierUsed; // stores ballotId + 1
    mapping(uint256 => uint256[]) public voteCommitments;

    // ─── Events ──────────────────────────────────────────────
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
        Proof calldata proof,
        PublicSignals calldata signals
    ) external {
        // Instantiate ballotRoots[signals.ballotId] if it's currently 0
        if (ballotRoots[signals.ballotId] == 0) {
            ballotRoots[signals.ballotId] = signals.merkleRoot;
            emit BallotRootSet(signals.ballotId, signals.merkleRoot);
        }

        require(
            ballotRoots[signals.ballotId] == signals.merkleRoot,
            "Unknown Merkle root"
        );
        require(nullifierUsed[signals.nullifierHash] == 0, "Nullifier already spent");
        require(signals.voteCommitment != 0, "Empty vote commitment");

        // Convert structures to flat arrays for verifyProof
        uint[2] memory pA = [proof.a.x, proof.a.y];
        uint[2][2] memory pB = [
            [proof.b.x[0], proof.b.x[1]],
            [proof.b.y[0], proof.b.y[1]]
        ];
        uint[2] memory pC = [proof.c.x, proof.c.y];
        uint[4] memory pubSignals = [
            signals.merkleRoot,
            signals.nullifierHash,
            signals.ballotId,
            signals.voteCommitment
        ];

        require(verifyProof(pA, pB, pC, pubSignals), "Invalid zk proof");

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

    // ─── Groth16 verifyProof from SnarkJS (using memory load) ────────────────────
    function verifyProof(uint[2] memory _pA, uint[2][2] memory _pB, uint[2] memory _pC, uint[4] memory _pubSignals) public view returns (bool isValidProof) {
        assembly {
            function checkField(v) {
                if iszero(lt(v, r)) {
                    mstore(0, 0)
                    return(0, 0x20)
                }
            }
            
            // G1 function to multiply a G1 value(x,y) to value in an address
            function g1_mulAccC(pR, x, y, s) {
                let success
                let mIn := mload(0x40)
                mstore(mIn, x)
                mstore(add(mIn, 32), y)
                mstore(add(mIn, 64), s)

                success := staticcall(sub(gas(), 2000), 7, mIn, 96, mIn, 64)

                if iszero(success) {
                    mstore(0, 0)
                    return(0, 0x20)
                }

                mstore(add(mIn, 64), mload(pR))
                mstore(add(mIn, 96), mload(add(pR, 32)))

                success := staticcall(sub(gas(), 2000), 6, mIn, 128, pR, 64)

                if iszero(success) {
                    mstore(0, 0)
                    return(0, 0x20)
                }
            }

            function checkPairing(pA, pB, pC, pubSignals, pMem) -> isOk {
                let _pPairing := add(pMem, pPairing)
                let _pVk := add(pMem, pVk)

                mstore(_pVk, IC0x)
                mstore(add(_pVk, 32), IC0y)

                // Compute the linear combination vk_x
                
                g1_mulAccC(_pVk, IC1x, IC1y, mload(add(pubSignals, 0)))
                
                g1_mulAccC(_pVk, IC2x, IC2y, mload(add(pubSignals, 32)))
                
                g1_mulAccC(_pVk, IC3x, IC3y, mload(add(pubSignals, 64)))
                
                g1_mulAccC(_pVk, IC4x, IC4y, mload(add(pubSignals, 96)))
                

                // -A
                mstore(_pPairing, mload(pA))
                mstore(add(_pPairing, 32), mod(sub(q, mload(add(pA, 32))), q))

                // B
                mstore(add(_pPairing, 64), mload(pB))
                mstore(add(_pPairing, 96), mload(add(pB, 32)))
                mstore(add(_pPairing, 128), mload(add(pB, 64)))
                mstore(add(_pPairing, 160), mload(add(pB, 96)))

                // alpha1
                mstore(add(_pPairing, 192), alphax)
                mstore(add(_pPairing, 224), alphay)

                // beta2
                mstore(add(_pPairing, 256), betax1)
                mstore(add(_pPairing, 288), betax2)
                mstore(add(_pPairing, 320), betay1)
                mstore(add(_pPairing, 352), betay2)

                // vk_x
                mstore(add(_pPairing, 384), mload(add(pMem, pVk)))
                mstore(add(_pPairing, 416), mload(add(pMem, add(pVk, 32))))


                // gamma2
                mstore(add(_pPairing, 448), gammax1)
                mstore(add(_pPairing, 480), gammax2)
                mstore(add(_pPairing, 512), gammay1)
                mstore(add(_pPairing, 544), gammay2)

                // C
                mstore(add(_pPairing, 576), mload(pC))
                mstore(add(_pPairing, 608), mload(add(pC, 32)))

                // delta2
                mstore(add(_pPairing, 640), deltax1)
                mstore(add(_pPairing, 672), deltax2)
                mstore(add(_pPairing, 704), deltay1)
                mstore(add(_pPairing, 736), deltay2)


                let success := staticcall(sub(gas(), 2000), 8, _pPairing, 768, _pPairing, 0x20)

                isOk := and(success, mload(_pPairing))
            }

            let pMem := mload(0x40)
            mstore(0x40, add(pMem, pLastMem))

            // Validate that all evaluations ∈ F
            
            checkField(mload(add(_pubSignals, 0)))
            
            checkField(mload(add(_pubSignals, 32)))
            
            checkField(mload(add(_pubSignals, 64)))
            
            checkField(mload(add(_pubSignals, 96)))
            

            // Validate all evaluations
            let isValid := checkPairing(_pA, _pB, _pC, _pubSignals, pMem)

            isValidProof := isValid
        }
    }
}
