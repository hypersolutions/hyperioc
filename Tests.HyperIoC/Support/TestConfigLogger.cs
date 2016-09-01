using HyperIoC.Logging;

namespace Tests.HyperIoC.Support
{
    public class TestConfigLogger : IConfigLogger
    {
        public string Message { get; private set; }

        public void Log(string message)
        {
            Message = message;
        }
    }
}
