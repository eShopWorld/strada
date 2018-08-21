[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://ci.appveyor.com/project/daishisystems/strada)
[![NuGet](https://img.shields.io/badge/myget-v1.6.2-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)
## Overview
The **Data Analytics Transmission Component** is a [.NET Standard 2.0](https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/) library that transmits data to [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/).

## Purpose
To transmit data models to a Data Lake, facilitating reporting services and business intelligence.

## Usage
### Initialisation
Initialisation should be executed once, during your **_application start-up_** phase.
```cs
DataTransmissionClient.Instance.Init(
    "GCP PROJECT ID",
    "GCP TOPIC ID",
    "GCP SERVICE CREDENTIALS");
```
### Transmission
The transmission mechanism accepts a generic payload, allowing clients to transmit any class instance
```cs
await DataTransmissionClient.Instance.TransmitAsync(
    "BRANDCODE", // E.g., "ESW". Injected by Autofac
    "CORRELATIONID", // Unique ID. Injected by Autofac. Currently PENDING implementation.
    new PreOrder // The class instance to be transmitted
    {
        ProductName = "SNKRS",
        ProductValue = 1.5
    });
```
Or, simply load an already-serialised JSON payload
```cs
await DataTransmissionClient.Instance.TransmitAsync(
    "BRANDCODE", // E.g., "ESW". Injected by Autofac
    "CORRELATIONID", // Unique ID. Injected by Autofac. Currently PENDING implementation.
    "[My serialised JSON]");
```
### Shutdown
Shutdown should be called once, during your application **_shutdown_** phase. **WARNING**: If your application executes a shutdown phase as part of a **_restart_** operation during, for example, Application Pool recycling, you must ensure that [initialisation](https://github.com/eShopWorld/strada/blob/master/README.md#initialisation) occurs in the subsequent **_start-up_** phase.
```cs
await DataTransmissionClient.ShutDownAsync();
```
## Overhead
Crumple zones are built in, so that in case of network failure, or any other issue that results in an unacceptable delay (configurable; default 3 seconds), the transmission request will abort, ensuring consistent, minimal overhead.
