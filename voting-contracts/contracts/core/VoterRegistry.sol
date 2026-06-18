// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "../security/AccessControl.sol";

contract VoterRegistry {
    enum VoterStatus {
        Unregistered,
        Pending,
        Approved,
        Revoked
    }

    struct Voter {
        address wallet;
        VoterStatus status;
        uint256 registeredAt;
        uint256 approvedAt;
        string name;
    }

    AccessControl public acl;
    bytes32 private immutable ADMIN_ROLE;
    bytes32 private immutable BALLOT_ADMIN_ROLE;

    mapping(address => Voter) private voters;
    address[] private voterList;

    mapping(address => mapping(address => bool)) private ballotEligibility;

    event VoterRegistered(address indexed voter, string name);
    event VoterApproved(address indexed voter, address indexed approvedBy);
    event VoterRevoked(address indexed voter, address indexed revokedBy);
    event EligibilitySet(
        address indexed ballot,
        address indexed voter,
        bool eligible
    );

    modifier onlyAdmin() {
        require(acl.hasRole(ADMIN_ROLE, msg.sender), "Registry: not admin");
        _;
    }

    modifier onlyBallotAdmin(address ballot) {
        require(
            acl.isAuthorized(ballot, BALLOT_ADMIN_ROLE, msg.sender),
            "Registry: not ballot admin"
        );
        _;
    }

    constructor(address _acl) {
        acl = AccessControl(_acl);
        ADMIN_ROLE = acl.ADMIN();
        BALLOT_ADMIN_ROLE = acl.BALLOT_ADMIN();
    }

    // AUTO-APPROVE REGISTRATION
    function register(string calldata _name) external {
        require(
            voters[msg.sender].status == VoterStatus.Unregistered,
            "Already registered"
        );
        require(bytes(_name).length > 0, "Name required");

        voters[msg.sender] = Voter({
            wallet: msg.sender,
            status: VoterStatus.Approved,
            registeredAt: block.timestamp,
            approvedAt: block.timestamp,
            name: _name
        });

        voterList.push(msg.sender);

        emit VoterRegistered(msg.sender, _name);
        emit VoterApproved(msg.sender, msg.sender);
    }

    function approveVoter(address _voter) external onlyAdmin {
        require(voters[_voter].status == VoterStatus.Pending, "Not pending");
        voters[_voter].status = VoterStatus.Approved;
        voters[_voter].approvedAt = block.timestamp;
        emit VoterApproved(_voter, msg.sender);
    }

    function batchApprove(address[] calldata _voters) external onlyAdmin {
        uint256 len = _voters.length;
        for (uint256 i = 0; i < len;) {
            address v = _voters[i];
            if (voters[v].status == VoterStatus.Pending) {
                voters[v].status = VoterStatus.Approved;
                voters[v].approvedAt = block.timestamp;
                emit VoterApproved(v, msg.sender);
            }
            unchecked { i++; }
        }
    }

    function revokeVoter(address _voter) external onlyAdmin {
        require(voters[_voter].status == VoterStatus.Approved, "Not approved");
        voters[_voter].status = VoterStatus.Revoked;
        emit VoterRevoked(_voter, msg.sender);
    }

    function setEligibility(
        address ballot,
        address _voter,
        bool _eligible
    ) external onlyBallotAdmin(ballot) {
        ballotEligibility[ballot][_voter] = _eligible;
        emit EligibilitySet(ballot, _voter, _eligible);
    }

    function isApproved(address _voter) external view returns (bool) {
        return voters[_voter].status == VoterStatus.Approved;
    }

    function isEligible(
        address _ballot,
        address _voter
    ) external view returns (bool) {
        VoterStatus s = voters[_voter].status;

        if (s == VoterStatus.Revoked)
            return false;

        if (s == VoterStatus.Pending)
            return false;

        bool hasOverride = ballotEligibility[_ballot][_voter];

        return hasOverride || s == VoterStatus.Approved;
    }

    function getVoter(
        address _voter
    )
        external
        view
        returns (
            string memory name,
            VoterStatus status,
            uint256 registeredAt,
            uint256 approvedAt
        )
    {
        Voter storage v = voters[_voter];

        return (
            v.name,
            v.status,
            v.registeredAt,
            v.approvedAt
        );
    }

    function getVoterCount() external view returns (uint256) {
        return voterList.length;
    }

    function getVotersPaginated(
        uint256 _offset,
        uint256 _limit
    ) external view returns (Voter[] memory page) {
        uint256 total = voterList.length;

        if (_offset >= total) {
            return new Voter[](0);
        }

        uint256 end = _offset + _limit;

        if (end > total) {
            end = total;
        }

        page = new Voter[](end - _offset);

        uint256 size = end - _offset;
        for (uint256 i = 0; i < size;) {
            page[i] = voters[voterList[_offset + i]];
            unchecked { i++; }
        }
    }
}