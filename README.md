# HyperIoC

## Getting Started

You can find this packages via NuGet: 

[**HyperIoC**](https://www.nuget.org/packages/HyperIoC)

## Overview

This package was originally created when Windows 8 first came out and IoC frameworks were not available. It was designed to be simple to use and cover the common cases 
such as transient and singleton registration. It also provided a clean, simple API to work with.

The availability of options such as the Dependency Injection API which is part of the .NET Standard, now render this redundant, but it is being maintained for historical 
and learning purposes.  

## The Factory Class

The hub of the framework is the _Factory_ class. This contains the registration and resolution API. You need to keep an instance of the _Factory_ alive for the lifetime of your application or application domain. 

Very simply, to register an a type:

```
var factory = new Factory();
factory.Add<IAccountService, AccountService>();
```

By default, all registration is transient meaning that each and every instance returned from the _Factory_ is a new instance.

To control this, the API provides two ways of configuring the _lifetime_ of a registered type. For _singletons_ there is a built-in extension:

```
var factory = new Factory();
factory.Add<IAccountService, AccountService>().AsSingleton();
```

Or alternatively you can implement the _ILifetimeManager_ interface or (preferably) inherit from the _LifetimeManager_ class, and attach your own _lifetime_ management:

```
var factory = new Factory();
factory.Add<IAccountService, AccountService>().SetLifetimeTo(new HttpLifetimeManager());
```

## The FactoryBuilder Class

A cleaner and more configurable way to construct your containers is to use the _FactoryBuilder_ class:

```
var factory = FactoryBuilder
    .Build()
    .WithProfile<MainProfile>()
    .Create();
```

Where the _MainProfile_ class is defined:

```
public class MainProfile : FactoryProfile
{
    public override void Construct(IFactoryBuilder builder)
    {
        builder.Add<IAccountService, AccountService>();
    }
}
```

Using the _FactoryBuilder_ class allows you to separate your deployment configuration by environment. The **.WithProfile()** accepts a 
predicate so you can, for example read a value from your config file and load only specific profiles:

```
var config = ConfigurationManager.AppSettings["Config"];

var factory = FactoryBuilder
    .Build()
    .WithProfile<DebugProfile>(() => config == "DEBUG")
    .WithProfile<LiveProfile>(() => config == "LIVE")
    .Create();

public class DebugProfile : FactoryProfile
{
    public override void Construct(IFactoryBuilder builder)
    {
        builder.Add<IAccountService, DebugAccountService>();
    }
}

public class LiveProfile : FactoryProfile
{
    public override void Construct(IFactoryBuilder builder)
    {
        builder.Add<IAccountService, AccountService>();
    }
}

```

The **Build** method on the _FactoryBuilder_ accepts a _Factory_ instance. This allows you to chain _factories_ together.

## Multiple interface instances

For scenarios where you have multiple instances of the same interface, the **Add** method on the _Factory_ class allows for a 
key to be provided. In fact all types are registered with an implicit key created but you can specify your own:

```
var factory = new Factory();
factory.Add<IAccountService, FirstAccountService>("First");
factory.Add<IAccountService, SecondAccountService>("Second");
```

## The Factory Get

Instances are resolved via the _Factory_ class **Get** method:

```
var service = factory.Get<IAccountService>();
```

You can also get a specific instance if you have specified a key during construction:

```
var service = factory.Get<IAccountService>("KEY");
```

## Self Registration

To aid factory support throughout your code, the _IFactoryResolver_ interface is automatically registered. Basically it's a bad 
idea to have references of the _IoC_ throughout your codebase. Typically the entry project will be the place where your _IoC_ is 
referenced and built, thus allowing easy switching of the _IoC_ or to control the construction in one place.

Typically for a factory, you would define your factory interface in a shared location in your codebase but implement it in your 
entry project where you have the _IoC_ reference:

```
var factory = new Factory();
factory.Add<IAccountService, AccountService>("Main");
factory.Add<IAccountService, OtherAccountService>("Other");
factory.Add<IAccountServiceFactory, AccountServiceFactory>();

var serviceFactory = factory.Get<IAccountServiceFactory>();
var service = serviceFactory.Create("Other");
```

Where the _IAccountServiceFactory_ implementation looks like:

```
public class AccountServiceFactory : IAccountServiceFactory
{
    private readonly IFactoryResolver _resolver;

    public AccountServiceFactory(IFactoryResolver resolver)
    {
        _resolver = resolver;
    }

    public IAccountService Create(string key)
    {
        return _resolver.Get<IAccountService>(key);
    }
}
```

## The Config Logger

On the _Factory_ class there is a method define called **Log**. Calling this will write out all the _IoC_ configuration to either a 
supplied _IConfigLogger_ implementation, or, by default, to the debug window:

```
var factory = new Factory();
factory.Add<IAccountService, AccountService>();
factory.Log();
```

The output will look something like:

```
Logging the registration...

Registered type: 'HyperIoC.IFactoryResolver' contains...
Type: 'HyperIoC.Factory' with key 'HyperIoC.Factory' as 'HyperIoC.Lifetime.SingletonLifetimeManager' lifetime
Registered type: 'HyperIoC.IFactoryResolver' complete.

Registered type: 'HyperIoCDemo.IAccountService' contains...
Type: 'HyperIoCDemo.AccountService' with key 'HyperIoCDemo.AccountService' as 'HyperIoC.Lifetime.TransientLifetimeManager' lifetime
Registered type: 'HyperIoCDemo.IAccountService' complete.

Registration log complete.
```

As you can see from the above window, the _IFactoryResolver_ is registered first and then each configured component. 
It will details the key (or default key), the full type name and the lifetime strategy being used.

You can provide your own logger by implementing the _IConfigLogger_ to write out the messages:

```
public class TestConfigLogger : IConfigLogger
{
    public void Log(string message)
    {
        // Log the message...
    }
}

var factory = new Factory();
factory.Add<IAccountService, AccountService>();
factory.Log(new TestConfigLogger());

```

## Setting up HyperIoC for MVC

**This is for reference and might be out-of-date** 

Rather than build a NuGet package for a few lines of code, and also as the MVC framework changes a little over time, below will 
detail how to go about configuring the _HyperIoC_ in MVC.

### The HTTP lifetime manager

The first place to start is to create a HTTP-based lifetime manager something like:

```
public class HttpContextLifetimeManager : LifetimeManager
{
    public override object Get(Type type, IFactoryLocator locator, IFactoryResolver resolver)
    {
        var context = HttpContext.Current;

        if (context == null)
            throw new InvalidOperationException("The HTTP context is not available.");

        lock (context.Items.SyncRoot)
        {
            var key = "HyperIoC-" + type.FullName;
            context.Items[key] = context.Items[key] ?? CreateInstance(type, locator, resolver);
            return context.Items[key];
        }
    }
}
```

The next step is to create a new controller factory that is constructed from the _Factory_ class:

```
public class HyperIoCControllerFactory : DefaultControllerFactory
{
    private readonly IFactoryResolver _resolver;

    public HyperIoCControllerFactory(IFactoryResolver resolver)
    {
        if (resolver == null) throw new ArgumentNullException(nameof(resolver));

        _resolver = resolver;
    }

    public override IController CreateController(RequestContext requestContext, string controllerName)
    {
        return _resolver.Get<IController>(controllerName + "Controller");
    }
}
```

**Note** The key in _HyperIoCControllerFactory_ class and the controller type in the _HttpContextLifetimeManager_ should match.

Create an extension to the _Item_ class:

```
public static class ItemExtensions
{
    public static void AsPerRequest(this Item item)
    {
        item.SetLifetimeTo(new HttpContextLifetimeManager());
    }
}
```

And finally an extension to enumerate assembly with all the controllers and automatically addd them:

```
public static class FactoryBuilderExtensions
{
    public static void AddControllers(this IFactoryBuilder builder, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(t => typeof(IController).IsAssignableFrom(t)))
        {
            builder.Add(typeof (IController), type, type.Name);
        }
    }
}
```

### Putting it together

In the startup of your site set:

```
var factory = FactoryBuilder
    .Build()
    .WithProfile<ControllerProfile>()
    .Create();

ControllerBuilder.Current.SetControllerFactory(new HyperIoCControllerFactory(factory));

```

And create a profile to register your controllers:

```
public class ControllerProfile : FactoryProfile
{
    public override void Construct(IFactoryBuilder builder)
    {
        builder.AddControllers(typeof(MvcApplication).Assembly);
    }
}
```

## Developer Notes

### Building and Publishing

From the root, to build, run:

```bash
dotnet build --configuration Release 
```

To run all the unit tests, run:

```bash
dotnet test --no-build --configuration Release
```

To create a package for the tool, run:
 
```bash
cd src/HyperIoC
dotnet pack --no-build --configuration Release 
```

To publish the package to the nuget feed on nuget.org:

```bash
dotnet nuget push ./bin/Release/HyperIoC.3.0.0.nupkg -k [THE API KEY] -s https://api.nuget.org/v3/index.json
```

## Links

* **GitFlow** https://datasift.github.io/gitflow/IntroducingGitFlow.html
