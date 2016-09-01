namespace HyperIoC.Logging
{
    /// <summary>
    /// Defines the members of the IConfigLogger interface. This is used to allow callers to log out the config
    /// of the factory registrations for debug purposes. A built-in version that writes to the Debug window
    /// is already provided.
    /// </summary>
    public interface IConfigLogger
    {
        void Log(string message);
    }
}
