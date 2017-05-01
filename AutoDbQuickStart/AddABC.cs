using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;
using Ploeh.AutoFixture.Kernel;
using Sitecore.FakeDb;

namespace AutoDbQuickStart
{
    internal class AddABC : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            Db db = fixture.Create<Db>();
            DbItem root = new DbItem("a")
            {
                new DbItem("b")
                {
                    new DbItem("c")
                }
            };
            db.Add(root);
        }
    }

    internal class AddXYZ : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            Db db = fixture.Create<Db>();
            DbItem root = new DbItem("x")
            {
                new DbItem("y")
                {
                    new DbItem("z")
                }
            };
            db.Add(root);
        }
    }


}