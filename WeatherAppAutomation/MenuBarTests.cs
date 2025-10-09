using FlaUI.Core.Conditions;
using FlaUI.UIA3;

namespace WeatherAppAutomation;

[TestFixture]
[NonParallelizable]
public class MenuBarTests : BaseTestFixture
{
    [SetUp]
    public override void Setup()
    {
        base.Setup();
    }

    [Test]
    public void TestRefresh()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());

        Assert.Pass();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
    }
}
