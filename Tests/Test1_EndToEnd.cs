﻿using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

[TestFixture]
public class O2Tests
{
    IPlaywright? playwright;
    IBrowser? browser;
    IBrowserContext? context;
    IPage? page;

    [SetUp]
    public async Task SetUp()
    {
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        context = await browser.NewContextAsync(new BrowserNewContextOptions { RecordVideoDir = "Videos/" });
        page = await context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    { 
        if (context != null){ await context.CloseAsync(); }
        if (browser != null){ await browser.CloseAsync(); }
    }

    [Test]
    public async Task TestO2Website()
    {
        await page.GotoAsync("https://www.o2.sk/");
        Assert.AreEqual("O2 | Mobilné telefóny, internet a volacie programy", await page.TitleAsync());

        await page.ClickAsync("a[href='/ponuka/telefony-a-zariadenia']");
        await page.ClickAsync("a[data-gtmname='Mobilne telefony']");

        // Vyber mobilu | Vyber pausalu | Vyska akontacie | Nakupny kosik 
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

        // FORMULAR
        await page.FillAsync("input[name='firstName']", "John");
        await page.FillAsync("input[name='lastName']", "Smith");
        await page.FillAsync("input[name='contactPhone']", "0904123123");
        await page.FillAsync("input[name='email']", "john@smith.com");
        await page.FillAsync("input[name='street']", "Dunajská");
        await page.FillAsync("input[name='streetNumber']", "5");
        await page.FillAsync("input[name='city']", "Bratislava");
        await page.FillAsync("input[name='zip']", "81106");

        // Rodne cislo generator
        Random randCislo = new Random();
        long rodneCisloTest = 0;
        long rodneCislo = 0;
        while (true)
        {
            int rok = randCislo.Next(50, 100);
            int mesiac = randCislo.Next(10, 13);
            int den = randCislo.Next(10, 29);
            int koniec = randCislo.Next(1000, 10000);
            string rodneCisloStr = rok.ToString() + mesiac.ToString() + den.ToString() + koniec.ToString();
            rodneCisloTest = long.Parse(rodneCisloStr);

            if (rodneCisloTest % 11 == 0)
            {
                rodneCislo = rodneCisloTest;
                break;
            }
        }

        await page.FillAsync("input[name='birthNumber']", rodneCislo.ToString());
        await page.FillAsync("input[name='idCardNumber']", "SP123123");
        await page.Keyboard.PressAsync("Tab");
        await page.Keyboard.TypeAsync("01.04.2026");
        await page.ClickAsync("input[name='idCardNumber']");
        await page.ScreenshotAsync(new() { Path = "Screenshots/formular_zakaznika.png", FullPage = true });
        await page.ClickAsync("[data-testid='checkout-submit']");

        // Dorucenie a platba
        await page.ClickAsync("[id='o2-radio-7']");
        await page.ClickAsync("[data-testid='checkout-submit']");
        Assert.AreEqual("Nákup", await page.TitleAsync());

        await page.WaitForSelectorAsync("h1:has-text('Zhrnutie')");
        var h1 = page.Locator("h1[class*='my-3']");
        var h1Text = await h1.InnerTextAsync();
        Assert.AreEqual("Zhrnutie", h1Text);

        await Task.Delay(3000);
    }
}