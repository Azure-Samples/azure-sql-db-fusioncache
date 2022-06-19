---
page_type: sample
languages:
  - csharp
  - tsql
  - sql
products:
  - azure
  - vs-code
  - azure-sql-database
  - azure-web-apps
  - azure-sql-database
  - azure-cache-redis
  - dotnet
  - dotnet-core
  - azure-app-service-web
description: 'Creating a simple REST API with .NET Core, Azure SQL Database and FusionCache'
---

# Creating a simple REST API with .NET Core, Azure SQL Database and FusionCache

![License](https://img.shields.io/badge/license-MIT-green.svg)

A little sample showcasing a REST API backed by an Azure SQL database and powered by [FusionCache](https://github.com/jodydonetti/ZiggyCreatures.FusionCache), to achieve top performance and robustness.

## 👩‍🏫 What is this about?

Let's say we have a simple piece of code like this in our REST API:

```csharp
var id = 42;
var product = GetProductFromDb(id);
```

this can be subject to unnecessary [database overload]([https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FactoryOptimization.md](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheStampede.md)), temporary database failures, slow calls due to a temporary network congestion or [anything else](https://en.wikipedia.org/wiki/Fallacies_of_distributed_computing) really.

By simply introducing FusionCache we can easily handle most of these problems. Just turn the line above into this:

```csharp
var id = 42;
var product = cache.GetOrSet<Product>(
    $"product:{id}",
    _ => GetProductFromDb(id)
);
```

In this sample we'll see how to use FusionCache main features, like the [fail-safe](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FailSafe.md) mechanism, the optional [2nd level](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheLevels.md), soft/hard [timeouts](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Timeouts.md) and [adaptive caching](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/AdaptiveCaching.md).

For more we can read the official FusionCache [docs](https://github.com/jodydonetti/ZiggyCreatures.FusionCache).

Basically, this is the ideal result:

![Two sample graphs showing the results of using FusionCache in our service](/docs/stepbystep-intro.png)

## 🧰 Prerequisites

We'll need an [Azure SQL](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal) database or compatible.

If we want to also use a 2nd level (distributed) cache and/or a backplane, we'll also need a [Redis](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/quickstart-create-redis) instance somewhere.

FusionCache targets `.NET Standard 2.0`, so it runs on basically [any version](https://dotnet.microsoft.com/en-us/platform/dotnet-standard#versions) of .NET: this sample project though has been created for .NET 6, so be sure to have the [.NET 6.0](https://dotnet.microsoft.com/download) SDK (or newer) installed on our machine.

## ⭐ Quickstart

To get started:
- create an [Azure SQL](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal) database, we can even do it [for free](https://azure.microsoft.com/en-us/free/).
- execute the SQL script located at [/data/schema.sql](/data/schema.sql) which will generate the necessary tables ([need help?](https://docs.microsoft.com/en-us/sql/azure-data-studio/quickstart-sql-database))
- download or clone this repo on our local machine
- set the connection string to point to our Azure SQL database
- if we want, we can also set the connection string to point to a Redis instance to have a [2nd level](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/CacheLevels.md) cache and a [backplane](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Backplane.md)
- run the `WebApi1` project

Going at [https://localhost:5001](https://localhost:5001) we should see this screen:

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

If we don't want to save our **Sql** connection string in the `appsettings.json` file for security reasons, we can just use a [secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) or set it using an environment variable:

Linux:

```bash
export ConnectionStrings__Sql="<our-connection-string>"
```

Windows (Powershell):

```powershell
$Env:ConnectionStrings__Sql="<our-connection-string>"
```

Our connection string should be something like:

```text
SERVER=<our-server-name>.database.windows.net;DATABASE=<our-database-name>;UID=<our-username>;PWD=<our-password>
```

Just replace `<our-server-name>`, `<our-database-name>`, `<our-username>` and `<our-password>` with the correct values for our environment.

The same procedure can be used for the **Redis** connection string, if we would like to try to use the 2nd level or the backplane.

## 🚀 Good, now what?

This sample showcases different ways to use FusionCache: let's take a look at each part.

### Initialization
In the `Program.cs` file (other project types may have a `Startup.cs` file, but it's basically the same) we'll find how the various parts are setup.

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
- the [fail-safe](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/FailSafe.md) mechanism enabled, and with a max duration (`FailSafeMaxDUration`) of `1h`
- a [soft timeout](https://github.com/jodydonetti/ZiggyCreatures.FusionCache/blob/main/docs/Timeouts.md) (`FactorySoftTimeout`)of `100ms`

Keep in mind that these are just defaults: later on we can **override** anything we want in each single call, so think of this as just a baseline from where to start.

Finally there's the usual ASP.NET stuff: controllers, Swagger support and so on.

### Simple caching

The `CompaniesController` provides access to, well, some sample companies: we should remember to generate some via the `/companies/generate/{quantity}` endpoint (like `/companies/generate/10` to generate 10 of them).

We can get a company from the database via our service by calling the `/companies/{id}` endpoint: as we can see the important point is this:

```csharp
var company = _cache.GetOrSet(
    $"company:{id}",
    _ => conn.Get<Company>(id)
);
```

What this does is asking FusionCache for the entry with the key `"company:{id}"` and also telling it how to go get the data if not there: FusionCache automatically handles multiple concurrent calls in an optimized way to avoid the [Cache Stampede](https://en.wikipedia.org/wiki/Cache_stampede) problem, and coordinates the dance between the caching layer(s), the database, timeouts and failures.

As we can see we haven't even specified a `Duration` or other options: this is because this is the "simple caching" scenario and we decided to rely on the **defaults** we set earlier.

In the `Insert` endpoint we create the new company in the database, and we can observe that we are not saving the data in the cache: we can do this or not, and the right approach will depend on the specific scenario. In general we should just know that, even without saving it, FusionCache will automatically load the needed data later on, if requested and not already in the cache.

For the `Update` endpoint instead we can see that we save the data in the cache, always: this is because we are updating an **existing** piece of data, and if may have been already saved in the cache before. By saving it we make sure it is overwritten with the last version, to avoid serving stale data.

```csharp
_cache.Set(
    $"company:{company.Id}",
    company
);
```

It is worth pointing out that another perfectly valid approach would be not to update the entry in the cache, but to just remove it: when the next request comes in for that specific company, FusionCache will simply go to the database in an optimized way and grab the last version.

The differences between the two approaches are:

- removing the cache entry will save some memory in case that piece of data is not requested anymore, but if instead it will be requested again it may takes more time to go grab the data from the database. Also, in case of temporary failures of the database, there would not be a stale/fallback entry to use (via fail-safe)
- updating the cache will consume some more memory, but the data will be available immediately for consumption when requested and there will also be a stale/fallback entry to use in case of problems (again, thanks to fail-safe)

As you can see there is not a "right approach" in and on itself, it always depends on context.

For the `Remove` action we also make sure to, well, remove the data from the cache: this will avoid serving data from the cache that does not exist anymore in the database.

```csharp
_cache.Remove(
    $"company:{id}"
);
```

### Adaptive caching

The `ProductsController` is almost the same as the `CompaniesController`, both conceptually and as the code itself. Than main difference is that here we decided to actively specify a different caching `Duration`, and not just a fixed one but an **adaptive** one.

But what does **adaptive caching** mean?

It means that we can have different entry options (like `Duration`, `Priority`, etc...) for each single piece of data, based on some fields of the data itself.

To not have code scattered around, we created a method on the `Product` class like this:

```csharp
public TimeSpan GetCacheDuration() {
    // LOGIC HERE
}
```

that we can call like this:

```csharp
var duration = product.GetCacheDuration();
```

Now, if we already have the data to cache and simply need to do a `cache.Set("key", data)` that is not hard to do, right?

It should be something simple like this:

```csharp
_cache.Set(
    $"product:{product.Id}",
    product,
    product.GetCacheDuration()
);
```

But how can we to do it when we use the `cache.GetOrSet("key", _ => ...)` method? In that case we do not have the data upfront, so we cannot change the options based on that: the only moment when we'll have the data is **during** the factory execution (that is, **inside** the part where we load the data from the database), so how can we do that?

Easy, we simply user a different `GetOrSet` overload, one with an additional *context* object, with which we can **adapt** the entry options based on our own logic.

Here's the related code:

```csharp
var product = _cache.GetOrSet(
    $"product:{id}",
    (ctx, _) =>
    {
        var x = conn.Get<Product>(id);

        // ADAPT THE ENTRY OPTIONS HERE
        ctx.Options.Duration = x.GetCacheDuration();

        return x;
    }
);
```

In this particular case we let the `Duration` be lower for data updated recently, whereas for data that has not been touched for some time we can suppose it will not receive updates anymore and so we are ok with caching it for some more time. We will basically avoid going to the database every `30sec` for something that has been touched last time, say, a year ago. Makes sense?

## 📕 Learn more

If you are interested in learning more take a look here.

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

Interested in [Dapper](https://github.com/DapperLib/Dapper)? Great, look no further with this nice [introduction](https://medium.com/dapper-net/get-started-with-dapper-net-591592c335aa) by [Davide Mauri](https://twitter.com/mauridb).

Want more? He made an entire [series](https://medium.com/dapper-net/dapper-net-tutorial-summary-79125c8ecdb2), covering various topics like [multiple executions](https://medium.com/dapper-net/multiple-executions-56c410e9f8dd), [SQL Server specific features](https://medium.com/dapper-net/sql-server-specific-features-2773d894a6ae), [Custom Type Handling](https://medium.com/dapper-net/custom-type-handling-4b447b97c620) and more.
