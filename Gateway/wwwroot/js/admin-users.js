// [VILLAMOR] User Details Modal Loader
function loadUserDetails(email, status, groups, lastLogin) {
    document.getElementById('detailEmail').textContent = email;

    let statusBadge = document.getElementById('detailStatus');
    statusBadge.textContent = status;
    statusBadge.className = status === 'Active'
        ? 'badge badge-active-green'
        : 'badge badge-suspended-gray';

    document.getElementById('detailGroups').textContent = groups;
    document.getElementById('detailLastLogin').textContent = lastLogin;
}
