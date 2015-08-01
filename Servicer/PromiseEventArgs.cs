namespace Servicer
{
    using System;

    /// <summary>
    /// Event args for when a promise is fulfilled.
    /// </summary>
    /// <typeparam name="T">The type of value that fulfills this promise.</typeparam>
    public sealed class PromiseFulfilledEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromiseFulfilledEventArgs{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PromiseFulfilledEventArgs(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; private set; }
    }
}