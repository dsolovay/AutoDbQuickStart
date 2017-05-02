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
        [Fact]
        public void AutoContentCustomizationAddsItemsToDb()
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


        [Theory, AutoAbcXyzTree]
        public void AutoDbDataCanShowOneTree(Db db)
        {
            db.GetItem("/sitecore/content/a/b/c").Should().NotBeNull();
            db.GetItem("/sitecore/content/x/y/z").Should().NotBeNull();
        }

        [Theory, AutoAbcTree]
        public void AutoDbDataCanCombineTrees(Db db)
        {
            db.GetItem("/sitecore/content/a/b/c").Should().NotBeNull();
            db.GetItem("/sitecore/content/x/y/z").Should().BeNull();
        }
    }

    internal class AutoAbcTreeAttribute : AutoDataAttribute
    {
        public AutoAbcTreeAttribute()
        {
            Fixture.Customize(new AutoDbCustomization())
                .Customize(new AddABC());
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
