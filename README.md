



# Overview
[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://eshopworld.visualstudio.com/Github%20build/_build?definitionId=204&_a=summary) [![NuGet](https://img.shields.io/badge/myget-v2.6.2-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)

The Data Analytics Transmission Component (DATC) is a [.NET Standard 2.0](https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/) library, that includes components specific to [.NET Framework 4.6](https://en.wikipedia.org/wiki/.NET_Framework) and [.NET Core 2](https://en.wikipedia.org/wiki/.NET_Core), to transmit data from your application to [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/). Data is transmitted asynchronously from an in-memory cache, resulting in evenly-balanced throughput, and low resource overhead

![DATC Data Flow](https://storage.googleapis.com/streaming-library/implicit-mode.PNG)
# Installation
## .NET Framework 4.6
[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://eshopworld.visualstudio.com/Github%20build/_build?definitionId=644&_a=summary) [![NuGet](https://img.shields.io/badge/myget-v2.2.1-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming.AspNet)

Install the `Strada` NuGet package
`Install-Package Eshopworld.Strada.Plugins.Streaming`

and the `Strada ASP.NET` NuGet package
`Install-Package Eshopworld.Strada.Plugins.Streaming.AspNet`
### Authentication
A `CloudServiceCredentials` instance is necessary to establish a persistent connection with the Data Analytics Cloud. Authentication meta is stored in JSON format
```json
{
	"Type": "service_account",
	"project_id": "{GCP Project ID}",
	"private_key_id": "{Your private key ID}",
	"private_key": "{Your private key}",
	"client_email": "{Your GCP custom email address}",
	"client_id": "{Your GCP client ID}",
	"auth_uri": "https://accounts.google.com/o/oauth2/auth",
	"token_uri": "https://oauth2.googleapis.com/token",
	"auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
	"client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/deploy%40eshop-puddle.iam.gserviceaccount.com"
}
```
Deserialize these credentials in the `Application_Start` method of your `Global.asax.cs` file
```csharp
var gcpServiceCredentials =
	JsonConvert.DeserializeObject<CloudServiceCredentials>(Resources.GcpServiceCredentials);
```
Note, this examples assumes that the `GcpServiceCredentials` JSON values are stored in a local `Resource` file, although this is not recommended for live deployments
### Configuration
A `DataTransmissionClientConfigSettings` instance is necessary to configure the streaming component. Configuration meta is also stored in JSON format 
```json
{
	"ProjectId": "{GCP Project ID}",
	"TopicId": "{GCP Topic ID (Cloud Pub/Sub)}",
	"ElementCountThreshold": 1000,
	"DelayThreshold": 3
}
```
#### Configuration Explained
##### ProjectId
> The Google Cloud Project to which the streaming component will connect
##### TopicID
> The Google Cloud Pub/Sub Topic to which the streaming component will transmit data
##### ElementCountThreshold
> Event batches retained in memory will be uploaded once the count of events reaches this value, assuming that the `DelayThreshold` time period has not elapsed. Default, and max value, is 1,000
##### DelayThreshold
> Event batches retained in memory will be uploaded once this time period (in seconds) has elapsed, assuming that the count of events stored in memory has not reached the `ElementCountThreshold` value. Default value is 3 seconds

Deserialize these configuration settings in the `Application_Start` method of your `Global.asax.cs` file
```csharp
var dataTransmissionClientConfigSettings =
	JsonConvert.DeserializeObject<DataTransmissionClientConfigSettings>(
		Resources.DataTransmissionClientConfigSettings);
```
Note, this examples assumes that the `DataTransmissionClientConfigSettings` JSON values are stored in a local `Resource` file, although this is not recommended for live deployments

### Initialisation
Initialising the `DataTransmissionClient` establishes a persistent connection with Google Cloud Pub/Sub
```csharp
DataTransmissionClient.Instance.InitAsync(
	gcpServiceCredentials,
	dataTransmissionClientConfigSettings
).Wait();
```
Starting the `DataUploader` establishes a single-threaded background process that publishes data to Google Cloud Pub/Sub
```csharp
AspNet.DataUploader.Instance.StartAsync(
	DataTransmissionClient.Instance,
	EventMetaCache.Instance).Wait();
```
Notice the `EventMetaCache.Instance` parameter - this is a Singleton component that manages the retention and processing of events stored in memory

## .NET Core 2
[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://eshopworld.visualstudio.com/Github%20build/_build?definitionId=645&_a=summary) [![NuGet](https://img.shields.io/badge/myget-v2.1.8-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming.NetCore)

Install the `Strada` NuGet package
`Install-Package Eshopworld.Strada.Plugins.Streaming`

and the `Strada .NET Core` NuGet package
`Install-Package Eshopworld.Strada.Plugins.Streaming.NetCore`
### Authentication
A `CloudServiceCredentials` instance is necessary to establish a persistent connection with the Data Analytics Cloud. Authentication meta is stored in JSON format
```json
{
	"Type": "service_account",
	"project_id": "{GCP Project ID}",
	"private_key_id": "{Your private key ID}",
	"private_key": "{Your private key}",
	"client_email": "{Your GCP custom email address}",
	"client_id": "{Your GCP client ID}",
	"auth_uri": "https://accounts.google.com/o/oauth2/auth",
	"token_uri": "https://oauth2.googleapis.com/token",
	"auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
	"client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/deploy%40eshop-puddle.iam.gserviceaccount.com"
}
```
Deserialize these credentials in the `Configure` method of your `Startup` class
```csharp
var cloudServiceCredentials = new CloudServiceCredentials();
Configuration.GetSection("cloudServiceCredentials").Bind(cloudServiceCredentials);
```
Note, this examples assumes that the `CloudServiceCredentials` JSON values are stored in a local `appsettings.json` file, although this is not recommended for live deployments
### Configuration
A `DataTransmissionClientConfigSettings` instance is necessary to configure the streaming component. Configuration meta is also stored in JSON format 
```json
{
	"ProjectId": "{GCP Project ID}",
	"TopicId": "{GCP Topic ID (Cloud Pub/Sub)}",
	"ElementCountThreshold": 1000,
	"DelayThreshold": 3
}
```
#### Configuration Explained
##### ProjectId
> The Google Cloud Project to which the streaming component will connect
##### TopicID
> The Google Cloud Pub/Sub Topic to which the streaming component will transmit data
##### ElementCountThreshold
> Event batches retained in memory will be uploaded once the count of events reaches this value, assuming that the `DelayThreshold` time period has not elapsed. Default, and max value, is 1,000
##### DelayThreshold
> Event batches retained in memory will be uploaded once this time period (in seconds) has elapsed, assuming that the count of events stored in memory has not reached the `ElementCountThreshold` value. Default value is 3 seconds

Deserialize these configuration settings in the `Configure` method of your `Startup` class
```csharp
var dataTransmissionClientConfigSettings = new DataTransmissionClientConfigSettings();            
Configuration
	.GetSection("dataTransmissionClientConfigSettings")
	.Bind(dataTransmissionClientConfigSettings);
```
Note, this examples assumes that the `DataTransmissionClientConfigSettings` JSON values are stored in a local `appsettings.json` file, although this is not recommended for live deployments
### Initialisation
Initialising the `DataTransmissionClient` establishes a persistent connection with Google Cloud Pub/Sub
```csharp
DataTransmissionClient.Instance.InitAsync(
	gcpServiceCredentials,
	dataTransmissionClientConfigSettings
).Wait();
```
Starting the `DataUploader` establishes a single-threaded background process that publishes data to Google Cloud Pub/Sub
```csharp
DataUploader.Instance.StartAsync(
	DataTransmissionClient.Instance,
	EventMetaCache.Instance).Wait();
```
Notice the `EventMetaCache.Instance` parameter - this is a Singleton component that manages the retention and processing of events stored in memory
# Usage
## Device Fingerprinting
### Generating
Fingerprints are an alphanumeric series of characters that represent unique identifiers for user devices. These values are generated by the `correlation.js` self-executing JavaScript file that executes on the application UI. Include this `script` tag in your HTML markup to automatically generate a device fingerprint
```js
<script language="javascript" src="correlation.js"></script>
```
Now every XHR request to your application API will include a `FingerprintId` HTTP header. 
### Reading
Execute the following function in any API `Controller` method to read the fingerprint value from the fingerprint 
#### .NET Framework 4.6
```csharp
string fingerprint = AspNet.Functions.GetFingerprint(Request),
```
#### .NET Core 2
.NET Core introduces the [Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-2.2) concept. Using this concept we can leverage [Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection) to automatically load fingerprint values to API `Controllers`.  Include the `DataTransmissionMiddleware` in your application pipeline by executing the following in the `Configure` method of the `Startup` class
```csharp
app.UseMiddleware<DataTransmissionMiddleware>();
```
Include a reference to the `DataAnalyticsMeta` class in your controller
```csharp
public YourController(DataAnalyticsMeta dataAnalyticsMeta)
{
	_dataAnalyticsMeta = dataAnalyticsMeta;
}
```
Now you can access the fingerprint value anywhere in your `Controller`
```csharp
string fingerprint = _dataAnalyticsMeta.Fingerprint
```
## Caching Events
An event is a data model that contains metadata relevant to an application operation. E.g., a `Payment`. These events are cached in memory, and ultimately uploaded in batches at regular intervals (30 second default). Events are added as they are created in your API, using the `EventMetaCache` Singleton class
```csharp
var myEvent = new MyEvent
{
    Id = "event-1",	    
};

EventMetaCache.Instance.Add(
	myEvent,
    "{brand-code}",
    "{event-name (e.g., 'create-myevent') }}",
    "{fingerprint}"
    "{URI Query string}",
    {HTTP request headers});
```
## Error Handling
Errors are handled implicitly, so that the your application process flow is not interrupted by streaming errors. You can subscribe to any thrown error
##### `EventMetaCache.Instance.AddEventMetaFailed`
> An event could not be added to the `EventMetaCache` instance
##### `EventMetaCache.Instance.GetEventMetadataPayloadBatchFailed`
> Cached events could not be removed from cache during batch-upload
##### `EventMetaCache.Instance.ClearCacheFailed`
> The event cache could not be cleared explicitly
##### `DataUploader.Instance.DataUploaderStartFailed`
> The `DataUploader` could not start
##### `DataUploader.Instance.EventMetadataUploadJobExecutionFailed`
> The batch-upload background task occurrence did not execute successfully
## Subscribing to Notifications
Your application can subscribe to any data-streaming operation
##### DataTransmissionClient.Instance.DataTransmitted
> A batch of events has just been transmitted to Google Cloud Pub/Sub
> ###### Parameters
> `numItemsTransmitted`, *int*
	> The number of items that have been transmitted
##### EventMetaCache.Instance.EventMetaAdded
> An event has just been added to the cache
> ###### Parameters
> `eventMeta`, *object* 
	> The event added to the cache 
##### EventMetaCache.Instance.GotEventMetadataPayloadBatch
> A batch of events has just been removed from the cache, and batched for upload
> ###### Parameters
> `numItemsReturned`, *int*
	> The number of events removed from the cache, and batched for upload
>	
> `numEventsCached`, *int*
	> The number of events remaining in the cache
