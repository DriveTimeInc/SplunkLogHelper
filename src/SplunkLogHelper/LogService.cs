using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplunkLogHelper
{

    public class LogService : ILog, IDisposable
    {
        public LogService(string projectName, string splunkConnectionString = "", string splunkToken = "")
        {
            var conn = splunkConnectionString ?? ConfigurationManager.AppSettings["SplunkConnectionString"];
            var token = splunkToken ?? ConfigurationManager.AppSettings["SplunkToken"];

            SplunkLogService.Configure(projectName, conn, token);
        }

        public void Error(string message, Exception ex, object payload = null)
        {
            SplunkLogService.SendError(TraceEventType.Error, 0, message, ex, payload);
        }

        public void Message(string message, object payload = null)
        {
            SplunkLogService.SendMessage(TraceEventType.Information, 0, message, payload);
        }

        public void Warning(string warning, object payload = null)
        {
            SplunkLogService.SendMessage(TraceEventType.Warning, 0, warning, payload);
        }

        public void Dispose()
        {
            SplunkLogService.Close();
        }
    }
}
