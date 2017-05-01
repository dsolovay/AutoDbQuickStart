using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.FakeDb;
using Ploeh.AutoFixture;
using Xunit;
using Sitecore.FakeDb.AutoFixture;
using Sitecore.Data.Items;
using Sitecore.Data;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.Diagnostics;
using Xunit.Abstractions;

namespace AutoDbQuickStart
{

    public class CreatingItems
    {
        private ITestOutputHelper _output;

        public CreatingItems(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact(Skip="Temporary diagnostic test")]
        public void WhereAmI()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.GetFiles(path).Select(s => s.EndsWith("license.xml")).Count().Should().BeGreaterThan(0);
            
            path.Should().Be("");
        }

        [Fact]
        public void FakeDBWorking()
        {
            using (var db = new Db
            {
                new DbItem("Home") {{"Title", "Welcome!"}}
            })
            {
                Sitecore.Data.Items.Item home = db.GetItem("/sitecore/content/home");
                Xunit.Assert.Equal("Welcome!", home["Title"]);
            }
        }

        [Fact]
        public void MakeItem()
        { 
            IFixture fixture = new Fixture();
            fixture.Customize(new AutoDbCustomization()).Customize(new AutoContentItemCustomization());
            var item = fixture.Create<Item>();
            var database = fixture.Create<Database>();
            var newItem = database.GetItem(item.ID);
            newItem.Should().NotBeNull();     
        }

        [Fact]
        public void CreateItemWithNameUsingAutoFixture()
        {
   
            IFixture fixture = new Fixture();
            string anonymousString = fixture.Create<string>();
            fixture.Customize(new AutoDbCustomization()).Customize(new AutoContentItemCustomization());

            fixture.Inject(anonymousString);  

            var item = fixture.Create<Item>();
            var database = fixture.Create<Database>();
            var newItem = database.GetItem(item.ID);
            newItem.Name.Should().Be(anonymousString);
            database.GetItem($"/sitecore/content/{anonymousString}").Should().NotBeNull();
        }

        [Fact]
        public void CanCreateItemTree()
        {
            Database database = new Fixture().Customize(new AutoDbCustomization()).Customize( new BuildTreeCustomization()).Create<Database>();
            database.Should().NotBeNull();
            database.GetItem("/sitecore/content/a/b/c").Should().NotBeNull();
        }

        [Fact]
        public void CanCreateItemTreeWithPostProcessor()
        {
            Db db = new Fixture().Customize(new AutoDbCustomization()).Customize(new AddABC()).Create<Db>();
            db.Should().NotBeNull();
            db.GetItem("/sitecore/content/a/b/c").Should().NotBeNull();
            db.GetItem("/sitecore/content/x/y/z").Should().BeNull();
        }

   
        [Fact]
        public void CanComposeTrees()
        {
            Db db = new Fixture().Customize(new AutoDbCustomization()).Customize(new AddABC()).Customize(new AddXYZ()).Create<Db>();
             
            db.Should().NotBeNull();
            db.GetItem("/sitecore/content/a/b/c").Should().NotBeNull();
            db.GetItem("/sitecore/content/x/y/z").Should().NotBeNull();
        }


        [Fact]
        public void CanDefineCustomizations()
        {
            IFixture fixture = new Fixture();
            fixture.Customize(new AutoDbCustomization())
                .Customize(new AddABC())
                .Customize(new AddXYZ());
            Db db = fixture.Create<Db>();
            db.GetItem("/sitecore/content/a/b/c").Should().NotBeNull();
            db.GetItem("/sitecore/content/x/y/z").Should().NotBeNull();
        }

        [Theory, AutoAbcXyzTree]
        public void AutoDbData(Db db)
        {
            db.GetItem("/sitecore/content/a/b/c").Should().NotBeNull();
            db.GetItem("/sitecore/content/x/y/z").Should().NotBeNull();
        }


    }

    internal class AutoAbcXyzTreeAttribute : AutoDataAttribute
    {

        public AutoAbcXyzTreeAttribute()
        {
            Fixture.Customize(new AutoDbCustomization())
                .Customize(new AddABC())
                .Customize(new AddXYZ());
        }
    }
}
