using Ploeh.AutoFixture;

namespace AutoDbQuickStart
{
    internal class BuildTreeCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new BuildTreeProvider());
        }
    }
}