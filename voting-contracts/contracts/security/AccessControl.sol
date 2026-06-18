// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract AccessControl {

    // ─── Role constants ──────────────────────────────────────
    bytes32 public constant SUPER_ADMIN  = keccak256("SUPER_ADMIN");
    bytes32 public constant ADMIN        = keccak256("ADMIN");
    bytes32 public constant BALLOT_ADMIN = keccak256("BALLOT_ADMIN");
    bytes32 public constant AUDITOR      = keccak256("AUDITOR");
    bytes32 public constant VOTER        = keccak256("VOTER");

    // ─── Types ───────────────────────────────────────────────
    struct RoleData {
        mapping(address => bool) members;
        bytes32 adminRole;
        uint256 memberCount;
    }

    struct RoleInfo {
        bytes32 role;
        address account;
        uint256 grantedAt;
        address grantedBy;
    }

    // ─── State ───────────────────────────────────────────────
    mapping(bytes32 => RoleData)  private _roles;
    mapping(address => bytes32[]) private _accountRoles;

    RoleInfo[] public roleHistory;

    mapping(address => mapping(bytes32 => mapping(address => bool)))
        private _ballotRoles;

    // ─── Events ──────────────────────────────────────────────
    event RoleGranted(bytes32 indexed role, address indexed account, address indexed sender);
    event RoleRevoked(bytes32 indexed role, address indexed account, address indexed sender);
    event RoleAdminChanged(bytes32 indexed role, bytes32 indexed previousAdmin, bytes32 indexed newAdmin);
    event BallotRoleGranted(address indexed ballot, bytes32 indexed role, address indexed account);
    event BallotRoleRevoked(address indexed ballot, bytes32 indexed role, address indexed account);

    // ─── Modifiers ───────────────────────────────────────────
    modifier onlyRole(bytes32 role) {
        require(hasRole(role, msg.sender), "AccessControl: missing role");
        _;
    }

    // ─── Constructor ─────────────────────────────────────────
    constructor() {
        _setRoleAdmin(SUPER_ADMIN,  SUPER_ADMIN);
        _setRoleAdmin(ADMIN,        SUPER_ADMIN);
        _setRoleAdmin(BALLOT_ADMIN, ADMIN);
        _setRoleAdmin(AUDITOR,      ADMIN);
        _setRoleAdmin(VOTER,        BALLOT_ADMIN);

        _grantRole(SUPER_ADMIN, msg.sender, msg.sender);
    }

    // ─── Core role management ────────────────────────────────
    function hasRole(bytes32 role, address account)
        public view returns (bool)
    {
        return _roles[role].members[account];
    }

    function grantRole(bytes32 role, address account)
        external
        onlyRole(_roles[role].adminRole)
    {
        _grantRole(role, account, msg.sender);
    }

    function revokeRole(bytes32 role, address account)
        external
        onlyRole(_roles[role].adminRole)
    {
        _revokeRole(role, account, msg.sender);
    }

    function renounceRole(bytes32 role) external {
        _revokeRole(role, msg.sender, msg.sender);
    }

    function setRoleAdmin(bytes32 role, bytes32 adminRole)
        external
        onlyRole(SUPER_ADMIN)
    {
        _setRoleAdmin(role, adminRole);
    }

    // ─── Ballot-scoped roles ─────────────────────────────────
    function grantBallotRole(address ballot, bytes32 role, address account)
        external
        onlyRole(ADMIN)
    {
        require(ballot != address(0), "Zero ballot address");
        _ballotRoles[ballot][role][account] = true;
        emit BallotRoleGranted(ballot, role, account);
    }

    function revokeBallotRole(address ballot, bytes32 role, address account)
        external
        onlyRole(ADMIN)
    {
        _ballotRoles[ballot][role][account] = false;
        emit BallotRoleRevoked(ballot, role, account);
    }

    function hasBallotRole(address ballot, bytes32 role, address account)
        public view returns (bool)
    {
        return _ballotRoles[ballot][role][account];
    }

    function isAuthorized(address ballot, bytes32 role, address account)
        external view returns (bool)
    {
        return hasRole(role, account) || hasBallotRole(ballot, role, account);
    }

    // ─── Introspection ───────────────────────────────────────
    function getRoleAdmin(bytes32 role) external view returns (bytes32) {
        return _roles[role].adminRole;
    }

    function getRoleMemberCount(bytes32 role)
        external view returns (uint256)
    {
        return _roles[role].memberCount;
    }

    function getRolesOf(address account)
        external view returns (bytes32[] memory)
    {
        return _accountRoles[account];
    }

    function getRoleHistory(uint256 _offset, uint256 _limit)
        external view
        returns (RoleInfo[] memory page, uint256 total)
    {
        total = roleHistory.length;
        if (_offset >= total) return (new RoleInfo[](0), total);

        uint256 end = _offset + _limit;
        if (end > total) end = total;

        uint256 size = end - _offset;
        page = new RoleInfo[](size);
        for (uint256 i = 0; i < size;) {
            page[i] = roleHistory[_offset + i];
            unchecked { i++; }
        }
    }

    // ─── Batch helpers ───────────────────────────────────────
    function batchGrantRole(bytes32 role, address[] calldata accounts)
        external
        onlyRole(_roles[role].adminRole)
    {
        uint256 len = accounts.length;
        for (uint256 i = 0; i < len;) {
            _grantRole(role, accounts[i], msg.sender);
            unchecked { i++; }
        }
    }

    function batchRevokeRole(bytes32 role, address[] calldata accounts)
        external
        onlyRole(_roles[role].adminRole)
    {
        uint256 len = accounts.length;
        for (uint256 i = 0; i < len;) {
            _revokeRole(role, accounts[i], msg.sender);
            unchecked { i++; }
        }
    }

    // ─── Internals ───────────────────────────────────────────
    function _grantRole(bytes32 role, address account, address sender) internal {
        if (!_roles[role].members[account]) {
            _roles[role].members[account] = true;
            _roles[role].memberCount++;
            _accountRoles[account].push(role);
            roleHistory.push(RoleInfo({
                role:      role,
                account:   account,
                grantedAt: block.timestamp,
                grantedBy: sender
            }));
            emit RoleGranted(role, account, sender);
        }
    }

    function _revokeRole(bytes32 role, address account, address sender) internal {
        if (_roles[role].members[account]) {
            _roles[role].members[account] = false;
            _roles[role].memberCount--;
            _removeAccountRole(account, role);
            roleHistory.push(RoleInfo({
                role:      role,
                account:   account,
                grantedAt: block.timestamp,
                grantedBy: sender
            }));
            emit RoleRevoked(role, account, sender);
        }
    }

    function _setRoleAdmin(bytes32 role, bytes32 adminRole) internal {
        bytes32 prev = _roles[role].adminRole;
        _roles[role].adminRole = adminRole;
        emit RoleAdminChanged(role, prev, adminRole);
    }

    function _removeAccountRole(address account, bytes32 role) internal {
        bytes32[] storage arr = _accountRoles[account];
        uint256 len = arr.length;
        for (uint256 i = 0; i < len;) {
            if (arr[i] == role) {
                arr[i] = arr[len - 1];
                arr.pop();
                break;
            }
            unchecked { i++; }
        }
    }
}