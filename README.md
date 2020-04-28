# Azure IoT Edge Unit Testing

This is a unit testing Azure IoT Edge reference implementation that demonstrates how we can abstract the Azure IoT Edge SDK and define unit tests to test our business code.

You'll need to have .NET Core SDK (>= 3.1) to run this reference application.

> This repo has been presented and analysed in this blog post : http://havedatawilltrain.com/the-beloved-mans-objection/

## Running the application

To run this unit test:

1. Clone this repo:

``` bash
   git clone https://github.com/paloukari/AzureIoTEdgeUnitTesting
```

1. Build and run the xunit test

``` bash
   cd AzureIoTEdgeUnitTesting
   cd IoTModuleTest

   dotnet test
```
