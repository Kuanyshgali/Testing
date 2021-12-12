using NUnit.Framework;

using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace AutomatedTests
{
    public class LamodaTests
    {
        WebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.lamoda.ru/");
            driver.FindElement(By.XPath("//button[contains(.,'Хорошо')]")).Click();
        }

        [Test]
        public void TestPriceFilter()
        {
            string charsToTrim = "( \\.? )";

            driver.FindElement(By.XPath("(//a[@data-genders='men'])")).Click();
            driver.FindElement(By.XPath("(//div[@class='sub-popup-feb18__close'])")).Click();
            //Thread.Sleep(400);
            new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(x => driver.FindElement(By.XPath("//*[@class='wCjUeog4KtWw64IplV1e6 _3dch7Ytt3ivpea7TIsKVjb _32MQA-gIaGxNU0uTG1yKum _2rJ9t7_3qwx1I1yZf7aTwh'][4]")).Enabled);
            //new Actions(driver).MoveToElement(driver.FindElement(By.XPath("//*[@class='wCjUeog4KtWw64IplV1e6 _3dch7Ytt3ivpea7TIsKVjb _32MQA-gIaGxNU0uTG1yKum _2rJ9t7_3qwx1I1yZf7aTwh'][4]"))).Build().Perform();
            //Thread.Sleep(400);
            driver.FindElement(By.XPath("//*[@class='wCjUeog4KtWw64IplV1e6 _3dch7Ytt3ivpea7TIsKVjb _32MQA-gIaGxNU0uTG1yKum _2rJ9t7_3qwx1I1yZf7aTwh'][4]")).Click();

            driver.FindElement(By.XPath("//span[text()='Цена']")).Click();
            //Thread.Sleep(400);

            driver.FindElement(By.XPath("//*[@class='text-field range__value range__value_left']")).Clear();
            driver.FindElement(By.XPath("//*[@class='text-field range__value range__value_left']")).SendKeys("1000");

            driver.FindElement(By.XPath("//*[@class='text-field range__value range__value_right']")).Clear();
            driver.FindElement(By.XPath("//*[@class='text-field range__value range__value_right']")).SendKeys("10000");
            driver.FindElement(By.XPath("//span[@class='button button_s button_blue multifilter-actions__apply']")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElements(By.CssSelector(".product-catalog-main.product-catalog-main_innactive")).Count > 0);

            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until(x => driver.FindElements(By.CssSelector(".product-catalog-main.product-catalog-main_innactive")).Count == 0);

            var webPrices = driver.FindElements(By.CssSelector(".price__actual"));
            if (driver.FindElements(By.CssSelector(".price__actual.parts__price_cd-disabled")).Any()) webPrices = driver.FindElements(By.CssSelector(".price__action.js-cd-discount"));

            int[] actualPrices = webPrices.Select(webPrice => Int32.Parse(Regex.Replace(webPrice.Text, @" ", ""))).ToArray();
            actualPrices.ToList().ForEach(price => Assert.IsTrue(price >= 1000 && price <= 10000));
        }

        [Test]
        public void TestAddToCartButtonTooltipText()
        {
            //var firstButtonAddToCart = driver.FindElement(By.XPath("//span[@class ='notranslate']"));

            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => new Actions(driver).MoveToElement(driver.FindElement(By.XPath("//span[@class ='notranslate']"))));
            new Actions(driver).MoveToElement(driver.FindElement(By.XPath("//span[@class ='notranslate']"))).Build().Perform();
            Assert.IsTrue(driver.FindElements(By.XPath(".//*[contains(@class,'_3BBdEIgjNVVM2vxYxWklU1')]")).Any(),
                "Tooltip on has not appeared");

            Assert.AreEqual(driver.FindElement(By.XPath(".//*[contains(@class,'_3BBdEIgjNVVM2vxYxWklU1')]")).Text.Trim(), "Информация о доставке будет отображаться для региона:",
                "Incorrect tooltip text");
        }

        [Test]
        public void NegativeTestPhoneNumberConfirmationWithEmptyPhoneNumber()
        {
            driver.FindElement(By.XPath("//a[@class = 'wCjUeog4KtWw64IplV1e6 _3A5-9K2JrODjfTiazRr7pk BLS-hOSrikRnPX76_f5Xr']")).Click();
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElement(By.XPath("//a[@class = 'wCjUeog4KtWw64IplV1e6 _3539glT5gkZd9IN6zZJk6G sqgUf472Rzh6p9Y-lmoKX']")));
            driver.FindElement(By.XPath("//a[@class = 'wCjUeog4KtWw64IplV1e6 _3539glT5gkZd9IN6zZJk6G sqgUf472Rzh6p9Y-lmoKX']")).Click();
            driver.FindElement(By.XPath("//input[@name = 'Подтверждение пароля']")).SendKeys("AutoTest");
            //driver.FindElement(By.XPath("//input[@name = 'Имя']")).SendKeys("AutoTest");
            driver.FindElement(By.XPath("//form[@class='_3dr8g_1eJXN-ElQNuG-g2a']//*[@name = 'Электронная почта']")).SendKeys("egorukr@mail.ru");
            driver.FindElement(By.XPath("//form[@class='_3dr8g_1eJXN-ElQNuG-g2a']//input[@type = 'password']")).SendKeys("AutoTest");
            Assert.IsTrue(!driver.FindElements(By.XPath("//button[@class = 'x-button x-button_accented x-button_40 x-button_intrinsic-width _27JVPsn4v4UPukX_BOxWUd'and not(@disabled)]")).Any(),
                "Error");
            //driver.FindElement(By.XPath("//button[@class = 'x-button x-button_accented x-button_40 x-button_intrinsic-width _27JVPsn4v4UPukX_BOxWUd']")).Click();
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}