namespace UnoPlatform_v5.Tests
{
    public class AppInfoTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AppInfoCreation()
        {
            var appInfo = new AppConfig { Environment = "Test" };

            appInfo.Should().NotBeNull();
            appInfo.Environment.Should().Be("Test");
        }
    }
}