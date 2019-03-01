
[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://ci.appveyor.com/project/daishisystems/strada)
[![NuGet](https://img.shields.io/badge/myget-v1.9.8-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)
# Overview
The Data Analytics Transmission Component (DATC) is a [.NET Standard 2.0](https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/) library that transmits data from your app to [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/).  Data is transmitted asynchronously from an in-memory cache, resulting in evenly-balanced throughput, and low CPU/RAM overhead.

There are 2 modes of operation: implicit,  and explicit. In implicit mode, events are automatically written to a backing cache, before being uploaded in batches. This results in a latency measured in single-digit milliseconds. In explicit mode, events are uploaded on-demand, by executing an upload function inside your API Controller. This results in a latency measured in 2|3-digit milliseconds.
## Data Flow
### Implicit Mode
Data models, based on your `DataTransmissionClientConfigSettings` [configuration](####Configuration) are automatically cached in memory and published to GCP at regular intervals without the need to explicitly cache the data models

<a href="https://bit.ly/2H8arim">![Data Flow Implicit Mode](https://bit.ly/2H8arim)</a>
### Explicit Mode
Data models must be explicitly cached in memory in order to be published to GCP

<a href="https://bit.ly/2GTZdyp">![Data Flow Explicit Mode](https://bit.ly/2GTZdyp)</a>
# Installation
## .NET Framework 4.6
Install the `Strada Core` NuGet package
`Install-Package Eshopworld.Strada.Plugins.Streaming -Version 1.9.8 -Source`

and the `ASP.NET` NuGet package
`Install-Package Eshopworld.Strada.Plugins.Streaming -Version 1.9.8 -Source`

### Implementation
#### Dependency Injection
Register the `DataTransmissionClient` class as a [Singleton](http://csharpindepth.com/Articles/Singleton) instance with your DI provider in your `Application_Start` method. This example assumes that you are leveraging [AutoFac](https://autofac.org/).
```
builder.RegisterType<DataTransmissionClient>()
	.AsSelf()
	.SingleInstance();
```
#### Authentication
We will issue a custom `GcpServiceCredentials` file with the following JSON properties
```
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
Reference and bind the `GcpServiceCredentials` file in your `Application_Start` method
```
var gcpServiceCredentials =
                JsonConvert.DeserializeObject<GcpServiceCredentials>(Resources.GcpServiceCredentials);
```
#### Configuration
We will issue a custom `DataTransmissionClientConfigSettings` file with the following JSON properties
```
{
	"ProjectId": "{GCP Project ID}",
	"TopicId": "{GCP Pub/Sub Topic ID}",
	"UriSegmentMeta": [{
		"UriSegmentName": "{}",
		"AllowedHttpMethods": ["POST", "GET"]
	}, {
		"UriSegmentName": "{as above}",
		"AllowedHttpMethods": ["{as above}"]
	}],
	"AllowedHttpHeaders": ["user-agent", "accept"],
	"SwallowExceptions": true,
	"BatchMode": true,
	"ElementCountThreshold": 1000,
	"DelayThreshold": 3
}
```
#### Configuration Explained
The configuration file determines where and how data models are intercepted and uploaded. Below are explanations of each configuration property

##### UriSegmentName
URI segment that you would like to intercept. E.g., given the following URI: `http://www.myapp/api/accounts?id=1`, setting `UriSegmentName` to "_accounts_" allows the component to intercept HTTP calls to the _accounts_ URI segment. This property applies to [implicit](#implicit-mode) mode only.

##### AllowedHttpMethods
HTTP methods associated with `UriSegmentName` that you would like to intercept. E.g., specifying a value of `POST`, associated with `UriSegmentName` _accounts_,  will intercept all HTTP POST calls to the _accounts_ URI segment. You must individually specify each HTTP method that you would like to intercept. This property applies to [implicit](#implicit-mode) mode only.

##### AllowedHttpHeaders
HTTP headers that you would like to read on data model interception. These headers will be persisted to GCP along with the data model.

##### SwallowExceptions
Indicates that Exceptions should not break the call chain, and instead should be raised as custom events, to which your application can subscribe. E.g., `DataTransmissionClient.InitialisationFailed` and `TransmissionFailed`. Setting this property to `false` results in any thrown exceptions breaking the call chain. In this instance, you need to wrap the executing method in a try-catch block.

##### BatchMode
Set to `true` by default, `BatchMode` publishes cached data models to GCP in batches.

##### ElementCountThreshold
Cached data models will be published as a batch when the number of cached data models is equal to or greater than this value.

##### DelayThreshold
Cached data models will be published as a batch when the number of elapsed seconds is equal to or greater than this value.
#### Initialization

Reference and bind the `DataTransmissionClientConfigSettings` file in your `Application_Start` method
```
var dataTransmissionClientConfigSettings =
	JsonConvert.DeserializeObject<DataTransmissionClientConfigSettings>(
		Resources.DataTransmissionClientConfigSettings);
```
Resolve a reference to your `DataTransmissionClient` [Singleton](http://csharpindepth.com/Articles/Singleton) instance
```
var dataTransmissionClient = container.Resolve<DataTransmissionClient>();
```
This example assumes that you are leveraging [AutoFac](https://autofac.org/).

Initialize your `DataTransmissionClient` instance using your [Authentication](#authentication
) and [Configuration](#configuration) settings
```
dataTransmissionClient.InitAsync(
	gcpServiceCredentials,
	dataTransmissionClientConfigSettings
).Wait();
```
Start the `DataUploader` background task at the required interval. This runs a single-threaded background process that uploads data batches every 30 seconds
```
DataUploader.Start(dataTransmissionClient, 30);
```
Register a `DataTransmissionHandler` instance with your `WebApiConfig` class. This applies to [implicit mode](#implicit-mode) only
```
config.MessageHandlers.Add(new DataTransmissionHandler());
```
Data models will be automatically cached and published in [implicit](#implicit-mode) mode. [Explicitly](#explicit-mode) cache your data model by referencing the model in a `HttpRequestMeta` instance
```
var myDataModel = new MyDataModel();
var httpRequestMeta = new HttpRequestMeta
{
    Uri = request.RequestUri,
    Body = myDataModel,
    HttpRequestHeaders = request.Headers
        .Where(header => UriMetaCache
            .Instance
            .AllowedHttpHeaders
            .Contains(header.Key.ToLowerInvariant()))
        .ToList()
};
```
