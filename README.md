[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://ci.appveyor.com/project/daishisystems/strada)
[![NuGet](https://img.shields.io/badge/myget-v1.5.7-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)
## Overview
The **Data Analytics Transmission Component** is a .NET Standard 2.0 library that transmits data to [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/).

## Purpose
To transmit data to a Data Lake to facilitate reporting services, and to derive business intelligence.

## Usage
### Initialisation
Initialisation should be executed once, during your **_application start-up_** phase.
```
DataTransmissionClient.Instance.Init(
    "GCP PROJECT ID",
    "GCP TOPIC ID,
    "GCP SERVICE CREDENTIALS FILE LOCATION");
```
### Transmission
The transmission mechanism accepts a generic payload, allowing clients to transmit any class instance.
```
await DataTransmissionClient.Instance.TransmitAsync(
    "BRANDNAME", // E.g., "NKE", "NIKE". Injected by Autofac
    "CORRELATIONID", // Unique ID. Injected by Autofac. Currently unsupported.
    new PreOrder
    {
        ProductName = "SNKRS",
        ProductValue = 1.5
    });
```
### Shutdown
Shutdown should be called once, during your application shutdown phase. **WARNING**: If your application executes a **_shutdown_** phase during, for example, Application Pool recycling, you must ensure that the ``` await DataTransmissionClient.Instance.Init``` method is executed in a subsequent **_start-up_** phase.
```
await DataTransmissionClient.ShutDownAsync();
```
## Overhead
Crumple zones are built in, so that in case of network failure, or any other issue that results in an unacceptable delay (configurable; default 3 seconds), the transmission request will abort, ensuring a consistent minimal overhead.
