namespace Servicer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Simple service locator
    /// </summary>
    public sealed class ServiceLocator
    {
        /// <summary>
        /// The instance for the Singleton pattern.
        /// </summary>
        /// <remarks>
        /// This isn't lazy, but it is thread-safe. Laziness is not necessary as this class is very light-weight initially.
        /// </remarks>
        private static readonly ServiceLocator InstanceValue = new ServiceLocator();

        private static MethodInfo fulfillPromiseMethodInfo = typeof(ServiceLocator).GetMethod("FulfillPromise", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IDictionary<Tuple<Type, object>, IPromise> promises = new Dictionary<Tuple<Type, object>, IPromise>();

        /// <summary>
        /// The services
        /// </summary>
        private readonly IDictionary<Tuple<Type, object>, object> services = new Dictionary<Tuple<Type, object>, object>();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ServiceLocator()
        {
        }

        private ServiceLocator()
        {
        }

        private interface IPromise
        {
        }

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
                return InstanceValue;
            }
        }

        /// <summary>
        /// Clears this instance by removing all services.
        /// </summary>
        public static void Clear()
        {
            Instance.services.Clear();
            Instance.promises.Clear();
        }

        /// <summary>
        /// Gets a promised service.
        /// </summary>
        /// <typeparam name="T">The type of service required.</typeparam>
        /// <returns>A promise to deliver the service when it is registered.</returns>
        public static IPromise<T> GetPromisedService<T>() where T : class
        {
            return GetPromisedService<T>(null);
        }

        /// <summary>
        /// Gets a promised service.
        /// </summary>
        /// <typeparam name="T">The type of service required.</typeparam>
        /// <param name="key">The key to filter multiple services of the same type by.</param>
        /// <returns>A promise to deliver the service when it is registered.</returns>
        public static IPromise<T> GetPromisedService<T>(object key) where T : class
        {
            var type = typeof(T);
            var dictKey = new Tuple<Type, object>(type, key);

            object returnValue;

            if (Instance.services.TryGetValue(dictKey, out returnValue))
            {
                return NewFulfilledPromise<T>(returnValue);
            }

            if (!Explicit)
            {
                var subTypeKey = Instance.services.Keys.FirstOrDefault(
                    x =>
                        (key == null ? x.Item2 == null : key.Equals(x.Item2)) &&
                        type.IsAssignableFrom(x.Item1));

                if (subTypeKey != null)
                {
                    return NewFulfilledPromise<T>(Instance.services[subTypeKey]);
                }
            }

            if (!Instance.promises.ContainsKey(dictKey))
            {
                Instance.promises.Add(dictKey, new Promise<T>());
            }

            return (Promise<T>)Instance.promises[dictKey];
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
        /// <exception cref="KeyNotFoundException">Thrown when the Service Locator cannot find an a service of the requested type with a matching key and Explicit is true, or when the Service Locator cannot find a service of the requested type or any type that can be assigned to the requested type with the matching key and Explicit is false.</exception>
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
        /// <exception cref="InvalidOperationException">Thrown when a service of this type has already been registered without a key.</exception>
        public static void Register<T>(T service) where T : class
        {
            Register(service, null);
        }

        /// <summary>
        /// Registers the specified service with a key. Any existing promises for that service will be fulfilled.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <exception cref="System.InvalidOperationException">Thrown when a service of this type has already been registered with this key.</exception>
        public static void Register<T>(T service, object key) where T : class
        {
            var type = typeof(T);
            var dictKey = new Tuple<Type, object>(type, key);

            if (Instance.services.ContainsKey(dictKey))
            {
                throw new InvalidOperationException(string.Format("Cannot register an item of type: {0} and key: {1}. That type is already registered.", dictKey.Item1, key));
            }
            else
            {
                Instance.services.Add(dictKey, service);

                IPromise promise;
                if (Instance.promises.TryGetValue(dictKey, out promise))
                {
                    ((Promise<T>)promise).Fulfill(service);
                    InstanceValue.promises.Remove(dictKey);
                }

                if (!Explicit)
                {
                    var subTypeKeys = Instance.promises.Keys.Where(
                        x =>
                            (key == null ? x.Item2 == null : key.Equals(x.Item2)) &&
                            x.Item1.IsAssignableFrom(type));

                    foreach (var subTypeKey in subTypeKeys.ToList())
                    {
                        // We have to use reflection to execute a generic method typed to a type variable.
                        var methodInfo = fulfillPromiseMethodInfo.MakeGenericMethod(subTypeKey.Item1);
                        methodInfo.Invoke(InstanceValue, new object[] { InstanceValue.promises[subTypeKey], service });
                        InstanceValue.promises.Remove(subTypeKey);
                    }
                }
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

        /// <summary>
        /// Creates a new pre-filled promise.
        /// </summary>
        /// <typeparam name="T">The type of value the promise intends to deliver.</typeparam>
        /// <param name="value">The value used to fulfill the promise.</param>
        /// <returns>A new promise, already fulfilled.</returns>
        private static IPromise<T> NewFulfilledPromise<T>(object value) where T : class
        {
            var promise = new Promise<T>();
            promise.Fulfill((T)value);
            return promise;
        }

        /// <summary>
        /// Fulfills the promise.
        /// </summary>
        /// <remarks>
        /// This method is part of a work-around for generic typing. See Register(service, key) for more information.
        /// </remarks>
        /// <typeparam name="T">The type of value to fulfill the promise with.</typeparam>
        /// <param name="promise">The promise.</param>
        /// <param name="value">The value.</param>
        private void FulfillPromise<T>(IPromise promise, T value) where T : class
        {
            ((Promise<T>)promise).Fulfill(value);
        }

        /// <summary>
        /// A promise to deliver a service.
        /// </summary>
        /// <typeparam name="T">The type of sevice.</typeparam>
        private sealed class Promise<T> : IPromise<T>, IPromise where T : class
        {
            private object eventLock = new object();
            private T fulfilledValue;

            /// <summary>
            /// Occurs when the promise has been fulfilled.
            /// </summary>
            /// <remarks>
            /// If the promise has already been fulfilled, any event handlers attached to this instance will immediately be fired at the point of attachment.
            /// </remarks>
            public event EventHandler<PromiseFulfilledEventArgs<T>> Done
            {
                add
                {
                    lock (eventLock)
                    {
                        if (fulfilledValue == null)
                        {
                            DoneInternalEvent += value;
                        }
                        else
                        {
                            value.Fire(this, new PromiseFulfilledEventArgs<T>(fulfilledValue));
                        }
                    }
                }

                remove
                {
                    lock (eventLock)
                    {
                        DoneInternalEvent -= value;
                    }
                }
            }

            /// <summary>
            /// Occurs when the promise has been fulfilled.
            /// </summary>
            private event EventHandler<PromiseFulfilledEventArgs<T>> DoneInternalEvent;

            /// <summary>
            /// Fulfills this promise with the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public void Fulfill(T value)
            {
                fulfilledValue = value;
                DoneInternalEvent.Fire(this, new PromiseFulfilledEventArgs<T>(value));
            }

            /// <summary>
            /// Requires a value from the promise. If the promise has not been fulfilled an exception is thrown.
            /// </summary>
            /// <returns>
            /// The fulfilled value of the promise.
            /// </returns>
            /// <exception cref="PromiseUnfulfilledException">Thrown when the promise has not been fulfilled yet.</exception>
            public T Require()
            {
                if (fulfilledValue != null)
                {
                    return fulfilledValue;
                }

                throw new PromiseUnfulfilledException();
            }
        }

        /// <summary>
        /// Simple .NET 3.5 immutable Tuple class.
        /// </summary>
        /// <typeparam name="T">The type of the first item.</typeparam>
        /// <typeparam name="V">The type of the second item.</typeparam>
        [DebuggerDisplay("Item1: {Item1}, Item2: {Item2}")]
        private sealed class Tuple<T, V>
        {
            private readonly T item1;

            /// <summary>
            /// Gets or sets the second item.
            /// </summary>
            /// <value>
            /// The second item.
            /// </value>
            private readonly V item2;

            /// <summary>
            /// Initializes a new instance of the <see cref="Tuple{T, V}"/> class.
            /// </summary>
            /// <param name="item1">The first item.</param>
            /// <param name="item2">The second item.</param>
            public Tuple(T item1, V item2)
            {
                this.item1 = item1;
                this.item2 = item2;
            }

            /// <summary>
            /// Gets the first item.
            /// </summary>
            /// <value>
            /// The first item.
            /// </value>
            public T Item1
            {
                get { return item1; }
            }

            /// <summary>
            /// Gets the second item.
            /// </summary>
            /// <value>
            /// The second item.
            /// </value>
            public V Item2
            {
                get { return item2; }
            }

            /// <summary>
            /// Determines whether the specified <see cref="object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
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
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Potential Code Quality Issues", "RECS0017:Possible compare of value type with 'null'", Justification = "We allow a generic comparison with null here, because .GetHashCode still works fine on default values.")]
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
    }
}