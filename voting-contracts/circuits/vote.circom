pragma circom 2.0.0;

include "../node_modules/circomlib/circuits/poseidon.circom";

// Verifies that merkleRoot is the root of a Merkle tree of depth N containing leaf at pathElements/pathIndices
template MerkleTreeChecker(levels) {
    signal input leaf;
    signal input pathElements[levels];
    signal input pathIndices[levels];
    signal output root;

    component hashes[levels];

    signal levelHashes[levels + 1];
    levelHashes[0] <== leaf;

    for (var i = 0; i < levels; i++) {
        hashes[i] = Poseidon(2);
        
        // pathIndices[i] is 0 if levelHashes[i] is left child, 1 if it is right child
        hashes[i].inputs[0] <== levelHashes[i] + pathIndices[i] * (pathElements[i] - levelHashes[i]);
        hashes[i].inputs[1] <== pathElements[i] + pathIndices[i] * (levelHashes[i] - pathElements[i]);

        levelHashes[i + 1] <== hashes[i].out;
    }

    root <== levelHashes[levels];
}

template VoteVerifier() {
    // Public inputs (signals matching ZKVerifier.sol PublicSignals struct)
    signal input merkleRoot;
    signal input nullifierHash;
    signal input ballotId;
    signal input voteCommitment;

    // Private inputs
    signal input secret;
    signal input salt;
    signal input pathElements[3];
    signal input pathIndices[3];
    signal input candidateId;

    // 1. Verify identity commitment computation: leaf = Poseidon(secret, salt)
    component leafHasher = Poseidon(2);
    leafHasher.inputs[0] <== secret;
    leafHasher.inputs[1] <== salt;
    
    signal identityCommitment;
    identityCommitment <== leafHasher.out;

    // 2. Verify membership in Merkle tree (depth 3, max 8 voters)
    component treeChecker = MerkleTreeChecker(3);
    treeChecker.leaf <== identityCommitment;
    for (var i = 0; i < 3; i++) {
        treeChecker.pathElements[i] <== pathElements[i];
        treeChecker.pathIndices[i] <== pathIndices[i];
    }
    
    // Constrain computed root to equal the public merkleRoot
    treeChecker.root === merkleRoot;

    // 3. Verify nullifier hash computation: nullifierHash = Poseidon(secret, ballotId)
    component nullHasher = Poseidon(2);
    nullHasher.inputs[0] <== secret;
    nullHasher.inputs[1] <== ballotId;
    nullHasher.out === nullifierHash;

    // 4. Verify vote commitment computation: voteCommitment = Poseidon(candidateId, secret)
    component voteHasher = Poseidon(2);
    voteHasher.inputs[0] <== candidateId;
    voteHasher.inputs[1] <== secret;
    voteHasher.out === voteCommitment;
}

// Declare main component with public inputs
component main {public [merkleRoot, nullifierHash, ballotId, voteCommitment]} = VoteVerifier();
