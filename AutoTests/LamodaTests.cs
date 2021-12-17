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

            new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(x => driver.FindElement(By.XPath("//*[contains(text(), 'Обувь')]")));

            driver.FindElement(By.XPath("//*[contains(text(), 'Обувь')]")).Click();

            driver.FindElement(By.XPath("//span[text()='Цена']")).Click();

            driver.FindElement(By.CssSelector(".text-field.range__value.range__value_left")).Clear();
            driver.FindElement(By.CssSelector(".text-field.range__value.range__value_left")).SendKeys("1000");

            driver.FindElement(By.CssSelector(".text-field.range__value.range__value_right")).Clear();
            driver.FindElement(By.CssSelector(".text-field.range__value.range__value_right")).SendKeys("10000");
            var x = driver.FindElement(By.CssSelector(".multifilter.multifilter_price"));
            x.FindElement(By.CssSelector(".button.button_s.button_blue.multifilter-actions__apply")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElements(By.CssSelector(".product-catalog-main.product-catalog-main_innactive")).Count > 0);

            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until(x => driver.FindElements(By.CssSelector(".product-catalog-main.product-catalog-main_innactive")).Count == 0);

            var webPrices = driver.FindElements(By.CssSelector(".price__actual"));
            if (driver.FindElements(By.CssSelector(".price__actual.parts__price_cd-disabled")).Any()) webPrices = driver.FindElements(By.CssSelector(".price__action.js-cd-discount"));

            int[] actualPrices = webPrices.Select(webPrice => Int32.Parse(Regex.Replace(webPrice.Text, @" ", ""))).ToArray();
            actualPrices.ToList().ForEach(price => Assert.IsTrue(price >= 1000 && price <= 10000, "Some element did not pass into the filter"));
        }

        [Test]
        public void TestAddToCartButtonTooltipText()
        {
            driver.FindElement(By.XPath("(//a[@data-genders='men'])")).Click();
            driver.FindElement(By.XPath("(//div[@class='sub-popup-feb18__close'])")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(x => driver.FindElement(By.XPath("//*[contains(text(), 'Обувь')]")));

            driver.FindElement(By.XPath("//*[contains(text(), 'Обувь')]")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector(".products-list-item.dyother.dyMonitor"))));
            var elem = driver.FindElement(By.CssSelector(".products-list-item.dyother.dyMonitor"));
            new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector(".products-list-item.dyother.dyMonitor"))).Build().Perform();

            Assert.IsTrue(elem.FindElements(By.CssSelector(".zoomin.products-list-item__qv")).Any(),
                "Tooltip on has not appeared");

            Assert.AreEqual(driver.FindElement(By.CssSelector(".zoomin.products-list-item__qv")).Text.Trim(), "Подробнее",
                "Incorrect tooltip text");
        }

        [Test]
        public void NegativeTestPhoneNumberConfirmationWithEmptyPhoneNumber()
        {
            driver.FindElement(By.XPath("//a[contains(text(), 'Войти')]")).Click();
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElement(By.XPath("//a[contains(text(), 'Создать аккаунт')]")));
            driver.FindElement(By.XPath("//a[contains(text(), 'Создать аккаунт')]")).Click();
            driver.FindElement(By.XPath("//input[@name = 'Подтверждение пароля']")).SendKeys("AutoTest");
            driver.FindElement(By.XPath("//div[@name='subscribe']/..//*[@name = 'Электронная почта']")).SendKeys("egorukr@mail.ru");
            driver.FindElement(By.XPath("//div[@name='subscribe']/..//input[@type = 'password']")).SendKeys("AutoTest");
            Assert.IsTrue(!driver.FindElements(By.XPath("//a[contains(text(), 'Зарегестрироваться') and not(@disabled)]")).Any(),
                "The register button is available if the fields are empty");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}