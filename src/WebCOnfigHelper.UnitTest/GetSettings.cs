using Moq;
using NUnit.Framework;
using System;
using System.Globalization;

[assembly: CLSCompliant(true)]
namespace WebConfigHelper.UnitTest
{
    public static class GetSettings
    {
        private const string ValueOkKey = "valueOkKey";
        private const string ValueNotOkKey = "valueNotOkKey";

        [Test]
        public static void GetSettingInt()
        {
            TestSetting(1234, "not an int");
            TestSetting<int, OverflowException>(123456789, "12345678901234567890");
        }

        [Test]
        public static void GetSettingDecimal()
        {
            TestSetting(1.44m, "1.44", "not a decimal");
        }

        [Test]
        public static void GetSettingBool()
        {
            TestSetting(true, "true", "not a bool");
            TestSetting(true, "True", "not a bool");
            TestSetting(false, "false", "not a bool");
            TestSetting(false, "False", "not a bool");
        }

        [Test]
        public static void GetSettingDateTime()
        {
            TestSetting(new DateTime(2000, 12, 31, 12, 12, 12), "31/12/2000 12:12:12", "not a date time");
            TestSetting(new DateTime(2000, 12, 31), "31/12/2000 00:00:00", "not a date time");
        }

        [Test]
        public static void GetSettingSByte()
        {
            TestSetting<sbyte>(127, "not an sbyte");
            TestSetting<sbyte>(-128, "not an sbyte");
            TestSetting<sbyte, OverflowException>(55, "129");
            TestSetting<sbyte, OverflowException>(55, "-129");
        }

        [Test]
        public static void GetSettingArrayInt()
        {
            var expectedValue = new int[] { 1, 2, 3, 4 };
            var helper = GetWebConfigValues("1, 2, 3, 4", "this is not an array");

            var valueOk = helper.GetAppSettingArray<int>(ValueOkKey);
            var defaultValue = helper.GetAppSettingArray(ValueOkKey, expectedValue);

            Assert.AreEqual(expectedValue, valueOk);
            Assert.Throws<FormatException>(() => helper.GetAppSetting<int>(ValueNotOkKey));
            Assert.AreEqual(expectedValue, defaultValue);
        }

        [Test]
        public static void GetSettingArrayDateTime()
        {
            var expectedValue = new DateTime[]
            {
                DateTime.Parse("01/01/2000", CultureInfo.InvariantCulture),
                DateTime.Parse("01/01/2001", CultureInfo.InvariantCulture),
                DateTime.Parse("01/01/2002", CultureInfo.InvariantCulture)
            };
            var helper = GetWebConfigValues("01/01/2000, 01/01/2001, 01/01/2002", "this is not an array");

            var valueOk = helper.GetAppSettingArray<DateTime>(ValueOkKey);
            var defaultValue = helper.GetAppSettingArray(ValueOkKey, expectedValue);

            Assert.AreEqual(expectedValue, valueOk);
            Assert.Throws<FormatException>(() => helper.GetAppSetting<DateTime>(ValueNotOkKey));
            Assert.AreEqual(expectedValue, defaultValue);
        }

        [Test]
        public static void GetSettingArrayBoolean()
        {
            var expectedValue = new bool[] { true, false, true, false };
            var helper = GetWebConfigValues("true, false, True, False", "this is not an array");

            var valueOk = helper.GetAppSettingArray<bool>(ValueOkKey);
            var defaultValue = helper.GetAppSettingArray(ValueOkKey, expectedValue);

            Assert.AreEqual(expectedValue, valueOk);
            Assert.Throws<FormatException>(() => helper.GetAppSetting<bool>(ValueNotOkKey));
            Assert.AreEqual(expectedValue, defaultValue);
        }

        private static void TestSetting<T>(T expectedValue, string unexpectedValue)
        {
            TestSetting<T, FormatException>(expectedValue, expectedValue.ToString(), unexpectedValue);
        }

        private static void TestSetting<T, E>(T expectedValue, string unexpectedValue) where E : Exception
        {
            TestSetting<T, E>(expectedValue, expectedValue.ToString(), unexpectedValue);
        }

        private static void TestSetting<T>(T expectedValue, string settingValue, string unexpectedValue)
        {
            TestSetting<T, FormatException>(expectedValue, settingValue, unexpectedValue);
        }

        private static void TestSetting<T, E>(T expectedValue, string settingValue, string unexpectedValue) where E : Exception
        {
            var helper = GetWebConfigValues(settingValue, unexpectedValue);

            var valueOk = helper.GetAppSetting<T>(ValueOkKey);
            var defaultValue = helper.GetAppSetting(ValueNotOkKey, expectedValue);

            Assert.AreEqual(expectedValue, valueOk);
            Assert.Throws<E>(() => helper.GetAppSetting<T>(ValueNotOkKey));
            Assert.AreEqual(expectedValue, defaultValue);
        }

        private static WebConfigValues GetWebConfigValues(string valueOk, string valueNotOk)
        {
            var provider = GetProvider(valueOk, valueNotOk);
            var helper = new WebConfigValues(provider);

            return helper;
        }

        private static IWebConfigProvider GetProvider(string valueOk, string valueNotOk)
        {
            var provider = new Mock<IWebConfigProvider>();
            provider.Setup(x => x.GetAppSetting(ValueOkKey)).Returns(valueOk);
            provider.Setup(x => x.GetAppSetting(ValueNotOkKey)).Returns(valueNotOk);

            return provider.Object;
        }
    }
}