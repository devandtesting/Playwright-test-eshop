using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

[TestFixture]
public class O2Tests2
{
    IPlaywright? playwright;
    IBrowser? browser;
    IBrowserContext? context;
    IPage? page;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        context = await browser.NewContextAsync(new BrowserNewContextOptions { RecordVideoDir = "Videos/" });
        page = await context.NewPageAsync();
    }

    [TestCase("Path to customer form")]
    public async Task EmailFieldValidationTest1(string testName)
    {
        Console.WriteLine(testName);
        // Web o2
        await page.GotoAsync("https://www.o2.sk/");

        // Vyber mobilu | Vyber pausalu | Vyska akontacie | Nakupny kosik 
        await page.ClickAsync("a[href='/ponuka/telefony-a-zariadenia']");
        await page.ClickAsync("a[data-gtmname='Mobilne telefony']");
        await page.ClickAsync("[data-testid='sku-card-box-9f4fe78f-18bc-4f7b-9bbe-3e8329294413']");
        await page.ClickAsync("[data-testid='hw-detail-pricing-buy-with-tariff']");
        await page.ClickAsync("[data-testid='login.cancel']");
        await page.ClickAsync("[class*='ss-main']");
        await page.ClickAsync("div.ss-option:has-text('29')");
        await page.ScreenshotAsync(new() { Path = "Screenshots/vybrany_pausal.png" });
        await page.ClickAsync("[data-testid='chooseTariff.submit']");
        await page.ClickAsync("o2-button[data-testid='submit-choose-transaction']");
        await page.ClickAsync("[data-testid='crossSell.submit']");
        await page.ClickAsync("[data-testid='hwSliderPage.submit']");
        await page.ClickAsync("[data-testid='cart-summary__proceed-to-checkout']");
        await page.ClickAsync("[data-testid='checkout-submit']");
        await page.WaitForSelectorAsync("input[name='email']");
        Assert.IsTrue(await page.IsVisibleAsync("input[name='email']"));
    }

    [TestCase("Test for invalid email format", "john@com")]
    [TestCase("Test for invalid email format", "john.com")]
    [TestCase("Test for invalid email format", "johncom")]
    [TestCase("Test for invalid email format", "123456")]
    [TestCase("Test for invalid email format", "###")]
    public async Task EmailFieldValidationTest2(string testName, string email)
    {
        Console.WriteLine(testName+": "+email);
        await page.FillAsync("input[name='email']", email);
        await page.Keyboard.PressAsync("Tab");
        await page.Keyboard.PressAsync("Tab");
        var helpTextElement = page.Locator("span[slot='help-text']:text('email')");
        await helpTextElement.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.IsTrue(await helpTextElement.IsVisibleAsync(), "The helpText element for email is not visible.");
    }

    [TestCase("Test for valid email format", "john@smith.com")]
    public async Task EmailFieldValidationTest5(string testName, string email)
    {
        Console.WriteLine(testName+": "+email);
        await page.FillAsync("input[name='email']", email);
        await page.Keyboard.PressAsync("Tab");
        await page.Keyboard.PressAsync("Tab");
        await Task.Delay(1000);
        var helpTextElement = page.Locator("span[slot='help-text']:text('email')");
        bool isVisible = await helpTextElement.IsVisibleAsync(new LocatorIsVisibleOptions { Timeout = 2000 });
        Assert.IsFalse(isVisible, "The helpText element for email should not be visible.");
    }

    [OneTimeTearDown]
    public async Task FinalTearDown()
    {
        if (context != null) { await context.CloseAsync(); }
        if (browser != null) { await browser.CloseAsync(); }
    }
}
