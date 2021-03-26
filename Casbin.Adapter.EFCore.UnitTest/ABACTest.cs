using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Adapter.EFCore.Entities;
using Casbin.Adapter.EFCore.UnitTest.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCasbin;
using NetCasbin.Persist;
using Xunit;

namespace Casbin.Adapter.EFCore.UnitTest
{
    public class ABACTest : TestUtil, IClassFixture<ModelProvideFixture>, IDisposable
    {
        private readonly ModelProvideFixture _modelProvideFixture;
        private readonly CasbinDbContext<Guid> _context;

        public ABACTest(ModelProvideFixture modelProvideFixture)
        {
            _modelProvideFixture = modelProvideFixture;

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            var options = new DbContextOptionsBuilder<CasbinDbContext<Guid>>()
                .UseSqlServer(config.GetConnectionString("Default"))
                .Options;

            _context = new CasbinDbContext<Guid>(options);
            _context.Database.EnsureCreated();

            InitPolicy(_context);
        }

        public void Dispose()
        {
            Dispose(_context);
        }

        private void Dispose(CasbinDbContext<Guid> context)
        {
            context.RemoveRange(context.CasbinRule);
            context.SaveChanges();
        }

        private static void InitPolicy(CasbinDbContext<Guid> context)
        {
            context.CasbinRule.Add(new CasbinRule<Guid>
            {
                PType = "p",
                V0 = "r.sub.Age > 18 && r.sub.Age < 60",
                V1 = "/data1",
                V2 = "read",
            });
            context.SaveChanges();
        }

        [Fact]
        public void TestAgeAttribute()
        {
            var adapter = new EFCoreAdapter<Guid>(_context);
            var enforcer = new Enforcer(_modelProvideFixture.GetNewAbacModel(), adapter);

            enforcer.LoadPolicy();

            var flag = enforcer.Enforce(new
            {
                Age = 30
            }, "/data1", "read");

            Assert.True(flag);
        }

    
    }
}
