using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.Diagnostics;
using SeleniumWebdriver.ComponentHelper;
using SeleniumWebdriver.Settings;
using System.Threading;
using SeleniumExtras.PageObjects;
using System.Collections.ObjectModel;
using System.Globalization;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace PHPTRAVELS_MSTest
{
    [TestClass]
    public class HotelReserve
    {
        IWebDriver driver;

        private By hotelBtn_By = By.XPath("//*[@id='tab']/li[2]/button");//  //*[@id='tab']/li[1]/button");
        private By flyingFromDropDown_By = By.Id("select2-hotels_city-container");
        private By checkinDate_By = By.Id("checkin");
        private By checkoutDate_By = By.Id("checkout");
        static IWebElement datepickerElement;
        private By calender_today_By = By.CssSelector("td.day.active");

        private By travelsAndRoomDropDown_By = By.CssSelector("a.dropdown-toggle.dropdown-btn.travellers.d-flex.align-items-center.waves-effect");


        private By addRoom_By = By.ClassName("roomInc");
        private By hotels_rooms_input_By = By.Id("hotels_rooms");

        private By addAdults_By = By.XPath("//*[@id='hotels - search']/div/div[4]/div/div/div/div/div[2]/div/div/div[2]/svg");
        private By hotels_adults_input_By = By.Id("hotels_adults");

        private By addChilds_By = By.XPath("//*[@id='hotels - search']/div/div[4]/div/div/div/div/div[3]/div/div/div[2]/svg");
        private By hotels_childs_input_By = By.Id("hotels_childs");

        private By searchBtn_By = By.Id("//*[@id='hotels - search']/div/div[5]/button");
        ReadOnlyCollection<IWebElement> cityListInflyingFrom => driver.FindElement(By.ClassName(("select2-dropdown"))).FindElements(By.TagName("strong"));

        private IWebElement hotelBtn => driver.FindElement(hotelBtn_By);

        private IWebElement flyingFromDropDown => driver.FindElement(flyingFromDropDown_By);
        private IWebElement checkinDate => driver.FindElement(checkinDate_By);

        private IWebElement checkoutDate => driver.FindElement(checkoutDate_By);

        private IWebElement travelsAndRoomDropDown => driver.FindElement(travelsAndRoomDropDown_By);

        private IWebElement addRoom => driver.FindElement(addRoom_By);
        private IWebElement hotels_rooms_input => driver.FindElement(hotels_rooms_input_By);
        private IWebElement addAdults => driver.FindElement(addAdults_By);
        private IWebElement hotels_adults_input => driver.FindElement(hotels_adults_input_By);

        private IWebElement addChilds => driver.FindElement(addChilds_By);
        private IWebElement hotels_childs_input => driver.FindElement(hotels_childs_input_By);

        private IWebElement searchBtn => driver.FindElement(searchBtn_By);

         
        [TestMethod("جستجوی هتل با تاریخ، مقصد، اتاق و تعداد نفرات معتبر")]
        public void searchByAllValidFields()
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("no-sandbox");

                driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
                driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromMinutes(2));

                ObjectRepository.Driver = driver;
                NavigationHelper.NavigateToUrl(ObjectRepository.Config.GetWebsite());

                GenericHelper.WaitForWebElement(hotelBtn_By, TimeSpan.FromSeconds(60));
                hotelBtn.Click();

                set_Room_Adult_Child_In_DropDown(4, 5, 3);

                GenericHelper.WaitForWebElement(flyingFromDropDown_By, TimeSpan.FromSeconds(60));
                flyingFromDropDown.Click();

                if (cityListInflyingFrom.Count > 1)
                {
                    cityListInflyingFrom[1].Click();
                }
                else
                {
                    throw new Exception("لیست شهر مبدا خالی است");
                };

                //set 2 day after current date
                setCalender(CalendarType.CheckinDate, 2);
                //set 40 day after current date
                setCalender(CalendarType.CheckoutDate, 40);

                GenericHelper.WaitForWebElement(searchBtn_By, TimeSpan.FromSeconds(60));
                searchBtn.Click();

                //اگر لیستی از هتل ها را نشان بدهد
                bool showHotelResult = GenericHelper.IsElemetPresent(By.Id("hotels--list-targets"));
                //اگر لیستی از هتل ها را نشان ندهد
                bool notShowHotelResult = GenericHelper.IsElemetPresent(By.XPath("//img[contains(@alt,'no results')]"));

                //اگر هیچ فریمی مبتنی بر نمایش یا عدم نمایش هتل نمایش داده نشد
                if (!showHotelResult && !notShowHotelResult)
                {
                    throw new Exception("عملکرد دکمه جستجو صحیح نیست");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


        [TestMethod]
        public void disposeChromeDriver()
        {
            Process[] cromeDrivers = Process.GetProcessesByName("ChromeDriver");

            foreach (Process item in cromeDrivers)
            {
                item.Kill();
                item.WaitForExit();
                item.Dispose();
            }
        }

        public static string AddDaysToDate(string date, int addDays)
        {
            // Parse the input date string to a DateTime object using the dd-MM-yyyy format
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // Add the specified number of days to the date
            dateTime = dateTime.AddDays(addDays);

            // Format the resulting date as a string in the dd-MM-yyyy format
            string persianFormat = dateTime.ToString("dd-MM-yyyy");

            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(int.Parse(persianFormat.Split('-')[2]), int.Parse(persianFormat.Split('-')[1]), int.Parse(persianFormat.Split('-')[0]), pc);
            string result = dt.ToString(CultureInfo.InvariantCulture).Substring(0, 10);
            return result;
        }

        public enum CalendarType
        {
            CheckinDate,
            CheckoutDate
        }
        public void setCalender(CalendarType calendarType, int addDaysToCurrentDate)
        {
            string currentDate = default;
            if (calendarType == CalendarType.CheckinDate)
            {
                datepickerElement = driver.FindElement(By.XPath("//*[@id='fadein']/div[8]"));
                GenericHelper.WaitForWebElement(checkinDate_By, TimeSpan.FromSeconds(60));
                currentDate = checkinDate.GetAttribute("value");
                checkinDate.Click();
            }
            else if (calendarType == CalendarType.CheckoutDate)
            {
                datepickerElement = driver.FindElement(By.XPath("//*[@id='fadein']/div[9]"));
                GenericHelper.WaitForWebElement(checkoutDate_By, TimeSpan.FromSeconds(60));
                currentDate = checkoutDate.GetAttribute("value");
                checkoutDate.Click();
            }
            //day-month-year
            int currentYear = int.Parse(currentDate.Split('-')[2]);
            int currentMonth = int.Parse(currentDate.Split('-')[1]);
            int currentDay = int.Parse(currentDate.Split('-')[0]);

            var newDateTime = AddDaysToDate(currentDate, addDaysToCurrentDate).Replace("/", "-");
            int newYear = int.Parse(newDateTime.Split('-')[2]);
            int newMonth = int.Parse(newDateTime.Split('-')[0]);
            int newDay = int.Parse(newDateTime.Split('-')[1]);
            IWebElement calender_monthList = datepickerElement.FindElement(By.XPath("//th[contains(@colspan,'5')]"));
            // calender_monthList.Click();
            if (newYear > currentYear)
            {
                calender_monthList.Click();
                int diff = newYear - currentYear;
                while (diff > 0)
                {
                    IWebElement calender_next = datepickerElement.FindElement(By.XPath("//th[contains(@class,'next')]"));
                    calender_next.Click();
                    diff--;
                }

                IList<IWebElement> allMonths_Elements = datepickerElement.FindElements(By.XPath("//span[contains(@class,'month')]"));
                allMonths_Elements[newMonth - 1].Click();

                setDayInDateTimePicker(datepickerElement, newDay);

            }
            else if (newMonth > currentMonth)
            {
                calender_monthList.Click();
                IList<IWebElement> allMonths_Elements = datepickerElement.FindElement(By.XPath("//div[2]/table/tbody/tr/td")).FindElements(By.TagName("span"));
                allMonths_Elements[newMonth - 1].Click();

                setDayInDateTimePicker(datepickerElement, newDay);
            }
            else if (newDay > currentDay)
            {
                setDayInDateTimePicker(datepickerElement, newDay);
            }
        }


        public void setDayInDateTimePicker(IWebElement dateTimePicker, int day)
        {
            IList<IWebElement> allDays_Elements = datepickerElement.FindElement(By.XPath("//div[1]/table/tbody")).FindElements(By.TagName("td"));

            foreach (var tdDay in allDays_Elements)
            {
                var tdClass = tdDay.GetAttribute("class");
                var tdValue = tdDay.GetAttribute("textContent");

                if (tdClass != "day  old"
                    &&
                    tdClass != "day  new"
                    &&
                    tdValue == day.ToString())
                {
                    tdDay.Click();
                }
            }
        }

        public void set_Room_Adult_Child_In_DropDown(int roomCount, int adultCount, int childCount)
        {
            GenericHelper.WaitForWebElement(travelsAndRoomDropDown_By, TimeSpan.FromSeconds(60));
            travelsAndRoomDropDown.Click();

            //-----------------صحت سنجی افزودن اتاق------------//
            GenericHelper.WaitForWebElement(hotels_rooms_input_By, TimeSpan.FromSeconds(60));
            hotels_rooms_input.Clear();
            hotels_rooms_input.SendKeys(roomCount.ToString());

            for (int i = 0; i < roomCount; i++)
            {
                GenericHelper.WaitForWebElement(addRoom_By, TimeSpan.FromSeconds(60));
                addRoom.Click();
            }
            if (Convert.ToInt32(hotels_rooms_input.GetAttribute("value")) != roomCount * 2)
            {
                throw new Exception("عملکرد افزودن اتاق نادرست است");
            }

            //-----------------صحت سنجی افزودن افراد بزرگسال------------//
            GenericHelper.WaitForWebElement(hotels_adults_input_By, TimeSpan.FromSeconds(60));
            hotels_adults_input.Clear();
            hotels_adults_input.SendKeys(adultCount.ToString());

            for (int i = 0; i < adultCount; i++)
            {
                GenericHelper.WaitForWebElement(addAdults_By, TimeSpan.FromSeconds(60));
                addAdults.Click();
            }
            if (Convert.ToInt32(hotels_adults_input.GetAttribute("value")) != adultCount * 2)
            {
                throw new Exception("عملکرد افزودن افراد بالغ نادرست است");
            }

            //-----------------صحت سنجی افراد کودک------------//
            GenericHelper.WaitForWebElement(hotels_childs_input_By, TimeSpan.FromSeconds(60));
            hotels_childs_input.Clear();
            hotels_childs_input.SendKeys(childCount.ToString());

            for (int i = 0; i < childCount; i++)
            {
                GenericHelper.WaitForWebElement(addChilds_By, TimeSpan.FromSeconds(60));
                addChilds.Click();
            }
            if (Convert.ToInt32(hotels_childs_input.GetAttribute("value")) != childCount * 2)
            {
                throw new Exception("عملکرد افزودن کودکان نادرست است");
            }
        }
    }
}