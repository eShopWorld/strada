[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://ci.appveyor.com/project/daishisystems/strada)
[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)
## Overview
The Data Analytics Transmission Component is a .NET Standard 2.0 library that transmits data to Google Cloud Pub/Sub.

## Purpose
Transmitted data is ultimately stored in a Data Lake, and analysed to facilitate reporting services and to derive business intelligence.

## Usage
The transmission mechanism accepts a generic payload, allowing clients to transmit any class instance.

````
DataTransmissionClient.Instance.Init(
    "GCP PROJECT ID",
    "GCP TOPIC ID,
    "GCP SERVICE CREDENTIALS FILE");

await DataTransmissionClient.Instance.TransmitAsync(
    "BRANDNAME",
    "CORRELATIONID",
    new PreOrder
    {
        ProductName = "SNKRS",
        ProductValue = 1.5
    });

await DataTransmissionClient.ShutDownAsync();
````

## Overhead
Crumple zones are built in, so that in case of network failure, or any other issue that results in an unacceptable delay (configurable; default 3 seconds), the transmission request will abort, ensuring a consistent minimal overhead.
