# Web.Config AppSettings Helper

[![Build status](https://programystic.visualstudio.com/WebConfigHelper/_apis/build/status/WebConfigHelper-.NET%20Desktop-CI)](https://programystic.visualstudio.com/WebConfigHelper/_build/latest?definitionId=11)

[![NuGet package](https://img.shields.io/nuget/v/WebConfigHelper.svg)](https://nuget.org/packages/WebConfigHelper)

WebConfigHelper allows you to get strongly typed appsetting values from the web.config file.

## Getting a setting:
```cs
var config = new WebConfigValues();

var appVersion = config.GetAppSetting<int>("appVersion");
var releaseDate = config.GetAppSetting<DateTime>("releaseDate");
var appName = config.GetAppSetting<string>("appName");

// Return a comma separated list as an array
// <add key="versions" value="1, 9, 15, 23" />
// <add key="keyDates" value="01/02/2019, 01/03/2019, 01/04/2019" />
// <add key="names" value="Fred, Sarah, Sam" />
var versions = config.GetAppSettingArray<int>("versions");
var keyDates = config.GetAppSettingArray<DateTime>("keyDates");
var names = config.GetAppSettingArray<string>("names");

// with a default value
var appVersion = config.GetAppSetting("appVersion", 1);
var releaseDate = config.GetAppSettingArray<DateTime>("releaseDate", DateTime.Parse("01/01/2000");
var versions = config.GetAppSettingArray("versions", new int[] { 1, 2 });
var keyDates = config.GetAppSettingArray("keyDates", new int[] {  DateTime.Parse("01/01/2000"),  DateTime.Parse("01/01/2001") });
```

## Handling null values
```cs
// Either use a default value
var timeout = config.GetAppSetting("timeout", 30);

// or use a nullable type
var timeout = config.GetAppSetting<int?>("timeout");

// otherwise it will fail
var timeout = config.GetAppSetting<int>("timeout");
// System.ArgumentNullException : Setting 'timeout' returned null and type System.Int32 cannot have a null value
```

## Setting up a unit test (using Moq)
When you are creating unit tests for your web application, you can mock IWebConfigProvider allowing you to test different app settings values.

```cs
var provider = new Mock<IWebConfigProvider>();
// GetAppSetting always returns a string value
provider.Setup(x => x.GetAppSetting("appVersion")).Returns("1");
provider.Setup(x => x.GetAppSetting("versions")).Returns("1, 9, 15, 23");

var config = new WebConfigValues(provider.Object);
var appVersion = config.GetAppSetting<int>("appVersion");
var versions = config.GetAppSettingArray<int>("versions");
```