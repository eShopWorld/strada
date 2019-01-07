[![Build status](https://ci.appveyor.com/api/projects/status/ly3h4f406u5332n3?svg=true)](https://ci.appveyor.com/project/daishisystems/strada)
[![NuGet](https://img.shields.io/badge/myget-v1.8.3-blue.svg)](https://eshopworld.myget.org/feed/github-dev/package/nuget/Eshopworld.Strada.Plugins.Streaming)
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
Once transmitted, data models that share the same correlation-id are aggregated to facilitate reporting and to accurately calculate order-conversion. Click [here](https://github.com/eShopWorld/strada/tree/master/src/Eshopworld.Strada.Web) to view the sample [ASP.NET Core 2.0](https://docs.microsoft.com/en-us/aspnet/core/getting-started/?view=aspnetcore-2.1&tabs=windows) implementation.

## ASP.NET Core Sample App
The [sample app](https://github.com/eShopWorld/strada/tree/master/src/Eshopworld.Strada.Web) is an end-to-end solution that begins with the generation of a correlation-id at the UI layer. The correlation-id is bound to a simulated Order, which is persisted through the application API until it reaches a simulated data repository. At this point, the Order is transmitted to GCP Pub/Sub.

### Step 1  Create simulated ESW domain models
These classes are simplified domain models used as example
```cs
public class Order
{
    public string Number { get; set; }
    public decimal Value { get; set; }
}

public class OrderRepository
{    
    public bool Save(Order order)
    {
        return true;
    }
}
```
### Step 2  Create simulated Domain-Service layer
This class is a simulated service layer that controls database-persistence and data-transmission
```cs
public class DomainServiceLayer
{
    private readonly DataAnalyticsMeta _dataAnalyticsMeta;
    private readonly DataTransmissionClient _dataTransmissionClient;
    private readonly OrderRepository _orderRepository;

    public DomainServiceLayer(
        OrderRepository orderRepository,
        DataAnalyticsMeta dataAnalyticsMeta,
        DataTransmissionClient dataTransmissionClient)
    {
        _orderRepository = orderRepository;
        _dataAnalyticsMeta = dataAnalyticsMeta;
        _dataTransmissionClient = dataTransmissionClient;
    }

    public async Task SaveOrder(Order order)
    {
        var orderSaved = _orderRepository.Save(order);

        if (orderSaved)
            await _dataTransmissionClient.TransmitAsync("MAX", _dataAnalyticsMeta.CorrelationId, order);
        else
            throw new DataException("Something went wrong while saving the order.");
    }
}
```
### Step 3  Get and retain the correlation-id
The correlation-id is generated at the UI layer. Add the following markup to generate the correlation-id
```js
<script language="javascript" src="correlation.js"></script>
```
*Note* Add this toward the end of the HTML page to ensure that the DOM initialisation process is not unduly interrupted

Add a class to retain the correlation-id value as it progresses through the HTTP request pipeline
```cs
public class DataAnalyticsMeta
{
    public string CorrelationId { get; set; }
}
```
### Step 4  Add custom services to the Dependency Injection pipeline
The following service instances exist within the scope of the HTTP request
```cs
services.AddScoped<DataAnalyticsMeta>();
services.AddScoped<OrderRepository>();
services.AddScoped<DomainServiceLayer>();
```
The ```DataTransmissionClient``` instance must remain intact, as a [Singleton](http://csharpindepth.com/Articles/General/Singleton.aspx) instance, throughout the application lifecycle in order to maintain a persistent connection with GCP Pub/Sub
```cs
services.AddSingleton<DataTransmissionClient>();
```
### Step 5  Initialise the connection to GCP Pub/Sub
Instantiate a Singleton ```DataTransmissionClient``` instance and load GCP Pub/Sub credentials from ```appsettings.json```
```cs
var dataTransmissionClient = app.ApplicationServices.GetService<DataTransmissionClient>();

var gcpServiceCredentials = new GcpServiceCredentials();
Configuration.GetSection("gcpServiceCredentials").Bind(gcpServiceCredentials);
dataTransmissionClient.Init("eshop-bigdata", "checkout-dev", gcpServiceCredentials);
```
### Step 6  Subscribe to error events
Attach the following methods to the ```DataTransmissionClient``` instance in order to process errors
```cs
dataTransmissionClient.InitialisationFailed += DataTransmissionClient_InitialisationFailed;
dataTransmissionClient.TransmissionFailed += DataTransmissionClient_TransmissionFailed;

private static void DataTransmissionClient_InitialisationFailed(object sender, InitialisationFailedEventArgs e)
{
    Console.WriteLine(e.Exception.Message);
    throw new Exception("Start-up error.", e.Exception);
}

private static void DataTransmissionClient_TransmissionFailed(object sender, TransmissionFailedEventArgs e)
{
    Console.WriteLine(e.Exception.Message);
    throw new Exception("Start-up error.", e.Exception);
}
```
### Step 7  Read the correlation-id and persist through the HTTP request pipeline
The following middleware reads the correlatation-id from a custom HTTP header, and loads the value to a scoped ```DataAnalyticsMeta``` instance
```cs
public class DataAnalyticsMiddleware
{
    private readonly RequestDelegate _next;

    public DataAnalyticsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, DataAnalyticsMeta dataAnalyticsMeta)
    {
        try
        {                
            dataAnalyticsMeta.CorrelationId = GetCorrelationId(context.Request);
        }
        catch (Exception exception)
        {
            await context.Response.WriteAsync(exception.Message);
        }

        await _next(context);
    }
   
    private string GetCorrelationId(HttpRequest httpRequest, string correlationIdHeaderName = "correlationid")
    {
        if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

        var gotCorrelationId = httpRequest.Headers.TryGetValue(correlationIdHeaderName, out var headerValues);
        var correlationId =
            headerValues.LastOrDefault();

        return gotCorrelationId ? correlationId : null;
    }
}
```
### Step 8  Transmit an Order instance to GCP Pub/Sub after persisting to DB
The ```DomainServiceLayer``` instance simulates the transmission of an Order to DB, after which the Order is transmitted to GCP Pub/Sub
```cs
var order = new Order
{
    Number = Guid.NewGuid().ToString(),
    Value = 10.00m
};
await _domainServiceLayer.SaveOrder(order);
```
