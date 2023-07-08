using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OpenQA.Selenium;
using SeleniumWebdriver.Settings;

namespace SeleniumWebdriver.ComponentHelper
{
    public class NavigationHelper
    {
        private static readonly ILog Logger = Log4NetHelper.GetXmlLogger(typeof (NavigationHelper));

        public static void NavigateToUrl(string Url)
        {
            ObjectRepository.Driver.Navigate().GoToUrl(Url);
            Logger.Info(" Navigate To Page " + Url);
            if (Url.StartsWith("https:"))
                handleSSLError();
        }

        private static void handleSSLError() {
            try
            {
                GenericHelper.WaitForWebElement(By.Id("details-button"), TimeSpan.FromSeconds(5));
                IWebElement Advanced = ObjectRepository.Driver.FindElement(By.Id("details-button"));
                Advanced.Click();

                GenericHelper.WaitForWebElement(By.Id("proceed-link"), TimeSpan.FromSeconds(60));
                IWebElement unsafeBtn = ObjectRepository.Driver.FindElement(By.Id("proceed-link"));
                unsafeBtn.Click();
            }
            catch {

            }
        }
    }
}
