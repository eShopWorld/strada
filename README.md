# Overview
[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://eshopworld.visualstudio.com/Github%20build/_build?definitionId=204&_a=summary) [![NuGet](https://img.shields.io/badge/myget-v2.6.0-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)
The Data Analytics Transmission Component (DATC) is a [.NET Standard 2.0](https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/) library, including components specific to [.NET Framework 4.6](https://en.wikipedia.org/wiki/.NET_Framework) and [.NET Core 2](https://en.wikipedia.org/wiki/.NET_Core), that transmits data from your app to [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/).  Data is transmitted asynchronously from an in-memory cache, resulting in evenly-balanced throughput, and low CPU/RAM overhead.
# Installation
## .NET Framework 4.6
[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://eshopworld.visualstudio.com/Github%20build/_build?definitionId=644&_a=summary) [![NuGet](https://img.shields.io/badge/myget-v2.2.0-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming.AspNet)
Install the `Strada Core` NuGet package
`Install-Package Eshopworld.Strada.Plugins.Streaming`

and the `ASP.NET` NuGet package
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
	JsonConvert.DeserializeObject<CloudServiceCredentials>(GcpServiceCredentials);
```
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
The Google Cloud Project to which the streaming component will connect
##### TopicID
The Google Cloud Pub/Sub Topic to which the streaming component will transmit data
Deserialize these configuration settings in the `Application_Start` method of your `Global.asax.cs` file
```csharp
var dataTransmissionClientConfigSettings =
	JsonConvert.DeserializeObject<DataTransmissionClientConfigSettings>(
		DataTransmissionClientConfigSettings);
```

// rest of startup method
// background job [max threads]
// Subscriptions, error-handling, AI publishing
// .NET Core

## .NET Core 2
[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://eshopworld.visualstudio.com/Github%20build/_build?definitionId=645&_a=summary) [![NuGet](https://img.shields.io/badge/myget-v2.1.8-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming.NetCore)
---