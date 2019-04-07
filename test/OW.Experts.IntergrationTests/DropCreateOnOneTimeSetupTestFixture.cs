using NUnit.Framework;

namespace IntergrationTests
{
    public class DropCreateOnOneTimeSetupTestFixture : DropCreateTestFixture
    {
        [OneTimeSetUp]
        public virtual void OnOneTimeSetup()
        {
            DropCreate();
        }
    }
}
