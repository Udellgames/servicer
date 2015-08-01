namespace Servicer
{
    using System;

    /// <summary>
    /// Event args for when an exception is thrown.
    /// </summary>
    public sealed class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; private set; }
    }
}