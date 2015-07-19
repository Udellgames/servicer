# Servicer ![Servicer project build status](https://travis-ci.org/Udellgames/servicer.svg?branch=master)
A simple type-safe, generic service locator.

This repository is automatically built against the following versions of Mono:
  - beta
  - latest
  - 3.10.0
  - 3.8.0
  
Additionally, the solution was written targeting .NET 3.5. It should work with any version higher than or equal to 3.5, but cannot currently be continuously tested against .NET.
  
Any versions before Mono 3.8.0 are not supported. Nightly / Alpha builds of Mono are not supported, but should work.

# Usage

## Registering

Services must be registered with the Service Locator before they can be retrieved.

To register a new service, simply call `ServiceLocator.Register();`

**Note: You cannot register the same service twice without using different keys. See *Keyed, Unkeyed and Explicit* for more information.**

## Getting Services

To get a registered service from the Service Locator simply call `ServiceLocator.GetService<T>();` where `T` is the type of service you need.

## Unregistering

If you need to remove a service (for example, to replace it with something else), simply call `ServiceLocator.Unregister();`

## Keyed, Unkeyed and Explicit

If you need to register more than one service of the same type, you can provide a key. The key can be any object, but be warned: the Service Locator will use the key's implementation of Equals and GetHashCode to find your service. **Overridden implementations could cause the Service Locator to fail.**

If you provide a key, you must use that key (or any key that matches the implementation of `Equals` on the key's type) when getting the service. It is perfectly acceptable to define a keyed service and an unkeyed service of the same type, and they will be treated as different entries in the Service Locator.

By default, if the Service Locator cannot find a service of the type you are requesting and with the same key, it will search for any services that can be assigned to your requested type. For example, calling `GetService<IEnumerable<string>>` will return a `List<string>` service casted to an `IEnumerable<string>` instance. If you do not want this functionality, simply set `ServiceLocator.Explicit` to true. If the explicit service type cannot be found, a `KeyNotFoundException` will be thrown.


  
  
  
  
