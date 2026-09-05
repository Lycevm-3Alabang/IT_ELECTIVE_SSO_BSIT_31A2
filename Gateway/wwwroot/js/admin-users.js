// [BUENO] Search / Filter Table by Email
function filterUsersByEmail() {
    let input = document.getElementById('emailSearchInput').value.toLowerCase();
    let rows = document.querySelectorAll('#usersTable tbody tr');

    rows.forEach(row => {
        let emailCell = row.querySelector('.user-email');
        if (emailCell) {
            let emailText = emailCell.textContent.toLowerCase();
            row.style.display = emailText.includes(input) ? '' : 'none';
        }
    });
}

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

// [MANZANO] Form Submit & Inline Validation Errors
function handleCreateUserSubmit(e) {
    e.preventDefault();

    let passwordInput = document.getElementById('createPassword');
    let confirmInput = document.getElementById('createConfirmPassword');
    let errorDiv = document.getElementById('confirmPasswordError');

    if (passwordInput.value !== confirmInput.value) {
        confirmInput.classList.add('modal-dark-input-error');
        errorDiv.classList.remove('d-none');
        return;
    } else {
        confirmInput.classList.remove('modal-dark-input-error');
        errorDiv.classList.add('d-none');
    }

    let createModalEl = document.getElementById('createUserModal');
    let modalInstance = bootstrap.Modal.getInstance(createModalEl);
    if (modalInstance) modalInstance.hide();

    document.getElementById('createUserForm').reset();
}