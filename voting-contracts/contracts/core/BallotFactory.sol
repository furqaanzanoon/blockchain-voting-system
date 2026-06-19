// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./Voting.sol";

contract BallotFactory {
    struct BallotInfo {
        uint256 ballotId;
        address contractAddress;
        address owner;
        string title;
    }

    mapping(uint256 => BallotInfo) private ballots;

    function getBallotInfo(
        uint256 _ballotId
    ) external view returns (BallotInfo memory) {
        return ballots[_ballotId];
    }

    uint256 public ballotCount;

    address public immutable registryAddress;
    address public immutable zkVerifierAddress;

    event BallotCreated(
        uint256 indexed ballotId,
        address indexed contractAddress,
        address indexed owner,
        string title
    );

    constructor(address _registryAddress, address _zkVerifierAddress) {
        registryAddress = _registryAddress;
        zkVerifierAddress = _zkVerifierAddress;
    }

    function createBallot(
        string memory _title,
        string[] memory _candidateNames
    ) external returns (uint256) {
        require(_candidateNames.length >= 2, "At least 2 candidates required");

        Voting newVoting = new Voting(registryAddress, zkVerifierAddress);

        uint256 len = _candidateNames.length;
        for (uint256 i = 0; i < len;) {
            newVoting.addCandidate(_candidateNames[i]);
            unchecked { i++; }
        }

        newVoting.transferOwnership(msg.sender);

        uint256 currentId = ballotCount;
        ballots[currentId] = BallotInfo({
            ballotId: currentId,
            contractAddress: address(newVoting),
            owner: msg.sender,
            title: _title
        });

        emit BallotCreated(currentId, address(newVoting), msg.sender, _title);

        ballotCount = currentId + 1;

        return currentId;
    }

    function getBallotCount() external view returns (uint256) {
        return ballotCount;
    }

    function getVotingContract(
        uint256 _ballotId
    ) external view returns (Voting) {
        return Voting(ballots[_ballotId].contractAddress);
    }

    function getAllBallots() external view returns (BallotInfo[] memory) {
        uint256 count = ballotCount;
        BallotInfo[] memory all = new BallotInfo[](count);

        for (uint256 i = 0; i < count;) {
            all[i] = ballots[i];
            unchecked { i++; }
        }

        return all;
    }
}
