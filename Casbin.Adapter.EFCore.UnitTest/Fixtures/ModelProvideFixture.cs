using System.IO;
using NetCasbin.Model;

namespace Casbin.Adapter.EFCore.UnitTest.Fixtures
{
    public class ModelProvideFixture
    {
        private readonly string _rbacModelText = File.ReadAllText("examples/rbac_model.conf");

        private readonly string _abacModelText = File.ReadAllText("examples/abac_model.conf");

        public Model GetNewRbacModel()
        {
            return Model.CreateDefaultFromText(_rbacModelText);
        }

        public Model GetNewAbacModel()
        {
            return Model.CreateDefaultFromText(_abacModelText);
        }
    }
}