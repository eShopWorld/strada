[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://ci.appveyor.com/project/daishisystems/strada)
[![NuGet](https://img.shields.io/badge/myget-v1.6.4-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)
## Overview
The **Data Analytics Transmission Component** is a [.NET Standard 2.0](https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/) library that transmits data to [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/).

## Purpose
To transmit data models to a Data Lake, facilitating reporting services and business intelligence.

## Usage
### Initialisation
Initialisation should be executed once, during your **_application start-up_** phase
```cs
DataTransmissionClient.Instance.Init(
    "GCP PROJECT ID",
    "GCP TOPIC ID",
    "GCP SERVICE CREDENTIALS");
```
Note that the this code initialises the library as a [Singleton](http://csharpindepth.com/Articles/General/Singleton.aspx) instance. It can also be initialised as a transient instance
```
var dataTransmissionClient = new DataTransmissionClient();

dataTransmissionClient.Init(
    "GCP PROJECT ID",
    "GCP TOPIC ID",
    "GCP SERVICE CREDENTIALS");
```
### Authentication
Authentication is facilitated through a JSON-formatted [Service Credentials file](https://cloud.google.com/docs/authentication/). This file contains metadata pertaining to a **Google Cloud Service Account** with sufficient access to **Cloud Pub/Sub**. You can include the file itself, as raw text, when [initialising](https://github.com/eShopWorld/strada/blob/master/README.md#initialisation)
```cs
string serviceCredentials;
using (var client = new HttpClient())
{
    serviceCredentials = client.GetStringAsync(Resources.CredentialsFileUri).Result;
}
var dataTransmissionClient = new DataTransmissionClient();
dataTransmissionClient.Init("GCP PROJECT ID", "GCP TOPIC ID", serviceCredentials);
```
or you can instantiate a ```ServiceCredentials``` instance, passing it to the ```Init``` method
```cs
var serviceCredentials = new ServiceCredentials
{
    ProjectId = "GCP PROJECT ID",
    ClientId = "MY CLIENT ID"
    etc ...
};
var dataTransmissionClient = new DataTransmissionClient();
dataTransmissionClient.Init("GCP PROJECT ID", "GCP TOPIC ID", serviceCredentials);
```
### Transmission
The transmission mechanism accepts a generic payload, allowing clients to transmit any class instance
```cs
await DataTransmissionClient.Instance.TransmitAsync(
    "BRANDCODE", // E.g., "ESW". Contained within a custom HTTP header (at time of writing)
    "CORRELATIONID", // Unique ID transmitted by a custom JavaScript component (correlation.js)
    new PreOrder // The class instance to be transmitted
    {
        ProductName = "SNKRS",
        ProductValue = 1.5
    });
```
Or, simply load an already-serialised JSON payload
```cs
await DataTransmissionClient.Instance.TransmitAsync(
    "BRANDCODE", // E.g., "ESW". Contained within a custom HTTP header (at time of writing)
    "CORRELATIONID", // Unique ID transmitted by a custom JavaScript component (correlation.js)
    "[My serialised JSON]"); // The serialised class instance to be transmitted
```
### Shutdown
Shutdown should be called once, during your application **_shutdown_** phase. **WARNING**: If your application executes a shutdown phase as part of a **_restart_** operation during, for example, Application Pool recycling, you must ensure that [initialisation](https://github.com/eShopWorld/strada/blob/master/README.md#initialisation) occurs in the subsequent **_start-up_** phase.
```cs
await DataTransmissionClient.ShutDownAsync();
```
## Exception Handling
Exceptions are swallowed by default. Your app can subscribe to swallowed exceptions as follows
```
var dataTransmissionClient = new DataTransmissionClient();
dataTransmissionClient.InitialisationFailed += DataTransmissionClient_InitialisationFailed;
dataTransmissionClient.Init("GCP PROJECT ID", "GCP TOPIC ID", "GCP SERVICE CREDENTIALS");

private static void DataTransmissionClient_InitialisationFailed(object sender, InitialisationFailedEventArgs e)
{
    // todo: Log the exception ...
}
```
Alternatively, you can configure each method to throw any exceptions in the traditional manner
```
var dataTransmissionClient = new DataTransmissionClient();
try
{
    dataTransmissionClient.Init("GCP PROJECT ID", "GCP TOPIC ID", "GCP SERVICE CREDENTIALS", false);
}
catch (Exception e)
{
    // todo: Log the exception ...
}
```
## Overhead
Crumple zones are built in, so that in case of network failure, or any other issue that results in an unacceptable delay (configurable; default 3 seconds), the transmission request will abort, ensuring consistent, minimal overhead.

## Correlating Transmissions
Data models transmitted to Cloud Pub/Sub are tagged with a unique correlation-id, specific to each user's web browser. This correlation-id is persisted across all HTTP requests and remains intact for the duration of the user's session, and for an indeterminate (at time of writing) length of time thereafter. The correlation-id is generated 3 seconds after loading the following script
```js
<script language="javascript" src="correlation.js"></script>
```
Once set, the correlation-id is retrieved on the server-side API
```cs
[HttpGet]
public string Get()
{
    var gotCorrelationId = _httpRequestFunctions
        .TryGetCorrelationId(Request, "correlationid", out var correlationId);
    return gotCorrelationId ? correlationId : string.Empty;
}
```
Finally, the correlation-id is set as input parameter when transmitting the data model
```cs
await DataTransmissionClient.Instance.TransmitAsync(
    "BRANDCODE",
    correlationId, 
    new PreOrder
    {
        ProductName = "SNKRS",
        ProductValue = 1.5
    });
```
Once transmitted, data models that share the same correlation-id are aggregated to facilitate reporting and to accurately calculate order-conversion.