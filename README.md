




# Overview
[![MyGet](https://img.shields.io/badge/myget-v1.0.0-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Strada.Api)
[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-blue.svg)](https://www.nuget.org/packages/Strada.Api/)

High-throughput, low-overhead Google Cloud data publisher
## Installation
Install the Strada NuGet package
`install Strada.Api`
### Authentication
A `CloudServiceCredentials` instance is necessary to establish a persistent connection with Google Cloud. Authentication meta is stored in JSON format
```json
{
	"Type": "service_account",
	"project_id": "{Project ID}",
	"private_key_id": "{Your private key ID}",
	"private_key": "{Your private key}",
	"client_email": "{Your custom email address}",
	"client_id": "{Your client ID}",
	"auth_uri": "https://accounts.google.com/o/oauth2/auth",
	"token_uri": "https://oauth2.googleapis.com/token",
	"auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
	"client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/deploy%40eshop-puddle.iam.gserviceaccount.com"
}
```
Deserialize the credentials
```csharp
var cloudServiceCredentials = JsonConvert.DeserializeObject<CloudServiceCredentials>(Resources.CloudServiceCredentials);
```
Note, this examples assumes that the `CloudServiceCredentials` JSON values are stored in a local `Resource` file
### Configuration
A `ClientConfigSettings` instance is necessary to configure the Strada API. Configuration meta is stored in JSON format 
```json
{
	"ProjectId": "{Project ID}",
	"PublisherTopicId": "{Publisher Topic ID}"	
}
```
Deserialize the configuration settings
```csharp
var clientConfigSettings = JsonConvert.DeserializeObject<ClientConfigSettings>(Resource.ClientConfigSettings);
```
Note, this examples assumes that the `ClientConfigSettings` JSON values are stored in a local `Resource` file

### Initialisation
Starting the `Agent` establishes a background process that publishes data to Google Cloud
```csharp
Agent.Instance.Start(cloudServiceCredentials, clientConfigSettings);
```
## Usage
### Publishing Events
An event is a generic data model that contains metadata relevant to your application, e.g., a `Create Order`. These events are cached in memory, and uploaded in batches at regular intervals. Add events as they occur in your application as follows
```csharp
	EventMetadataCache.Instance.Add(
	eventMetadata,
	"{brand-code}",
	"{event-name}",
	"{fingerprint}",
	"{HTTP query string}",
	"{HTTP headers}");
```
### Error Handling
Errors are handled implicitly, so that the your application process flow is not interrupted. You can subscribe to the following errors
##### `Agent.Instance.AddEventMetaFailed`
> An event could not be added to the cache
> ###### Parameters
> `Exception`, *Exception*

> *The `Exception` instance that raised the event*
##### `Agent.Instance.ClearCacheFailed`
> The cache could not be cleared manually
> ###### Parameters
> `Exception`, *Exception*

> *The `Exception` instance that raised the event*
##### `Agent.Instance.InitialisationFailed`
> The Cloud database connection could not be established
> ###### Parameters
> `Exception`, *Exception*

> *The `Exception` instance that raised the event* 
##### `Agent.Instance.TransmissionFailed`
> Events could not be published to Google Cloud
> ###### Parameters
> `Exception`, *Exception*

> *The `Exception` instance that raised the event*
##### `Agent.Instance.GetEventMetadataPayloadBatchFailed`
> Unable to prepare the event batch for publishing to Google cloud
> ###### Parameters
> `NumEventsCached`, *int*

> *The number of events remaining in the cache*

> `Exception`, *Exception*

> *The `Exception` instance that raised the event*
##### `Agent.Instance.InitialisationFailed`
> The event transmission service could not be started
> ###### Parameters
> `Exception`, *Exception*

> *The `Exception` instance that raised the event*
##### `Agent.Instance.GotEventMetadataPayloadBatch`
> A batch of events has been removed from the cache
> ###### Parameters
> `NumItemsReturned`, *int*

> *The number of items removed from the cache*

> `NumEventsCached`, *int*

> *The number of items remaining in the cache*
### Subscribing to Notifications
Your application can subscribe to any successful operation
##### `Agent.Instance.EventMetaAdded`
> An event has been added to the cache
> ###### Parameters
> `EventMeta`, *object*

> *The event that has been added to the cache*
##### `Agent.Instance.DataTransmitted`
> A batch of events has been transmitted to the Botworks Cloud
> ###### Parameters
> `NumItemsTransmitted`, *int*

> *The number of events transmitted in the batch*
##### `Agent.Instance.GotEventMetadataPayloadBatch`
> A batch of events has been removed from the cache
> ###### Parameters
> `NumItemsReturned`, *int*

> *The number of items removed from the cache*

> `NumEventsCached`, *int*

> *The number of items remaining in the cache*