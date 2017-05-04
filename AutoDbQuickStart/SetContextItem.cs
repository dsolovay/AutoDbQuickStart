using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.FakeDb.AutoFixture;
using Xunit;

namespace AutoDbQuickStart
{
    public class SetContextItem
    {
        [Theory,AutoContextData]
        public void CanCreateContextItem(Db db, [ContextItem]Item item)
        {
            Context.Item.Should().NotBeNull();
            Context.Item.ID.Should().Be(item.ID);
        }
        
        [Theory, AutoContextData]
        public void CanCreateNonContextItem(Db db, Item item)
        {
            Context.Item.Should().BeNull();
            item.Should().NotBeNull();
        }
        
        [Theory, AutoContextData]
        public void CanCreateNonContextItemSecond(Db db, [ContextItem]Item contextItem, Item nonContextItem)
        {
            contextItem.Should().NotBeNull();
            nonContextItem.Should().NotBeNull();
            Context.Item.ID.Should().Be(contextItem.ID);
        }

        [Theory, AutoContextData]
        public void CanCreateNonContextItemFirst(Db db, Item nonContextItem, [ContextItem]Item contextItem)
        {
            contextItem.Should().NotBeNull();
            nonContextItem.Should().NotBeNull();
            Context.Item.ID.Should().Be(contextItem.ID);
        }
    }

    public class AutoContextDataAttribute : AutoDataAttribute
    {
        public AutoContextDataAttribute()
        {
            // It is necessary to reset the context item to avoid having test side effects.  Doing this in 
            // the constructor does not work as the constructor code executes after the parameters are resolved, so the 
            // impact of the [ContextItem] attribute is undone.
            Context.Item = null;  

            Fixture.Customize(new AutoDbCustomization());
            Fixture.Customize(new AutoContentDbItemCustomization());
            Fixture.Customizations.Add(new Postprocessor(new ContextAttributeRelay(), new MakeContextItemCommand()));
        }
    }

    public class ContextAttributeRelay : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            ParameterInfo pi = request as ParameterInfo;
            if (pi != null && pi.CustomAttributes.Any(data => data.AttributeType.Equals(typeof(ContextItemAttribute))))
            {
                return context.Resolve(typeof(Item));
            }
            return new NoSpecimen();
        }
    }

    public class MakeContextItemCommand : ISpecimenCommand
    {
        // The command does not execute if the specimen builder returns NoSpecimen(), so no 
        // additional checks are required here.  See 
        // https://github.com/AutoFixture/AutoFixture/blob/ffaa6c510f8a952a0b896bfa18d8402428f2d266/Src/AutoFixture/Kernel/Postprocessor.cs#L267-L269
        public void Execute(object specimen, ISpecimenContext context)
        {
            Context.Item = (Item) specimen;
        }
    }

    /// <summary>
    /// Indicates a <see cref="Sitecore.Context.Item"/> that should be assigned to Sitecore.Context.Item.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ContextItemAttribute : Attribute
    {
    }
}
