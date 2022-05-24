# Creating a simple REST API with .NET Core, Azure SQL and FusionCache

![License](https://img.shields.io/badge/license-MIT-green.svg)

A sample showcasing a REST API backed by an Azure SQL database and powered by [FusionCache](https://github.com/jodydonetti/ZiggyCreatures.FusionCache), to achieve top performance and robustness.

## 👩‍🏫 What is this about?

Let's say you have a simple piece of code like this in your REST API:

```csharp
var id = 42;
var product = GetProductFromDb(id);
```

this can be subject to unnecessary [database overload](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FactoryOptimization.md), temporary database failures, slow calls due to network congestion or [anything else](https://en.wikipedia.org/wiki/Fallacies_of_distributed_computing) really.

By simply introducing FusionCache you can easily handle most of these problems, by simply turning the line above into this:

```csharp
var id = 42;
var product = cache.GetOrSet<Product>(
    $"product:{id}",
    _ => GetProductFromDb(id)
);
```

In this sample you'll see how to use FusionCache main features, like the [fail-safe](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FailSafe.md) mechanism, the optional [2nd level](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheLevels.md), soft/hard [timeouts](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Timeouts.md) and [adaptive caching](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/AdaptiveCaching.md).

For more you can read the official FusionCache [docs](https://github.com/jodydonetti/ZiggyCreatures.FusionCache).

Basically, this will be your journey:

![Two sample graphs showing the results of using FusionCache in your service](/docs/stepbystep-intro.png)

## 🧰 Prerequisites

You'll need an [Azure SQL](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal) database or compatible.

If you want to also use a 2nd level (distributed) cache, you'll also need a [Redis](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/quickstart-create-redis) instance somewhere.

FusionCache supports `.NET Standard 2.0`, so it runs on basically [any version](https://dotnet.microsoft.com/en-us/platform/dotnet-standard#versions) of .NET: this sample project though has been created for .NET 6, so be sure to have the [.NET 6.0](https://dotnet.microsoft.com/download) SDK (or newer) installed on your machine.

## ⭐ Quickstart

To get started:
- create an [Azure SQL](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal) database, you can even do it [for free](https://azure.microsoft.com/en-us/free/).
- execute the SQL script located at [/data/schema.sql](/data/schema.sql) which will generate the necessary tables ([need help?](https://docs.microsoft.com/en-us/sql/azure-data-studio/quickstart-sql-database))
- download or clone this repo on your local machine
- set the connection string to point to your Azure SQL database
- run the `WebApi1` project

Going at [https://localhost:5001](https://localhost:5001) you should see this screen:

![The initial Swagger page with the available endpoints](/docs/screenshot.png)

## 📦 Packages

The main Nuget packages used are:
- [Dapper](https://www.nuget.org/packages/Dapper/): used to talk to the database
- [Dapper.Contrib](https://www.nuget.org/packages/Dapper.Contrib/): to speed up handling of [CRUD](https://it.wikipedia.org/wiki/CRUD) operations
- [ZiggyCreatures.FusionCache](https://www.nuget.org/packages/ZiggyCreatures.FusionCache/): the main FusionCache package
- [Faker.NET](https://www.nuget.org/packages/Faker.Net/): used to create some fake data

When using the optional 2nd level, these packages are also used:
- [Microsoft.Extensions.Caching.StackExchangeRedis](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis/): the Redis implementation of a distributed cache (`IDistributedCache`)
- [ZiggyCreatures.FusionCache.Serialization.SystemTextJson](https://www.nuget.org/packages/ZiggyCreatures.FusionCache.Serialization.SystemTextJson/): to handle serialization to/from the distributed cache
- [ZiggyCreatures.FusionCache.Backplane.StackExchangeRedis](https://www.nuget.org/packages/ZiggyCreatures.FusionCache.Backplane.StackExchangeRedis/): the Redis implementation of the backplane

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

## 📕 Learn more

<img src="docs/fusioncache-logo.png" align="left" width="100" height="100" alt="FusionCache logo" />

FusionCache looks nice to you? There's a [gentle introduction](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/AGentleIntroduction.md) to get familiar with it.

You may then take a look at a couple of its main features, like [cache stampede prevention](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FactoryOptimization.md), the [fail-safe](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FailSafe.md) mechanism, the useful [soft/hard timeouts](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Timeouts.md) or the optional [2nd level](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheLevels.md).

On the official GitHub repo you can find all of this and more, including a detailed [step by step](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/StepByStep.md) (grab some coffee ☕) that highlights from start to finish how each feature can help you and what benefits you will achieve.

<br/>
<br/>

<img src="docs/dotnet-logo.png" align="right" width="100" height="100" alt=".NET logo" />

If you're new to .NET you may want start with a quick [intro video](https://www.youtube.com/watch?v=eIHKZfgddLM) by [Scott Hanselman](https://twitter.com/shanselman) and [Kendra Havens](https://twitter.com/gotheap).

There are also a lot of tutorials available on the [Microsoft Learn](https://docs.microsoft.com/en-us/learn/browse/?products=dotnet) platform, like an [Introduction to .NET](https://docs.microsoft.com/en-us/learn/modules/dotnet-introduction/), how to [Write your first C# code](https://docs.microsoft.com/en-us/learn/modules/csharp-write-first/) or what to do to [Create a web API with ASP.NET Core controllers](https://docs.microsoft.com/en-us/learn/modules/build-web-api-net-core/?view=aspnetcore-3.1).

<br/>
<br/>

<img src="docs/vscode-logo.png" align="left" width="100" height="100" alt="Visual Studio Code logo" />

If you want to learn more about using .NET with Visual Studio Code, you can start with [Using .NET Core in Visual Studio Code](https://code.visualstudio.com/docs/languages/dotnet). Sometimes problems happen, so it's nice to know how to [Interactively debug .NET apps with the Visual Studio Code debugger](https://docs.microsoft.com/en-us/learn/modules/dotnet-debug/). It's also easy to [Develop web applications with Visual Studio Code](https://docs.microsoft.com/en-us/learn/modules/develop-web-apps-with-vs-code/) and to use [GitHub in Visual Studio Code](https://docs.microsoft.com/en-us/learn/modules/introduction-to-github-visual-studio-code/).

<br/>
<br/>

<img src="docs/dapper-logo.jpg" align="right" width="100" height="100" alt="Dapper logo" />

Interested in [Dapper](https://github.com/DapperLib/Dapper)? Great.

Look no further with this nice [introduction](https://medium.com/dapper-net/get-started-with-dapper-net-591592c335aa) by Davide Mauri. Want more? He made an entire [series](https://medium.com/dapper-net/dapper-net-tutorial-summary-79125c8ecdb2), covering various topics like [multiple executions](https://medium.com/dapper-net/multiple-executions-56c410e9f8dd), [SQL Server specific features](https://medium.com/dapper-net/sql-server-specific-features-2773d894a6ae), [Custom Type Handling](https://medium.com/dapper-net/custom-type-handling-4b447b97c620) and more.