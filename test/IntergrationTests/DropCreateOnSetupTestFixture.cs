using NUnit.Framework;

namespace IntergrationTests
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
