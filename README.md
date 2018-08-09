## Overview
The Data Analytics Transmission Component is a .NET Standard 2.0 library that transmits data to Google Cloud Pub/Sub.

## Purpose
Transmitted data is ultimately stored in a Data Lake. Stored data is analysed to facilitate reporting services and derive business intelligence.

## Usage
The transmission mechanism accepts a generic payload, allowing clients to transmit any .NET object. Transmission introduces a nominal sub-second I/O latency to client applications.

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
Crumple zones are built in, so that in case of network failure, or any other issue that results in an unacceptable delay (configurable; default 3 seconds) the transmission request will abort, ensuring a consistent minimal overhead.
