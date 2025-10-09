using NUnit.Framework;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using WeatherAppAutomation.Helpers;
using FlaUI.Core.AutomationElements;

namespace WeatherAppAutomation;

[TestFixture]
[NonParallelizable]
public class ForecastPageTests : BaseTestFixture
{
    string location1 = "Bury St Edmunds, England";

    [SetUp]
    public override void Setup()
    {
        base.Setup();
    }

    public AutomationElement _mainWindow => base.MainWindow;

    public AutomationElement? SubWindow =>
        _mainWindow.FindFirstDescendant(cf =>
             cf.ByName("Weather")
             .And(cf.ByClassName("Windows.UI.Core.CoreWindow"))
        );

    public AutomationElement? ChromeContent =>
        SubWindow?.FindFirstDescendant(cf =>
            cf.ByAutomationId("ChromeContent")
            .And(cf.ByControlType(ControlType.Custom))
        );

    public AutomationElement? ForeCastPage =>
        SubWindow?.FindAllDescendants(cf =>
            cf.ByAutomationId("WebView")
            .And(cf.ByControlType(ControlType.Pane))
        ).FirstOrDefault();

    [Test]
    public void Test1()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var subWindow = SubWindow!;
        Assert.That(subWindow?.IsOffscreen, Is.EqualTo(false), "Content window is not visible");
        var chromeContent = ChromeContent!;
        Assert.That(chromeContent?.IsOffscreen, Is.EqualTo(false), "Chrome conetent group is not visble");
        var forecastPage = ForeCastPage!;
        var locationList = forecastPage.FindFirstDescendant(cf.ByClassName("cardContainer-DS-EntryPoint1-1")).AsListBox();
        var btnWeatherLocation = locationList?.FindFirstDescendant(cf.ByClassName("weather_carousel_card_name-DS-EntryPoint1-1")).AsButton();
        btnWeatherLocation?.Name.Contains(location1);
        var btnLocationOptions = locationList?.FindFirstDescendant(cf.ByClassName("weather_carousel_card_dots-DS-EntryPoint1-1")).AsButton();
        btnLocationOptions?.Click();
        var listRemoveLocation = locationList?.FindFirstDescendant(cf.ByClassName("weather_carousel_card_popup-DS-EntryPoint1-1").And(cf.ByControlType(ControlType.List))).AsListBoxItem();
        var listItemRemove = listRemoveLocation?.FindAllChildren(cf.ByControlType(ControlType.ListItem)).FirstOrDefault();
        var btnRemoveLocation = listItemRemove?.FindAllChildren(cf.ByControlType(ControlType.Button)).FirstOrDefault();
        Assert.That(btnRemoveLocation?.Properties.Name, Is.EqualTo("Remove location"), "Remove location button not found in location options");
        Assert.Pass();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
    }
}
