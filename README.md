# Creating a simple REST API with .NET Core, Azure SQL and FusionCache

![License](https://img.shields.io/badge/license-MIT-green.svg)

A sample showcasing a REST API backed by an Azure SQL database and powered by [FusionCache](https://github.com/jodydonetti/ZiggyCreatures.FusionCache), to achieve top performance and robustness.

## 👩‍🏫 What is this about?

In your REST API, a simple piece of code like this:

```csharp
var id = 42;
var product = GetProductFromDb(id);
```

can be subject to unnecessary [database overload](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FactoryOptimization.md), temporary database failures, network congestions or anything else really.

By simply introducing FusionCache you can avoid most of these problems, by simply turning the line above into this:

```csharp
var id = 42;
var product = cache.GetOrSet<Product>(
    $"product:{id}",
    _ => GetProductFromDb(id)
);
```

You'll see how to use FusionCache main features, like the [fail-safe](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FailSafe.md) mechanism, the optional [2nd level](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheLevels.md), soft/hard [timeouts](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Timeouts.md) and [adaptive caching](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/AdaptiveCaching.md).

For more you can read the official FusionCache [docs](https://github.com/jodydonetti/ZiggyCreatures.FusionCache).

## 🧰 Prerequisites

You'll need an [Azure SQL](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal) database or compatible.

If you want to also use a 2nd level (distributed) cache, you'll also need a [Redis](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/quickstart-create-redis) instance somewhere.

FusionCache supports `.NET Standard 2.0`, so it runs on basically [any version](https://dotnet.microsoft.com/en-us/platform/dotnet-standard#versions) of .NET: this sample project though has been created for .NET 6, so be sure to have the [.NET 6.0](https://dotnet.microsoft.com/download) SDK (or newer) installed on your machine.

## 📦 Packages

The main Nuget packages used are:
- [Dapper](https://www.nuget.org/packages/Dapper/): used to talk to the database
- [Dapper.Contrib](https://www.nuget.org/packages/Dapper.Contrib/): to speed up handling of [CRUD](https://it.wikipedia.org/wiki/CRUD) operations
- [ZiggyCreatures.FusionCache](https://www.nuget.org/packages/ZiggyCreatures.FusionCache/): the main FusionCache package
- [Faker.NET](https://www.nuget.org/packages/Faker.Net/): used to create some fake data

If you want the optional 2nd level level, these packages are also used:
- [Microsoft.Extensions.Caching.StackExchangeRedis](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis/): the Redis implementation of a distributed cache (`IDistributedCache`)
- [ZiggyCreatures.FusionCache.Serialization.SystemTextJson](https://www.nuget.org/packages/ZiggyCreatures.FusionCache.Serialization.SystemTextJson/): to handle serialization to/from the distributed cache
- [ZiggyCreatures.FusionCache.Backplane.StackExchangeRedis](https://www.nuget.org/packages/ZiggyCreatures.FusionCache.Backplane.StackExchangeRedis/): the Redis implementation of the backplane

## ⭐ Quickstart

To get started:
- create an [Azure SQL](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal) database, you can even do it [for free](https://azure.microsoft.com/en-us/free/).
- execute the SQL script located at [/data/schema.sql](/data/schema.sql) which will generate the necessary tables. If you need any help in executing the SQL script, you can find a Quickstart here: [Quickstart: Use Azure Data Studio to connect and query Azure SQL database](https://docs.microsoft.com/en-us/sql/azure-data-studio/quickstart-sql-database).
- download or clone this repo on your local machine
- set the connection string to point to your Azure SQL database
- run the `WebApi1` project

This screen should pop up:

![foo](/docs/screenshot.png)

## 🔌 Connection strings

If you don't want to save your **Sql** connection string in the `appsettings.json` file for security reasons, you can just use a [secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) or set it using an environment variable:

Linux:

```bash
export ConnectionStrings__Sql="<your-connection-string>"
```

Windows (Powershell):

```powershell
$Env:ConnectionStrings__Sql="<your-connection-string>"
```

Your connection string is something like:

```text
SERVER=<your-server-name>.database.windows.net;DATABASE=<your-database-name>;UID=<your-username>;PWD=<your-password>
```

Just replace `<your-server-name>`, `<your-database-name>`, `<your-username>` and `<your-password>` with the correct values for your environment.

The same procedure can be used for the **Redis** connection string, if you would like to try to use the 2nd level or the backplane.

## 🚀 Ok, now what? (TODO)

[ Highlight of features used in the sample ]

## 📕 Learn more (TODO)

If you're new to .NET and want to learn more, there are a lot of tutorial available on the [Microsoft Learn](https://docs.microsoft.com/en-us/learn/browse/?products=dotnet) platform. You can start from here, for example:

- https://docs.microsoft.com/en-us/learn/modules/build-web-api-net-core/?view=aspnetcore-3.1

If you also want to learn more about Visual Studio Code, here's another resource:

[Using .NET Core in Visual Studio Code](https://code.visualstudio.com/docs/languages/dotnet)