using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Remote7zReaderWeb.Test
{
    [TestClass]
    public class UnitTest_Selenium
    {
        [TestMethod]
        public void TestMethod_IE_Opening()
        {
            using (IWebDriver webdriver = new OpenQA.Selenium.IE.InternetExplorerDriver(@"drivers"))
            {
                webdriver.Navigate().GoToUrl("about:blank");
                Assert.AreEqual("about:blank", webdriver.Url);
                webdriver.Quit();
            }
        }
    }
}
