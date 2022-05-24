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

If you want to also use a 2nd level (distributed) cache and/or a backplane, you'll also need a [Redis](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/quickstart-create-redis) instance somewhere.

FusionCache supports `.NET Standard 2.0`, so it runs on basically [any version](https://dotnet.microsoft.com/en-us/platform/dotnet-standard#versions) of .NET: this sample project though has been created for .NET 6, so be sure to have the [.NET 6.0](https://dotnet.microsoft.com/download) SDK (or newer) installed on your machine.

## ⭐ Quickstart

To get started:
- create an [Azure SQL](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal) database, you can even do it [for free](https://azure.microsoft.com/en-us/free/).
- execute the SQL script located at [/data/schema.sql](/data/schema.sql) which will generate the necessary tables ([need help?](https://docs.microsoft.com/en-us/sql/azure-data-studio/quickstart-sql-database))
- download or clone this repo on your local machine
- set the connection string to point to your Azure SQL database
- if you want, also set the connection string to point to a Redis instance to have a [2nd level](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheLevels.md) cache and a [backplane](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Backplane.md)
- run the `WebApi1` project

Going at [https://localhost:5001](https://localhost:5001) you should see this screen:

![The initial Swagger page with the available endpoints](/docs/screenshot.png)

We then can use a tool like Postman or Fiddler (or directly the Swagger UI in the screenshot above) to hit the various endpoints.

First we may want to generate some data to play with: we can do this via the `/generate/{quantity}` endpoints for the various models we have.

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

## 🚀 Good, now what? (TO FINISH)

This sample showcases different ways to use FusionCache: let's take a look at each part.

### Initialization
In the `Program.cs` file (other project types may have a `Startup.cs` file, but it's basically the same) you'll find how the various parts are setup.

We read the 2 connection strings from the config:

```csharp
// CONNECTION STRINGS
var sqlConn = builder.Configuration.GetConnectionString("Sql");
var redisConn = builder.Configuration.GetConnectionString("Redis");
```

We then register a `DapperContext`, which is a small class to ease the creation of a conection to the database. It's not strictly needed but it can be a nice way to go:

```csharp
// ADD SERVICES: DAPPER CONTEXT
builder.Services.AddSingleton<DapperContext>(new DapperContext(sqlConn));
```

If the Redis connection string is there, we register the distributed cache (that is, the implementation of `IDistributedCache` for Redis) and the FusionCache backplane.

```csharp
// ADD SERVICES: REDIS
if (string.IsNullOrWhiteSpace(redisConn) == false)
{
    // ADD SERVICES: REDIS DISTRIBUTED CACHE
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConn;
    });

    // ADD SERVICES: REDIS BACKPLANE
    builder.Services.AddFusionCacheStackExchangeRedisBackplane(options =>
    {
        options.Configuration = redisConn;
    });
}
```

We then register FusionCache itself, along with the default entry options:

```csharp
// ADD SERVICES: FUSIONCACHE
builder.Services.AddFusionCache(options =>
{
    // SET DEFAULT OPTIONS
    options.DefaultEntryOptions = new FusionCacheEntryOptions()
        // DEFULT DURATION OF 5min
        .SetDuration(TimeSpan.FromMinutes(5))
        // ENABLE FAIL-SAFE (MAX DURATION OF 1h)
        .SetFailSafe(true, TimeSpan.FromHours(1))
        // ENABLE A FACTORY (SOFT) TIMEOUT OF 100ms
        .SetFactoryTimeouts(TimeSpan.FromMilliseconds(100));
});
```

As we can see we have opted for these defaults:
- a `Duration` of `5min`
- the [fail-safe](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FailSafe.md) mechanism enabled, and with a max duration of `1h`
- a [soft timeout](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Timeouts.md) of `100ms`

Keep in mind that these are just defaults: later on we can **override** anything we want in each single call, so think of this as just a baseline from where to start.

Finally there's the usual ASP.NET stuff: controllers, Swagger support and so on.

### Simple caching (Companies)

The `CompaniesController` provides access to, well, some sample companies: we should remember to generate some via the `/companies/generate/{quantity}` endpoint (like `/companies/generate/10` to generate 10 of them).

We can get a company from the database via our service by calling the `/companies/{id}` endpoint: as we can see the important point is this:

```csharp
var company = _cache.GetOrSet(
    $"company:{id}",
    _ => conn.Get<Company>(id)
);
```

What this does is asking FusionCache for the entry with the key `"company:{id}"` and also telling it how to go get the data if not there: FusionCache automatically handles multiple concurrent calls in an optimized way to avoid the [Cache Stampede](https://en.wikipedia.org/wiki/Cache_stampede) problem, and coordinate the dance between the caching layer(s), the database, timeouts and failures.

As we can see we haven't even specified a `Duration` or other options: this is because this is the "simple caching" scenario and we decided to rely on the **defaults** we set earlier.

In the `Insert` endpoint we create the new company in the database, and we can observe that we are not saving the data in the cache: we can do this or not, and the right approach will depend on the specific scenario. In general we should just know that, even without saving it, FusionCache will automatically load the needed data later on, if requested and not already in the cache.

For the `Update` endpoint instead we can see that we save the data in the cache, always: this is because we are updating an **existing** piece of data, and if may have been already saved in the cache before. By saving it we make sure it is overwritten with the last version, to avoid serving stale data.

```csharp
_cache.Set(
    $"company:{company.Id}",
    company
);
```

For the `Remove` action we also make sure to, well, remove the data from the cache: this will avoid serving data from the cache that does not exist anymore in the database.

```csharp
_cache.Remove(
    $"company:{id}"
);
```

### Adaptive caching (Products)

The `ProductsControllre` is almost the same as the `CompaniesController`, both conceptually and as the code itself. Than main difference is that here we decided to actively specify a different caching duration, and not just a fixed one but an **adaptive** one.

But what does adaptive caching mean?

[TO FINISH]


## 📕 Learn more

### FusionCache

<img src="docs/fusioncache-logo.png" align="left" width="100" height="100" alt="FusionCache logo" />

FusionCache looks nice to you? There's a [gentle introduction](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/AGentleIntroduction.md) to get familiar with it.

You may then take a look at a couple of its main features, like [cache stampede prevention](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FactoryOptimization.md), the [fail-safe](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FailSafe.md) mechanism, the useful [soft/hard timeouts](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Timeouts.md) or the optional [2nd level](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheLevels.md).

On the official GitHub repo you can find all of this and more, including a detailed [step by step](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/StepByStep.md) (grab some coffee ☕) that highlights from start to finish how each feature can help you and what benefits you will achieve.

<br/>

### .NET

<img src="docs/dotnet-logo.png" align="right" width="100" height="100" alt=".NET logo" />

If you're new to .NET you may want start with a quick [intro video](https://www.youtube.com/watch?v=eIHKZfgddLM) by [Scott Hanselman](https://twitter.com/shanselman) and [Kendra Havens](https://twitter.com/gotheap).

There are also a lot of tutorials available on the [Microsoft Learn](https://docs.microsoft.com/en-us/learn/browse/?products=dotnet) platform, like an [Introduction to .NET](https://docs.microsoft.com/en-us/learn/modules/dotnet-introduction/), how to [Write your first C# code](https://docs.microsoft.com/en-us/learn/modules/csharp-write-first/) or what to do to [Create a web API with ASP.NET Core controllers](https://docs.microsoft.com/en-us/learn/modules/build-web-api-net-core/?view=aspnetcore-3.1).

<br/>

### Visual Studio Code

<img src="docs/vscode-logo.png" align="left" width="100" height="100" alt="Visual Studio Code logo" />

If you want to learn more about using .NET with Visual Studio Code, you can start with [Using .NET Core in Visual Studio Code](https://code.visualstudio.com/docs/languages/dotnet). Sometimes problems happen, so it's nice to know how to [Interactively debug .NET apps with the Visual Studio Code debugger](https://docs.microsoft.com/en-us/learn/modules/dotnet-debug/). It's also easy to [Develop web applications with Visual Studio Code](https://docs.microsoft.com/en-us/learn/modules/develop-web-apps-with-vs-code/) and to use [GitHub in Visual Studio Code](https://docs.microsoft.com/en-us/learn/modules/introduction-to-github-visual-studio-code/).

<br/>

### Dapper

<img src="docs/dapper-logo.jpg" align="right" width="100" height="100" alt="Dapper logo" />

Interested in [Dapper](https://github.com/DapperLib/Dapper)? Great!

Look no further with this nice [introduction](https://medium.com/dapper-net/get-started-with-dapper-net-591592c335aa) by [Davide Mauri](https://twitter.com/mauridb). Want more? He made an entire [series](https://medium.com/dapper-net/dapper-net-tutorial-summary-79125c8ecdb2), covering various topics like [multiple executions](https://medium.com/dapper-net/multiple-executions-56c410e9f8dd), [SQL Server specific features](https://medium.com/dapper-net/sql-server-specific-features-2773d894a6ae), [Custom Type Handling](https://medium.com/dapper-net/custom-type-handling-4b447b97c620) and more.