using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Simple service locator
/// </summary>
public sealed class ServiceLocator
{
    /// <summary>
    /// Simple .NET 3.5 Tuple class.
    /// </summary>
    /// <typeparam name="T">The type of the first item.</typeparam>
    /// <typeparam name="V">The type of the second item.</typeparam>
    [DebuggerDisplay("Item1: {Item1}, Item2: {Item2}")]
    private class Tuple<T, V>
    {
        public T Item1 { get; set; }

        public V Item2 { get; set; }

        public Tuple(T item1, V item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var objAsTuple = obj as Tuple<T, V>;
            return objAsTuple != null && IsEqualTo(objAsTuple.Item1, Item1) && IsEqualTo(objAsTuple.Item2, Item2);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var item1HashCode = Item1 == null ? 0 : Item1.GetHashCode();
                var item2HashCode = Item2 == null ? 0 : Item2.GetHashCode();
                return item1HashCode + item2HashCode;
            }
        }

        /// <summary>
        /// Determines whether the first object is equal to the second object, including if they are both null.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>True if both first and second are null or first is equal to second.</returns>
        private bool IsEqualTo(object first, object second)
        {
            return first == null ? second == null : first.Equals(second);
        }
    }

    /// <summary>
    /// The instance for the Singleton pattern.
    /// </summary>
    /// <remarks>
    /// This isn't lazy, but it is thread-safe. Laziness is not necessary as this class is very light-weight initially.
    /// </remarks>
    private static readonly ServiceLocator instance = new ServiceLocator();

    /// <summary>
    /// The services
    /// </summary>
    private IDictionary<Tuple<Type, object>, object> services = new Dictionary<Tuple<Type, object>, object>();

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ServiceLocator"/> is explicit.
    /// </summary>
    /// <remarks>
    /// This property controls how the ServiceLocator handles not being able to find a service of the requested type.
    /// If explicit is set to false, the ServiceLocator will look for services that can be casted to the requested type.
    /// If explicit is set to true, the ServiceLocator while throw an exception if it cannot find an exact match to the requested service.
    /// </remarks>
    /// <value>
    ///   <c>true</c> if explicit; otherwise, <c>false</c>.
    /// </value>
    public static bool Explicit { get; set; }

    private static ServiceLocator Instance
    {
        get
        {
            return instance;
        }
    }

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static ServiceLocator()
    {
    }

    private ServiceLocator()
    {
    }

    /// <summary>
    /// Clears this instance by removing all services.
    /// </summary>
    public static void Clear()
    {
        ServiceLocator.Instance.services.Clear();
    }

    /// <summary>
    /// Gets a service of type T.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>The instance of type T registered with the Service Locator.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the Service Locator cannot find an un-keyed service of the requested type and Explicit is true, or when the Service Locator cannot find an un-keyed service of the requested type or any type that can be assigned to the requested type and Explicit is false.</exception>
    public static T GetService<T>() where T : class
    {
        return GetService<T>(null);
    }

    /// <summary>
    /// Gets the service using a key object.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <param name="key">The key to filter multiple services of the same type by.</param>
    /// <returns>The instance of type T registered with the provided key in the Service Locator.</returns>
    /// <exception cref="System.KeyNotFoundException">Thrown when the Service Locator cannot find an a service of the requested type with a matching key and Explicit is true, or when the Service Locator cannot find a service of the requested type or any type that can be assigned to the requested type with the matching key and Explicit is false.</exception>
    public static T GetService<T>(object key) where T : class
    {
        var type = typeof(T);
        var dictKey = new Tuple<Type, object>(type, key);

        object returnValue;

        if (Instance.services.TryGetValue(dictKey, out returnValue))
        {
            return (T)returnValue;
        }

        if (!Explicit)
        {
            var subTypeKey = Instance.services.Keys.FirstOrDefault(
                x =>
                    (key == null ? x.Item2 == null : key.Equals(x.Item2)) &&
                    type.IsAssignableFrom(x.Item1));

            if (subTypeKey != null)
            {
                return (T)Instance.services[subTypeKey];
            }
        }

        throw new KeyNotFoundException(string.Format("Cannot get a value for type: {0} and key: {1}. That type has not been registered yet.", type, key));
    }

    /// <summary>
    /// Registers the specified service with no key.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <param name="service">The service.</param>
    /// <exception cref="InvalidOperationException">>Thrown when a service of this type has already been registered without a key.</exception>
    public static void Register<T>(T service) where T : class
    {
        Register(service, null);
    }

    /// <summary>
    /// Registers the specified service with a key.
    /// </summary>
    /// <typeparam name="T">The type of service.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="key">The key.</param>
    /// <exception cref="System.InvalidOperationException">Thrown when a service of this type has already been registered with this key.</exception>
    public static void Register<T>(T service, object key) where T : class
    {
        var dictKey = new Tuple<Type, object>(typeof(T), key);

        if (Instance.services.ContainsKey(dictKey))
        {
            throw new InvalidOperationException(string.Format("Cannot register an item of type: {0} and key: {1}. That type is already registered.", dictKey.Item1, key));
        }
        else
        {
            Instance.services.Add(dictKey, service);
        }
    }

    /// <summary>
    /// Unregisters the specified un-keyed service.
    /// </summary>
    /// <typeparam name="T">The type of service.</typeparam>
    /// <param name="service">The service.</param>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the given service and key are not registered to this ServiceLocator.</exception>
    public static void Unregister<T>(T service) where T : class
    {
        Unregister(service, null);
    }

    /// <summary>
    /// Unregisters the specified keyed service.
    /// </summary>
    /// <typeparam name="T">The type of service.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="key">The key.</param>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the given service and key are not registered to this ServiceLocator.</exception>
    public static void Unregister<T>(T service, object key) where T : class
    {
        var type = typeof(T);
        var dictKey = new Tuple<Type, object>(type, key);

        if (!Instance.services.Remove(dictKey))
        {
            throw new KeyNotFoundException(string.Format("Could not find a service of type {0} with key {1}", type, key));
        }
    }
}