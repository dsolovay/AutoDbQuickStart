using System;
using Ploeh.AutoFixture.Kernel;
using Sitecore.Data;
using Sitecore.FakeDb;

namespace AutoDbQuickStart
{
    internal class BuildTreeProvider : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            SeededRequest seededRequest = request as SeededRequest;
            
            bool? seededDatabaseRequest = seededRequest.Request?.Equals(typeof(Database));
            if ( !typeof(Database).Equals(request) &&  !(seededDatabaseRequest.HasValue && seededDatabaseRequest.Value))
                return new NoSpecimen();
            var db = context.Resolve(typeof(Db)) as Db;

            db.Add(
                new Sitecore.FakeDb.DbItem("a")
                {
                    new Sitecore.FakeDb.DbItem("b")
                    {
                        new Sitecore.FakeDb.DbItem("c")
                    }
                });
            return db.Database;

        }
    }
}