using NUnit.Framework;

namespace OW.Experts.IntergrationTests
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
