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

    // Get Anti-Forgery Token if available on the page
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : '';

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
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': token
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (!data.success) {
                // Revert checkbox state on failure
                checkbox.checked = !isActive;
                if (badge) {
                    badge.innerText = !isActive ? 'Active' : 'Suspended';
                    badge.className = !isActive ? 'badge badge-active-green status-badge' : 'badge badge-suspended-gray status-badge';
                }
                if (toggleLabel) {
                    toggleLabel.innerText = !isActive ? 'ON' : 'OFF';
                    toggleLabel.className = `me-2 fw-bold small toggle-label ${!isActive ? 'text-light' : 'text-secondary'}`;
                }
                alert('Failed to update status.');
            }
        })
        .catch(err => {
            console.error('Error toggling status:', err);
            // Revert checkbox state on error
            checkbox.checked = !isActive;
            if (badge) {
                badge.innerText = !isActive ? 'Active' : 'Suspended';
                badge.className = !isActive ? 'badge badge-active-green status-badge' : 'badge badge-suspended-gray status-badge';
            }
            if (toggleLabel) {
                toggleLabel.innerText = !isActive ? 'ON' : 'OFF';
                toggleLabel.className = `me-2 fw-bold small toggle-label ${!isActive ? 'text-light' : 'text-secondary'}`;
            }
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

    const emailInput = document.getElementById('createEmail');
    const passwordInput = document.getElementById('createPassword');
    const confirmPasswordInput = document.getElementById('createConfirmPassword');

    const emailError = document.getElementById('emailError');
    const passwordError = document.getElementById('passwordError');
    const confirmPasswordError = document.getElementById('confirmPasswordError');

    // Reset error displays
    if (emailError) { emailError.textContent = ''; emailError.classList.add('d-none'); }
    if (passwordError) { passwordError.textContent = ''; passwordError.classList.add('d-none'); }
    if (confirmPasswordError) { confirmPasswordError.classList.add('d-none'); }

    // Client-side password mismatch validation
    if (passwordInput.value !== confirmPasswordInput.value) {
        if (confirmPasswordError) {
            confirmPasswordError.textContent = "Passwords do not match!";
            confirmPasswordError.classList.remove('d-none');
        }
        return;
    }

    const formData = new FormData();
    formData.append('Email', emailInput.value.trim());
    formData.append('Password', passwordInput.value);
    formData.append('ConfirmPassword', confirmPasswordInput.value);

    try {
        const response = await fetch('/Admin/Users/Create', {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        // If creation succeeded (controller redirected to Index)
        if (response.redirected) {
            window.location.href = response.url;
            return;
        }

        const responseText = await response.text();

        // Parse returned HTML from controller to extract ModelState validation errors
        const parser = new DOMParser();
        const doc = parser.parseFromString(responseText, 'text/html');
        const validationSummary = doc.querySelector('.validation-summary-errors, [data-valmsg-summary="true"]');

        if (validationSummary && validationSummary.textContent.trim() !== '') {
            const errorText = validationSummary.textContent.trim();

            // Direct error to specific input based on message content
            if (errorText.toLowerCase().includes('email')) {
                if (emailError) {
                    emailError.textContent = errorText;
                    emailError.classList.remove('d-none');
                }
            } else if (errorText.toLowerCase().includes('password')) {
                if (passwordError) {
                    passwordError.textContent = errorText;
                    passwordError.classList.remove('d-none');
                }
            } else {
                if (emailError) {
                    emailError.textContent = errorText;
                    emailError.classList.remove('d-none');
                }
            }
        } else if (!response.ok) {
            if (passwordError) {
                passwordError.textContent = "Failed to create user. Ensure password meets complexity rules.";
                passwordError.classList.remove('d-none');
            }
        } else {
            window.location.reload();
        }
    } catch (err) {
        console.error("Error creating user:", err);
    }
}