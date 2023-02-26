This repository contains two nuget packages: `Serilog.Sinks.OpenSearch` and `Serilog.Formatting.OpenSearch`.

*NOTICE*
This sink is a port from the project Serilog.Sinks.Elasticsearch.
There are still many unrelavent references to Elasticsearch, feel free to help out! 


## Table of contents

* [What is this sink](#what-is-this-sink)
* [Features](#features)
* [Quick start](#quick-start)
  * [OpenSearch sinks](#elasticsearch-sinks)
  * [OpenSearch formatters](#elasticsearch-formatters)
* [More information](#more-information)
  * [A note about fields inside OpenSearch](#a-note-about-fields-inside-elasticsearch)
  * [A note about Kibana](#a-note-about-kibana)
  * [JSON `appsettings.json` configuration](#json-appsettingsjson-configuration)
  * [Handling errors](#handling-errors)
  * [Breaking changes](#breaking-changes)

## What is this sink
Elasticsearch packages (Elasticsearch.NET and NEST) with versions > 7.17 no longer support writing to OpenSearch and OpenDistro.
The Serilog OpenSearch sink project is a sink (basically a writer) for the Serilog logging framework. Structured log events are written to sinks and each sink is responsible for writing it to its own backend, database, store etc. This sink delivers the data to OpeanSearch, a NoSQL search engine. It does this in a similar structure as Logstash and makes it easy to use OpenSearch Dashboards for visualizing your logs.

## Features

* Simple configuration to get log events published to OpenSearch. Only server address is needed.
* All properties are stored inside fields in OpenSearch. This allows you to query on all the relevant data but also run analytics over this data.
* Be able to customize the store; specify the index name being used, the serializer or the connections to the server (load balanced).
* Durable mode; store the logevents first on disk before delivering them to ES making sure you never miss events if you have trouble connecting to your OpenSearch cluster.
* Automatically create the right mappings for the best usage of the log events in OpenSearch or automatically upload your own custom mapping.
* Compatible with OpenSearch > 1.0.0


## Quick start

### OpenSearch sinks

```powershell
Install-Package serilog.sinks.OpenSearch
```

Simplest way to register this sink is to use default configuration:

```csharp
var loggerConfig = new LoggerConfiguration()
    .WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri("http://localhost:9200")));
```

Or, if using .NET Core and `Serilog.Settings.Configuration` Nuget package and `appsettings.json`, default configuration would look like this:

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.OpenSearch" ],
    "MinimumLevel": "Warning",
    "WriteTo": [
      {
        "Name": "OpenSearch",
        "Args": {
          "nodeUris": "http://localhost:9200"
        }
      }
    ]
  }
}
```

More elaborate configuration, using additional Nuget packages (e.g. `Serilog.Enrichers.Environment`) would look like:

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.OpenSearch" ],
    "MinimumLevel": "Warning",
    "WriteTo": [
      {
        "Name": "OpenSearch",
        "Args": {
          "nodeUris": "http://localhost:9200"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName" ],
    "Properties": {
      "Application": "My app"
    }
  }
}
```
### Configurable properties

Besides a registration of the sink in the code, it is possible to register it using appSettings reader (from v2.0.42+) reader (from v2.0.42+) as shown below.

This example shows the options that are currently available when using the appSettings reader.

```xml
  <appSettings>
    <add key="serilog:using" value="Serilog.Sinks.OpenSearch"/>
    <add key="serilog:write-to:OpenSearch.nodeUris" value="http://localhost:9200;http://remotehost:9200"/>
    <add key="serilog:write-to:OpenSearch.indexFormat" value="custom-index-{0:yyyy.MM}"/>
    <add key="serilog:write-to:OpenSearch.templateName" value="myCustomTemplate"/>
    <add key="serilog:write-to:OpenSearch.typeName" value="myCustomLogEventType"/>
    <add key="serilog:write-to:OpenSearch.pipelineName" value="myCustomPipelineName"/>
    <add key="serilog:write-to:OpenSearch.batchPostingLimit" value="50"/>
    <add key="serilog:write-to:OpenSearch.batchAction" value="Create"/><!-- "Index" is default -->
    <add key="serilog:write-to:OpenSearch.period" value="2"/>
    <add key="serilog:write-to:OpenSearch.inlineFields" value="true"/>
    <add key="serilog:write-to:OpenSearch.restrictedToMinimumLevel" value="Warning"/>
    <add key="serilog:write-to:OpenSearch.bufferBaseFilename" value="C:\Temp\SerilogOpenSearchBuffer"/>
    <add key="serilog:write-to:OpenSearch.bufferFileSizeLimitBytes" value="5242880"/>
    <add key="serilog:write-to:OpenSearch.bufferLogShippingInterval" value="5000"/>
    <add key="serilog:write-to:OpenSearch.bufferRetainedInvalidPayloadsLimitBytes" value="5000"/>
    <add key="serilog:write-to:OpenSearch.bufferFileCountLimit " value="31"/>
    <add key="serilog:write-to:OpenSearch.connectionGlobalHeaders" value="Authorization=Bearer SOME-TOKEN;OtherHeader=OTHER-HEADER-VALUE" />
    <add key="serilog:write-to:OpenSearch.connectionTimeout" value="5" />
    <add key="serilog:write-to:OpenSearch.emitEventFailure" value="WriteToSelfLog" />
    <add key="serilog:write-to:OpenSearch.queueSizeLimit" value="100000" />
    <add key="serilog:write-to:OpenSearch.autoRegisterTemplate" value="true" />
    <add key="serilog:write-to:OpenSearch.overwriteTemplate" value="false" />
    <add key="serilog:write-to:OpenSearch.registerTemplateFailure" value="IndexAnyway" />
    <add key="serilog:write-to:OpenSearch.deadLetterIndexName" value="deadletter-{0:yyyy.MM}" />
    <add key="serilog:write-to:OpenSearch.numberOfShards" value="20" />
    <add key="serilog:write-to:OpenSearch.numberOfReplicas" value="10" />
    <add key="serilog:write-to:OpenSearch.formatProvider" value="My.Namespace.MyFormatProvider, My.Assembly.Name" />
    <add key="serilog:write-to:OpenSearch.connection" value="My.Namespace.MyConnection, My.Assembly.Name" />
    <add key="serilog:write-to:OpenSearch.serializer" value="My.Namespace.MySerializer, My.Assembly.Name" />
    <add key="serilog:write-to:OpenSearch.connectionPool" value="My.Namespace.MyConnectionPool, My.Assembly.Name" />
    <add key="serilog:write-to:OpenSearch.customFormatter" value="My.Namespace.MyCustomFormatter, My.Assembly.Name" />
    <add key="serilog:write-to:OpenSearch.customDurableFormatter" value="My.Namespace.MyCustomDurableFormatter, My.Assembly.Name" />
    <add key="serilog:write-to:OpenSearch.failureSink" value="My.Namespace.MyFailureSink, My.Assembly.Name" />
  </appSettings>
```

With the appSettings configuration the `nodeUris` property is required. Multiple nodes can be specified using `,` or `;` to separate them. All other properties are optional. Also required is the `<add key="serilog:using" value="Serilog.Sinks.OpenSearch"/>` setting to include this sink. All other properties are optional. If you do not explicitly specify an indexFormat-setting, a generic index such as 'logstash-[current_date]' will be used automatically.

And start writing your events using Serilog.

### OpenSearch formatters

```powershell
Install-Package serilog.formatting.elasticsearch
```

The `Serilog.Formatting.OpenSearch` nuget package consists of a several formatters:

* `OpenSearchJsonFormatter` - custom json formatter that respects the configured property name handling and forces `Timestamp` to @timestamp.
* `ExceptionAsObjectJsonFormatter` - a json formatter which serializes any exception into an exception object.

Override default formatter if it's possible with selected sink

```csharp
var loggerConfig = new LoggerConfiguration()
  .WriteTo.Console(new OpenSearchJsonFormatter());
```

## More information

* [Basic information](https://github.com/serilog/serilog-sinks-elasticsearch/wiki/basic-setup) on how to configure and use this sink.
* [Configuration options](https://github.com/serilog/serilog-sinks-elasticsearch/wiki/Configure-the-sink) which you can use.
* How to use the [durability](https://github.com/serilog/serilog-sinks-elasticsearch/wiki/durability) mode.
* Get the [NuGet package](http://www.nuget.org/packages/Serilog.Sinks.Elasticsearch).
* Report issues to the [issue tracker](https://github.com/serilog/serilog-sinks-elasticsearch/issues). PR welcome, but please do this against the dev branch.
* For an overview of recent changes, have a look at the [change log](https://github.com/serilog/serilog-sinks-elasticsearch/blob/master/CHANGES.md).

### A note about fields inside OpenSearch

Be aware that there is an explicit and implicit mapping of types inside an OpenSearch index. A value called `X` as a string will be indexed as being a string. Sending the same `X` as an integer in a next log message will not work. ES will raise a mapping exception, however it is not that evident that your log item was not stored due to the bulk actions performed.

So be careful about defining and using your fields (and type of fields). It is easy to miss that you first send a {User} as a simple username (string) and next as a User object. The first mapping dynamically created in the index wins. See also issue [#184](https://github.com/serilog/serilog-sinks-elasticsearch/issues/184) for details and a possible solution. There are also limits in ES on the number of dynamic fields you can actually throw inside an index.

### A note about Kibana

In order to avoid a potentially deeply nested JSON structure for exceptions with inner exceptions,
by default the logged exception and it's inner exception is logged as an array of exceptions in the field `exceptions`. Use the 'Depth' field to traverse the inner exceptions flow.

However, not all features in Kibana work just as well with JSON arrays - for instance, including
exception fields on dashboards and visualizations. Therefore, we provide an alternative formatter,  `ExceptionAsObjectJsonFormatter`, which will serialize the exception into the `exception` field as an object with nested `InnerException` properties. This was also the default behavior of the sink before version 2.

To use it, simply specify it as the `CustomFormatter` when creating the sink:

```csharp
    new OpenSearchSink(new OpenSearchSinkOptions(url)
    {
      CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage:true)
    });
```

### JSON `appsettings.json` configuration

To use the OpenSearch sink with _Microsoft.Extensions.Configuration_, for example with ASP.NET Core or .NET Core, use the [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) package. First install that package if you have not already done so:

```powershell
Install-Package Serilog.Settings.Configuration
```

Instead of configuring the sink directly in code, call `ReadFrom.Configuration()`:

```csharp
var configuration = new ConfigurationBuilder()
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
```

In your `appsettings.json` file, under the `Serilog` node, :

```json
{
  "Serilog": {
    "WriteTo": [{
        "Name": "OpenSearch",
        "Args": {
          "nodeUris": "http://localhost:9200;http://remotehost:9200/",
          "indexFormat": "custom-index-{0:yyyy.MM}",
          "templateName": "myCustomTemplate",
          "typeName": "myCustomLogEventType",
          "pipelineName": "myCustomPipelineName",
          "batchPostingLimit": 50,
          "batchAction": "Create",
          "period": 2,
          "inlineFields": true,
          "restrictedToMinimumLevel": "Warning",
          "bufferBaseFilename":  "C:/Temp/docker-elk-serilog-web-buffer",
          "bufferFileSizeLimitBytes": 5242880,
          "bufferLogShippingInterval": 5000,
          "bufferRetainedInvalidPayloadsLimitBytes": 5000,
          "bufferFileCountLimit": 31,
          "connectionGlobalHeaders" :"Authorization=Bearer SOME-TOKEN;OtherHeader=OTHER-HEADER-VALUE",
          "connectionTimeout": 5,
          "emitEventFailure": "WriteToSelfLog",
          "queueSizeLimit": "100000",
          "autoRegisterTemplate": true,
          "overwriteTemplate": false,
          "registerTemplateFailure": "IndexAnyway",
          "deadLetterIndexName": "deadletter-{0:yyyy.MM}",
          "numberOfShards": 20,
          "numberOfReplicas": 10,
          "templateCustomSettings": [{ "index.mapping.total_fields.limit": "10000000" } ],
          "formatProvider": "My.Namespace.MyFormatProvider, My.Assembly.Name",
          "connection": "My.Namespace.MyConnection, My.Assembly.Name",
          "serializer": "My.Namespace.MySerializer, My.Assembly.Name",
          "connectionPool": "My.Namespace.MyConnectionPool, My.Assembly.Name",
          "customFormatter": "My.Namespace.MyCustomFormatter, My.Assembly.Name",
          "customDurableFormatter": "My.Namespace.MyCustomDurableFormatter, My.Assembly.Name",
          "failureSink": "My.Namespace.MyFailureSink, My.Assembly.Name"
        }
    }]
  }
}
```

See the XML `<appSettings>` example above for a discussion of available `Args` options.

### Handling errors

From version 5.5 you have the option to specify how to handle issues with OpenSearch. Since the sink delivers in a batch, it might be possible that one or more events could actually not be stored in the OpenSearch store.
Can be a mapping issue for example. It is hard to find out what happened here. There is a new option called *EmitEventFailure* which is an enum (flagged) with the following options:

* WriteToSelfLog, the default option in which the errors are written to the SelfLog.
* WriteToFailureSink, the failed events are send to another sink. Make sure to configure this one by setting the FailureSink option.
* ThrowException, in which an exception is raised.
* RaiseCallback, the failure callback function will be called when the event cannot be submitted to OpenSearch. Make sure to set the FailureCallback option to handle the event.

An example:

```csharp
.WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                       EmitEventFailureHandling.WriteToFailureSink |
                                       EmitEventFailureHandling.RaiseCallback,
                    FailureSink = new FileSink("./failures.txt", new JsonFormatter(), null)
                })
```

With the AutoRegisterTemplate option the sink will write a default template to OpenSearch. When this template is not there, you might not want to index as it can influence the data quality.
Since version 5.5 you can use the RegisterTemplateFailure option. Set it to one of the following options:

* IndexAnyway; the default option, the events will be send to the server
* IndexToDeadletterIndex; using the deadletterindex format, it will write the events to the deadletter queue. When you fix your template mapping, you can copy your data into the right index.
* FailSink; this will simply fail the sink by raising an exception.

Since version 7 you can  specify an action to do when log row was denied by the elasticsearch because of the data (payload) if durable file is specied.
i.e.

```csharp
BufferCleanPayload = (failingEvent, statuscode, exception) =>
                    {
                        dynamic e = JObject.Parse(failingEvent);
                        return JsonConvert.SerializeObject(new Dictionary<string, object>()
                        {
                            { "@timestamp",e["@timestamp"]},
                            { "level","Error"},
                            { "message","Error: "+e.message},
                            { "messageTemplate",e.messageTemplate},
                            { "failingStatusCode", statuscode},
                            { "failingException", exception}
                        });
                    },
```

The IndexDecider didnt worked well when durable file was specified so an option to specify BufferIndexDecider is added.
Datatype of logEvent is string
i.e.

```csharp
 BufferIndexDecider = (logEvent, offset) => "log-serilog-" + (new Random().Next(0, 2)),
```

Option BufferFileCountLimit is added. The maximum number of log files that will be retained. including the current log file. For unlimited retention, pass null. The default is 31.
Option BufferFileSizeLimitBytes is added The maximum size, in bytes, to which the buffer log file for a specific date will be allowed to grow. By default `100L * 1024 * 1024` will be applied.

### Breaking Changes
#### Version 1.2.0 (starting off with the same version as the OpenSearch.Net and OpenSearch.Client versions)

* Removed Elasticsearch packages and added OpenSearch packages
* Changed naming conventions

### serilog-sinks-elasticsearch Breaking changes

#### Version 9

* Dropped support for 456 and sticking now with NETSTANDARD
* Dropped support for Opensearch - This package supported writing to Opensearch (without guarantees) up untill the last version, Updated ES packages dropped support for Opensearch

#### Version 7

* Nuget Serilog.Sinks.File is now used instead of deprecated Serilog.Sinks.RollingFile
* SingleEventSizePostingLimit option is changed from int to long? with default value null, Don't use value 0 nothing will be logged then!!!!!

#### Version 6

Starting from version 6, the sink has been upgraded to work with Elasticsearch 6.0 and has support for the new templates used by ES 6.

> If you use the `AutoRegisterTemplate` option, you need to set the `AutoRegisterTemplateVersion` option to `ESv6` in order to generate default templates that are compatible with the breaking changes in ES 6.

#### Version 4

Starting from version 4, the sink has been upgraded to work with Serilog 2.0 and has .NET Core support.

#### Version 3

Starting from version 3, the sink supports the Elasticsearch.Net 2 package and Elasticsearch version 2. If you need Elasticsearch 1.x support, then stick with version 2 of the sink.
The function

```csharp
protected virtual ElasticsearchResponse<T> EmitBatchChecked<T>(IEnumerable<LogEvent> events)
```

now uses a generic type. This allows you to map to either DynamicResponse when using Elasticsearch.NET or to BulkResponse if you want to use NEST.

We also dropped support for .NET 4 since the Elasticsearch.NET client also does not support this version of the framework anymore. If you need to use .net 4, then you need to stick with the 2.x version of the sink.

#### Version 2

Be aware that version 2 introduces some breaking changes.

* The overloads have been reduced to a single Elasticsearch function in which you can pass an options object.
* The namespace and function names are now Elasticsearch instead of ElasticSearch everywhere
* The Exceptions recorded by Serilog are customer serialized into the Exceptions property which is an array instead of an object.
* Inner exceptions are recorded in the same array but have an increasing depth parameter. So instead of nesting objects you need to look at this parameter to find the depth of the exception.
* Do no longer use the mapping once provided in the Gist. The Sink can automatically create the right mapping for you, but this feature is disabled by default. We advice you to use it.
* Since version 2.0.42 the ability to register this sink using the AppSettings reader is restored. You can pass in a node (or collection of nodes) and optionally an indexname and template.
