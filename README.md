# 🧰 DIToolSystem – Custom Dependency Injection Container

A lightweight and flexible **Dependency Injection (DI) Container** built for .NET.  
Supports:

- ✔ Constructor injection
- ✔ Lazy<T> resolution
- ✔ Named registrations
- ✔ Assembly scanning
- ✔ Singleton & transient lifetimes
- ✔ Factory registrations
- ✔ ResolveAll for multi-binding
- ✔ Async resolving
- ✔ Circular dependency detection

---

## 📦 Installation

Simply include the container source code in your project.  
No external dependencies besides the .NET runtime.

```csharp
var container = new Container();

⚙️ API Overview
Below is the full IContainer API:
public interface IContainer
{
    IContainer RegisterType<TImplementation>() where TImplementation : class;
    IContainer Register<TImplementation, TInterface>()
        where TInterface : class
        where TImplementation : class, TInterface;

    IContainer Register<TService>(Func<IContainer, TService> factory)
        where TService : class;

    IContainer RegisterAssembly<TService>(Assembly assembly)
        where TService : class;

    IContainer SingleInstance();
    IContainer Named(string name);

    TService Resolve<TService>() where TService : class;
    TService ResolveNamed<TService>(string name) where TService : class;
    IEnumerable<TService> ResolveAll<TService>() where TService : class;
    Task<TService> ResolveAsync<TService>() where TService : class;
}
```

## 📝 Registration Methods

| Method                            | Description                                                                      | Example                                                        |
| --------------------------------- | -------------------------------------------------------------------------------- | -------------------------------------------------------------- |
| **RegisterType<T>()**             | Registers a concrete class to itself. Used when type = service.                  | `container.RegisterType<MyService>();`                         |
| **Register<TImpl, TInterface>()** | Registers an implementation to an interface.                                     | `container.Register<MyService, IService>();`                   |
| **Register(factory)**             | Registers using a factory delegate (advanced scenarios).                         | `container.Register<ILogger>(c => new FileLogger("log.txt"));` |
| **RegisterAssembly<TService>()**  | Scans an assembly and registers *all* classes implementing a specific interface. | `container.RegisterAssembly<ITool>(typeof(ITool).Assembly);`   |
| **SingleInstance()**              | Converts the *last registration* into a Singleton.                               | `container.Register<Logger, ILogger>().SingleInstance();`      |
| **Named(name)**                   | Assigns a name to the last registration. Enables named resolving.                | `container.RegisterType<ConsoleLogger>().Named("Logger");`     |

## 🔍 Resolution Methods

| Method                    | Description                                                    | Example                                                          |
| ------------------------- | -------------------------------------------------------------- | ---------------------------------------------------------------- |
| **Resolve<T>()**          | Resolves a single instance of the requested type.              | `var logger = container.Resolve<ILogger>();`                     |
| **ResolveNamed<T>(name)** | Resolves a named registration.                                 | `container.ResolveNamed<ILogger>("Logger");`                     |
| **ResolveAll<T>()**       | Returns all implementations registered for the same interface. | `foreach (var tool in container.ResolveAll<ITool>()) …`          |
| **ResolveAsync<T>()**     | Async wrapper around `Resolve()`.                              | `var logger = await container.ResolveAsync<ILogger>();`          |
| **Lazy<T> support**       | Container automatically creates Lazy<T> of any registered T.   | `Lazy<ILogger> lazyLogger = container.Resolve<Lazy<ILogger>>();` |

## 🧠 How Resolution Works
- Locate the registration (or throw if missing)
- Detect circular dependencies
- Create Lazy<T> delegates dynamically
- Choose the constructor with the most parameters
- Resolve all dependencies recursively
- Create the instance (singleton or transient)

## 📄 Usage Examples
This example demonstrates how to use the IContainer interface to register and resolve services in various ways. It covers:
- Registering concrete types and interface mappings
- Using factories for custom initialization
- Registering all implementations from an assembly
- Single-instance (singleton) and named registrations
- Resolving services synchronously and asynchronously
- Resolving all implementations of a given interface
- 

```csharp
  using System;
  using System.Reflection;
  using System.Threading.Tasks;
  using DIToolSystem.DICustomContainer;

namespace DIToolSystemExample
{
// Sample interfaces and implementations
public interface IService { void Execute(); }
public class ServiceA : IService { public void Execute() => Console.WriteLine("ServiceA working"); }
public class ServiceB : IService { public void Execute() => Console.WriteLine("ServiceB working"); }

    class Program
    {
        static async Task Main(string[] args)
        {
            IContainer container = new Container();

            // 1️⃣ Register a concrete type
            container.RegisterType<ServiceA>();
            var serviceA = container.Resolve<ServiceA>();
            serviceA.Execute();

            // 2️⃣ Register an implementation for an interface
            container.Register<ServiceB, IService>();
            var serviceB = container.Resolve<IService>();
            serviceB.Execute();

            // 3️⃣ Register a service with a factory
            container.Register<IService>(c => new ServiceA());
            var factoryService = container.Resolve<IService>();
            factoryService.Execute();

            // 4️⃣ Register all types from an assembly that implement a given service
            container.RegisterAssembly<IService>(Assembly.GetExecutingAssembly());
            foreach (var svc in container.ResolveAll<IService>())
            {
                svc.Execute();
            }

            // 5️⃣ Register a single instance (singleton)
            container.RegisterType<ServiceA>().SingleInstance();
            var singleton1 = container.Resolve<ServiceA>();
            var singleton2 = container.Resolve<ServiceA>();
            Console.WriteLine(ReferenceEquals(singleton1, singleton2)); // True

            // 6️⃣ Register a named instance
            container.Register<ServiceB, IService>().Named("SpecialService");
            var namedService = container.ResolveNamed<IService>("SpecialService");
            namedService.Execute();

            // 7️⃣ Resolve all registered implementations of a service
            var allServices = container.ResolveAll<IService>();
            foreach (var s in allServices)
                s.Execute();

            // 8️⃣ Resolve asynchronously
            var asyncService = await container.ResolveAsync<IService>();
            asyncService.Execute();
        }
    }
}
  ```