using Splunk.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SplunkLogHelper
{
    public class SplunkLogService
    {
        private static TraceSource traceSource;
        private static HttpEventCollectorTraceListener listener;
        private static Uri _splunkConnectionString;
        private static Guid _splunkToken;
        private static string _projectName = "";

        public static void Configure(string projectName, string splunkConnectionString, string splunkToken)
        {
            Uri.TryCreate(splunkConnectionString, UriKind.Absolute, out _splunkConnectionString);
            Guid.TryParse(splunkToken, out _splunkToken);
            _projectName = projectName;
        }

        private static void InitializeTraceSource()
        {
            if (traceSource == null)
            {
                if (_splunkConnectionString == null) throw new Exception("SplunkLogService connection string not supplied");
                if (_splunkToken == null) throw new Exception("SplunkLogService token not supplied");

                traceSource = new TraceSource($"{_projectName}Logger");
                traceSource.Switch.Level = SourceLevels.All;
                traceSource.Listeners.Clear();
                listener = new HttpEventCollectorTraceListener(
                    uri: _splunkConnectionString,
                    token: _splunkToken.ToString().ToUpper(),
                    sendMode: HttpEventCollectorSender.SendMode.Parallel,
                    batchInterval: 1000,
                    batchSizeBytes: 10240,
                    batchSizeCount: 10
                    );
                listener.AddLoggingFailureHandler(e =>
                {
                    //if there are issues writing to splunk they should show up here.
                    Trace.TraceError(e.Message);
                });

                traceSource.Listeners.Add(listener);
            }
        }

        /// <summary>
        /// Make sure to call close in console apps.
        /// The flush and close need to be called for some reason, otherwise the logs may not be sent.
        /// https://github.com/splunk/splunk-library-dotnetlogging/issues/14
        /// </summary>
        public static void Close()
        {
            if (listener != null)
            {
                listener.Flush();
                listener.Close();
            }

            if (traceSource != null)
            {
                traceSource.Flush();
                traceSource.Close();
            }
        }

        private static readonly string CORRELATION_ID_HTTP_HEADER = "X-Correlation-Id";

        //private static string GetCurrentCorrelationId()
        //{
        //    if (HttpContext.Current != null && HttpContext.Current.Request != null)
        //    {
        //        return HttpContext.Current.Request.Headers[CORRELATION_ID_HTTP_HEADER];
        //    }
        //    return null;
        //}

        public static void SendMessage(TraceEventType type, int index, string message, object payload = null)
        {
            object obj = new
            {
                Source = _projectName,
                Timestamp = DateTime.UtcNow,
                Message = message,
                Payload = payload,
                //CorrelationId = GetCurrentCorrelationId(),
            };
            SendPayload(type, index, obj);
        }

        public static void SendError(TraceEventType type, int index, string message, Exception e = null, object payload = null)
        {
            object obj = new
            {
                Source = _projectName,
                Timestamp = DateTime.UtcNow,
                Message = message,
                Error = e,
                Payload = payload,
                //CorrelationId = GetCurrentCorrelationId(),
            };
            SendPayload(type, index, obj);
        }

        public static void SendPayload(TraceEventType type, int index, object payload)
        {
            if (traceSource == null) InitializeTraceSource();

            try
            {
                traceSource.TraceData(type, index, new[] { payload });

            }
            catch (Exception e)
            {
                Trace.TraceError("Error ocurred while sending message to splunk: " + e.Message, e);
            }
        }
    }
}