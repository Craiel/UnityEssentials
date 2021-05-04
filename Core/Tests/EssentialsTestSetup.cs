namespace Craiel.UnityEssentials.Tests
{
    using NUnit.Framework;

    [SetUpFixture]
    public class EssentialsTestSetup
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [OneTimeSetUp]
        public void Setup()
        {
            UnitTestUtils.Log("Essential Test Setup Complete");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }
    }
}