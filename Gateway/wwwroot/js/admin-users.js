// [MANZANO] Active Toggle Switch Handler
function toggleUserStatus(userId, switchElement) {
    let row = switchElement.closest('tr');
    let badge = row.querySelector('.status-badge');
    let toggleLabel = row.querySelector('.toggle-label');
    let isChecked = switchElement.checked;

    if (isChecked) {
        badge.className = 'badge badge-active-green status-badge';
        badge.textContent = 'Active';
        if (toggleLabel) {
            toggleLabel.textContent = 'ON';
            toggleLabel.className = 'me-2 fw-bold small text-light toggle-label';
        }
    } else {
        badge.className = 'badge badge-suspended-gray status-badge';
        badge.textContent = 'Suspended';
        if (toggleLabel) {
            toggleLabel.textContent = 'OFF';
            toggleLabel.className = 'me-2 fw-bold small text-secondary toggle-label';
        }
    }
}

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
