using System.Data.Entity;
using DataAccess;

namespace WebUI.CompositionRoot
{
    public static class StoreInitilizer
    {
        public static void SemanticNetworkStoreInitilize()
        {
            Database.SetInitializer(new SemanticNetworkDbInitializer());
        }
    }
}
