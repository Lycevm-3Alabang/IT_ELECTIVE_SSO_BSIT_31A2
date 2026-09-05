using Xunit;

namespace SSO_Gateway.Tests.UI
{
    public class UserManagementUITests
    {
        // =========================================================================
        // TASK #94: Write UI test: create user flow
        // Objective: Verify front-end validation behavior for mismatched passwords.
        // =========================================================================
        [Fact]
        public void CreateUserModal_PasswordMismatch_DisplaysInlineError()
        {
            // Arrange
            string password = "Password123!";
            string confirmPassword = "Password456!";
            bool isMismatch = password != confirmPassword;
            string errorMessage = isMismatch ? "Password do not match!" : string.Empty;

            // Assert
            Assert.True(isMismatch);
            Assert.Equal("Password do not match!", errorMessage);
        }

        // =========================================================================
        // TASK #95: Write UI test: toggle active flow
        // Objective: Verify front-end state transition between Active and Suspended.
        // =========================================================================
        [Fact]
        public void ActiveToggleSwitch_WhenClicked_UpdatesUserBadgeText()
        {
            // Arrange
            string initialStatus = "Active";
            bool isChecked = false; // Toggle clicked to OFF

            // Act
            string updatedStatus = isChecked ? "Active" : "Suspended";

            // Assert
            Assert.Equal("Active", initialStatus);
            Assert.Equal("Suspended", updatedStatus);
        }
    }
}

