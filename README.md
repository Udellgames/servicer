# Service Location Made Simple ![Servicer project build status](https://travis-ci.org/Udellgames/servicer.svg?branch=master) [![Servicer NuGet package version](https://img.shields.io/nuget/v/Servicer.svg)](https://www.nuget.org/packages/Servicer/)
A simple type-safe, generic service locator, now with promises and TPL support.

Now available as a [NuGet Package](https://www.nuget.org/packages/Servicer/)!

This repository is automatically built against the following versions of Mono:
  - beta
  - latest
  
The solution was written targeting .NET 3.5. It should work with any version higher than or equal to 3.5, but cannot currently be continuously tested against .NET. Task Parallel Library support is only available in .NET 4 or higher, and use of the `await` keyword only in .NET 4.5 or higher.
  
Any versions before Mono 3.8.0 are explicitly not compatible. Nightly / Alpha builds of Mono are not tested against, but should work.

# Usage

## Registering

Services must be registered with the Service Locator before they can be retrieved.

To register a new service, simply call `ServiceLocator.Register();`

If any promises or tasks have been created against that service, they will be immediately fulfilled or finished on registration. The *Explicit* setting applies to this operation.

**Note: You cannot register the same service twice without using different keys. See *Keyed, Unkeyed and Explicit* for more information.**

## Getting services

To get a registered service from the Service Locator simply call `ServiceLocator.GetService<T>();` where `T` is the type of service you need. If the service has not been registered, and `InvalidOperationException` will be thrown.

If you do not need the service immediately, and are happy for it to be fulfilled later, you can use `ServiceLocator.GetPromisedService<T>();`. This will return an `IPromise<T>` instance. Use the `Done` event to handle the promise. `Done` fires when the promise has been fulfilled, and your service is ready. You can get the value from the `PromiseFulfilledEventArgs` instance passed to the event handler.

Additionally you can call `.Require()` on your promise to give the promise an ultimatum, either it will return the fulfilled service, or it will throw an exception if the service is not ready yet. You might use this in situations when you would reasonably know when the service is ready, and it can be called synchronously in your code, rather than splitting your method into event handler callbacks.

If you are targeting .NET framework version 4 or higher, you can make use of the Task Parallelism Library. Simply call `ServiceLocator.GetServiceAsync`, which operates in the same way as the standard `ServiceLocator.GetService` methods, except it returns a Task. You can then execute code when a service is registered using `.ContinueWith`.

If you are targeting .NET framework version 4.5 or higher, you can use the convenient `await` keyword to await a service's availability. Use `ServiceLocator.GetServiceAsync` as before.

## Unregistering

If you need to remove a service (for example, to replace it with something else), simply call `ServiceLocator.Unregister();`

## Keyed, unkeyed and explicit

If you need to register more than one service of the same type, you can provide a key. The key can be any object, but be warned: the Service Locator will use the key's implementation of `Equals` and `GetHashCode` to find your service. **Overridden implementations could cause the Service Locator to fail.**

If you provide a key, you must use that key (or any key that matches the implementation of `Equals` on the key's type) when getting the service. It is perfectly acceptable to define a keyed service and an unkeyed service of the same type, and they will be treated as different entries in the Service Locator.

By default, if the Service Locator cannot find a service of the type you are requesting and with the same key, it will search for any services that can be assigned to your requested type. For example, calling `GetService<IEnumerable<string>>` will return a `List<string>` service casted to an `IEnumerable<string>` instance. When registering a service with `Explicit` set to false, any promises that can be fulfilled by a superclass of the service will be fulfilled. If you do not want this functionality, simply set `ServiceLocator.Explicit` to true. If the explicit service type cannot be found, a `KeyNotFoundException` will be thrown.

## Change log
* **v3.0**: Added support for the Task Parallel Library. GetServiceAsync returns a Task that can be awaited (.NET 4.5+) or consumed normally (.NET 4+).
* **v2.0**: Added promises support to the Service Locator. Promises allow you to request services that have not yet been registered, and act on those services when they are eventually registered.
* **v1.0**: Initial version. Services can be registered and requested, matched with keys and optionally polymorphically retrieved.


  
  
  
  
