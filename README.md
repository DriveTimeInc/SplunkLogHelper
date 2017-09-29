# Splunk Log Helper

Getting Splunk set up and working in a .NET project can be challenging. This library is a wrapper on top of the Splunk Trace Listener to easily get started with Splunk. 

The library provides a static 'core' class that you may call directly, or use an interface/instance class if you'd like to do dependancy injection.

## Usage

```
using SplunkLogHelper;

string projectName = "Project Name";
string splunkConnectionString = "https://http-inputs-********.splunkcloud.com/services/collector/event";
string splunkToken = "<Splunk Guid Token>";

//Configures the service, needs to be done once at project startup
var logger = new LogService(projectName, splunkConnectionString, splunkToken);

//Log a Message
logger.Message(TraceEventType.Information, 0, message, payload);

//Log an Error
logger.Error(TraceEventType.Error, 0, "Error Message", ex, payload);

//Log a Warning
logger.Warning(TraceEventType.Error, 0, "Error Message", ex, payload);

//If using a console app, you may need to call Close() to push any remaining messages
logger.Dispose()
```

### Static Class Usage

If you'd like to use the helper directly, you can call the static `SplunkLogService` class. 

```
using SplunkLogHelper;

string projectName = "Project Name";
string splunkConnectionString = "https://http-inputs-********.splunkcloud.com/services/collector/event";
string splunkToken = "<Splunk Guid Token>";

//Configures the service, needs to be done once at project startup
SplunkLogService.Configure(projectName, splunkConnectionString, splunkToken);

//Log a message
SplunkLogService.SendMessage(TraceEventType.Information, 0, message, payload);

//Log an Error
SplunkLogService.SendError(TraceEventType.Error, 0, "Error Message", ex, payload);

//If using a console App, you may need to call Close() to push any remaining messages
SplunkLogService.Close()
```

# To Do

* Testing
* Nuget Package
* Extend `SplunkLogService` to accept a callback when errors occur sending messages to Splunk

# Contributing

If you'd like to contribute, just submit a PR.
