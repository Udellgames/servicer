namespace Servicer
{
    using System;

    /// <summary>
    /// Interface for promises that eventually deliver a value.
    /// </summary>
    /// <typeparam name="T">The type of value that is promised.</typeparam>
    [Obsolete("Do not use promises in .NET 4+. Use Tasks with Service Locator's GetServiceAsync method instead.")]
    public interface IPromise<T>
    {
        /// <summary>
        /// Occurs when the promise has been fulfilled.
        /// </summary>
        event EventHandler<PromiseFulfilledEventArgs<T>> Done;

        /// <summary>
        /// Requires a value from the promise. If the promise has not been fulfilled an exception is thrown.
        /// </summary>
        /// <returns>The fulfilled value of the promise.</returns>
        T Require();
    }
}