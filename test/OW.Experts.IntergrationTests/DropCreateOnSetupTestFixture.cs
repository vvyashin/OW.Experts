using NUnit.Framework;

namespace OW.Experts.IntergrationTests
{
    public class DropCreateOnSetupTestFixture : DropCreateTestFixture
    {
        [SetUp]
        public void OnTestSetup()
        {
            DropCreate();
        }
    }
}