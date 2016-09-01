using System.Diagnostics;

namespace HyperIoC.Logging
{
    /// <summary>
    /// Default config logging.
    /// </summary>
    public class DebugConfigLogger : IConfigLogger
    {
        /// <summary>
        /// Logs the message to System.Diagnostics.Debug.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Log(string message)
        {
            Debug.WriteLine(message ?? "");
        }
    }
}