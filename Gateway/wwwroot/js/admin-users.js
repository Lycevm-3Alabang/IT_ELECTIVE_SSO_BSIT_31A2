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

// MODIFIED: Connected to backend /Admin/Users/ToggleActive/{id}
async function toggleUserStatus(userId, checkbox) {
    const row = checkbox.closest('tr');
    const badge = row ? row.querySelector('.status-badge') : null;
    const toggleLabel = row ? row.querySelector('.toggle-label') : null;
    const isActive = checkbox.checked;

    // Instantly update badge UI in table
    if (badge) {
        badge.innerText = isActive ? 'Active' : 'Suspended';
        badge.className = isActive ? 'badge badge-active-green status-badge' : 'badge badge-suspended-gray status-badge';
    }

    if (toggleLabel) {
        toggleLabel.innerText = isActive ? 'ON' : 'OFF';
        toggleLabel.className = `me-2 fw-bold small toggle-label ${isActive ? 'text-light' : 'text-secondary'}`;
    }

    // Perform AJAX request to backend
    fetch(`/Admin/Users/ToggleActive/${userId}`, {
        method: 'POST',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
        .then(response => response.json())
        .then(data => {
            if (!data.success) {
                // Revert checkbox state on failure
                checkbox.checked = !isActive;
                alert('Failed to update status.');
            }
        })
        .catch(err => {
            console.error('Error toggling status:', err);
            checkbox.checked = !isActive;
        });
}

// [VILLAMOR] User Details Modal Loader
function loadUserDetails(buttonElement) {
    // Read static parameters if available on button dataset, fallback to table row content
    const row = buttonElement.closest('tr');

    const email = buttonElement.dataset.email || row?.querySelector('.user-email')?.innerText.trim() || '-';
    const createdAt = buttonElement.dataset.created || row?.cells[1]?.innerText.trim() || 'N/A';

    // Get live status from the toggle switch in the row
    const toggleCheckbox = row ? row.querySelector('.user-toggle-switch') : null;
    const isActive = toggleCheckbox ? toggleCheckbox.checked : (buttonElement.dataset.active === 'true');

    // Update Modal Fields
    document.getElementById('detailEmail').innerText = email;
    document.getElementById('detailGroups').innerText = 'User';
    document.getElementById('detailLastLogin').innerText = createdAt;

    // Update Status Badge dynamically
    const statusElem = document.getElementById('detailStatus');
    if (isActive) {
        statusElem.innerText = 'Active';
        statusElem.className = 'badge badge-active-green';
    } else {
        statusElem.innerText = 'Suspended';
        statusElem.className = 'badge badge-suspended-gray';
    }
}

// [FACTOR] Delete Confirmation Setup
let targetDeleteUserId = null;
function setDeleteUserTarget(userId, email) {
    targetDeleteUserId = userId;
    const emailTarget = document.getElementById('deleteTargetEmail') || document.getElementById('deleteUserEmail');
    if (emailTarget) emailTarget.textContent = email;

    const inputElem = document.getElementById('deleteUserIdInput');
    if (inputElem) inputElem.value = userId;
}

// MODIFIED: Updated to trigger post delete to controller
async function confirmDeleteUser() {
    if (targetDeleteUserId) {
        let response = await fetch(`/Admin/Users/Delete/${targetDeleteUserId}`, {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (response.ok) {
            window.location.reload(); // Reload table after deletion/suspension
        }
    }
    let deleteModalEl = document.getElementById('deleteUserModal');
    if (deleteModalEl) {
        let modalInstance = bootstrap.Modal.getInstance(deleteModalEl);
        if (modalInstance) modalInstance.hide();
    }
}

// [MANZANO] Form Submit & Inline Validation Errors
async function handleCreateUserSubmit(event) {
    event.preventDefault();

    const email = document.getElementById('createEmail').value;
    const password = document.getElementById('createPassword').value;
    const confirmPassword = document.getElementById('createConfirmPassword').value;
    const errorAlert = document.getElementById('createUserErrorAlert');
    const mismatchError = document.getElementById('confirmPasswordError');

    // Reset error displays
    if (errorAlert) errorAlert.classList.add('d-none');
    if (mismatchError) mismatchError.classList.add('d-none');

    // Client-side password validation
    if (password !== confirmPassword) {
        if (mismatchError) mismatchError.classList.remove('d-none');
        return;
    }

    const formData = new FormData();
    formData.append('Email', email);
    formData.append('Password', password);
    formData.append('ConfirmPassword', confirmPassword);

    try {
        const response = await fetch('/Admin/Users/Create', {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (response.ok) {
            window.location.reload();
        } else {
            if (errorAlert) {
                errorAlert.textContent = "Failed to create user. Ensure password meets complexity rules.";
                errorAlert.classList.remove('d-none');
            }
        }
    } catch (err) {
        console.error("Error creating user:", err);
    }
}
