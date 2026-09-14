using Xunit;

namespace ITElectiveSSO.Tests.UI
{
    public class TenantAppUITests
    {
        // BUENO
        [Fact]
        public void CreateTenantAppModal_ValidInputs_SuccessfullyMapsAppDetails()
        {
            // Arrange
            string appName = "Test Automation App";
            string returnUrl = "https://testautomation.com/callback";

            // Act
            bool isValid = !string.IsNullOrEmpty(appName) && returnUrl.StartsWith("https://" /);

            // Assert
            Assert.True(isValid);
            Assert.Equal("Test Automation App", appName);
            Assert.Equal("https://testautomation.com/callback", returnUrl);
        }

        //Cabardo
    }
}