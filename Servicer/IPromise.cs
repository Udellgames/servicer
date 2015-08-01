namespace Servicer
{
    using System;

    /// <summary>
    /// Interface for promises that eventually deliver a value.
    /// </summary>
    /// <typeparam name="T">The type of value that is promised.</typeparam>
    public interface IPromise<T>
    {
        /// <summary>
        /// Occurs when the promise has been fulfilled.
        /// </summary>
        event EventHandler<PromiseFulfilledEventArgs<T>> Done;

        /// <summary>
        /// Occurs when promise fulfillment has failed.
        /// </summary>
        event EventHandler<ExceptionEventArgs> Failed;

        /// <summary>
        /// Occurs when the promise has been fulfilled, or has failed.
        /// </summary>
        event EventHandler Finally;

        /// <summary>
        /// Requires a value from the promise. If the promise has not been fulfilled an exception is thrown.
        /// </summary>
        /// <returns>The fulfilled value of the promise.</returns>
        T Require();
    }
}