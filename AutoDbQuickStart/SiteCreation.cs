using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Exceptions;
using Sitecore.FakeDb;
using Sitecore.FakeDb.AutoFixture;
using Sitecore.FakeDb.Sites;
using Sitecore.Sites;
using Xunit;
using Attribute = System.Attribute;

namespace AutoDbQuickStart
{
    public class SiteCreation
    {
        [Theory, AutoData]
        public void CanCreateSite(IFixture fixture)
        {
            fixture.Customizations.Add(new FakeSiteContextBuilder());
            SiteContext context = fixture.Create<SiteContext>();
            context.Should().NotBeNull();
        }

        [Theory, AutoSiteData]
        public void GetSiteFromAttribute(SiteContext siteContext)
        {
            siteContext.Should().NotBeNull();
        }
    }
    

    public class AutoSiteDataAttribute : AutoDataAttribute
    {
        public AutoSiteDataAttribute()
        {
            Fixture.Customizations.Add(new FakeSiteContextBuilder());
        }
    }

    public class FakeSiteContextBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            Type t = request as Type;
            if (t!=null && typeof(SiteContext).IsAssignableFrom(t))
            {
                return new FakeSiteContext(
                    new StringDictionary
                    {
                        {"enableWebEdit", "true"},
                        {"masterDatabase", "master"},
                    });
            }
            return new NoSpecimen();
        }
    }
}
