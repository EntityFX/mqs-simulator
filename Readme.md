# IoT Simulator

This simulator can be used to simulate JSOn telemetry with specified properties and send telemetry data by specified protocol (Http, Mqtt, Azure Iot Hub, Azure Iot Center, Azure SignalR). All telemetry properties can be defined in configuration file `appsettings.json`.

## Build

```
cd EntityFX.IotPlatform.Simulator
dotnet build -c Release
```

## Usage

```
cd EntityFX.IotPlatform.Simulator
dotnet run
```

or

```
cd EntityFX.IotPlatform.Simulator\bin\Release\net5.0
dotnet run EntityFX.IotPlatform.Simulator.dll
```

or

```
cd EntityFX.IotPlatform.Simulator\bin\Release\net5.0
./EntityFX.IotPlatform.Simulator.exe
```

To change configuration you can modify `appsettings.json` file.

### SendPeriod

Configuration:

```json
{
  "sendPeriod" :  "00:00:00.5"
}
```

> "00:00:00.5" means will send telemetry each 500 milliseconds.

`sendPeriod` can be set in any TimeStamp format.

### Properties

Property types:

* Number
* String
* Bool
* Timestamp
* DateTime
* Complex
* GeoLocation

Configuration fo properties:

```json
{
  "properties": {
    "Property1": {
      "type": "Number",
      ...
    },
    "Property2": {
      "type": "String",
      ..
    },
    ...
  }
}
```

#### Number property

Can generate numeric values by constant, enum, sequence, random.

For constant value:

```json
{
    "Temperature": {
      "type": "Number",
      "constant": 25
    },
}
```

For sequence value:

```json
{
    "Step1": {
      "type": "Number",
      "sequence": {
        "from": 5,
        "to": 9,
        "step": 2
      }
    },
}
```

`from`: inclusive

`to`: exclusive

`step`: value to increment, default is **1**.

> Will generate values like 5, 7, 5, 7, ...

For random sequence value:

```json
{
    "Step1": {
      "type": "Number",
      "randomSequence": {
        "from": 10,
        "to": 25,
        "isDouble": false
      },
      "useNull": true
    },
}
```

`from`: inclusive

`to`: exclusive

> Will generate random value in range from `10` to `25` (exclusive).
> 
> If `useNull` is true then will generate `null` randomly.
> 
> If `isDouble` is set then will generate in float format.

For enum value:

```json
{
    "Step1": {
      "type": "Number",
      "enum": [ 1, 2, 3, 5 ],
      "useNull": true,
      "random": true
    },
}
```

> Will generate values like 1, 2, 3, 5, 1, 2, 3

or random from list if `random` is true

If `useNull` is true then will generate `null` then `1, 2, 3, 5, 1, 2, 3` or random value from the list.

#### String property

Can generate string values by constant, enum abd guid.

For constant value:

```json
{
    "Name": {
      "type": "String",
      "constant": "John Doe"
    },
}
```

For enum value:

```json
{
    "Step1": {
      "type": "Number",
      "enum": [ "good", "warning", "critical" ],
      "useNull": true,
      "random": true
    },
}
```

> Will generate values like `good`, `warning`, `critical` or random from the list if `random` is true.
> 
> If `useNull` is true then will generate `null`

For guid value:

```json
{
    "Step1": {
      "type": "Number",
      "enum": [ "good", "warning", "critical" ],
      "useNull": true,
      "random": true
    },
}
```

#### Bool property

Can generate bool values by constant, random.

For constant value:

```json
{
    "IsOn": {
      "type": "Bool",
      "constant": true
    },
}
```

For random value:

```json
{
    "IsOpened": {
      "type": "Bool",
      "useNull": true,
      "random": true
    },
}
```

> Will generate random values`random` is true. Otherwise will generate sequential `false`, `true`, `false`, `true`.
> 
> If `useNull` is true then will generate `null`

#### Timestamp property

Can generate timestamp (numeric value in milliseconds since January 01 1970) by constant, enum, sequence, random or current date.

For Curent timestamp:

```json
{
    "_ts": {
      "type": "Timestamp",
      "dateType": "now"
    },
}
```

or

```json
{
    "_ts": {
      "type": "Timestamp",
      "dateType": "utcNow"
    },
}
```

For sequence value:

```json
{
    "Step1": {
      "type": "Timestamp",
      "sequence": {
        "from": 1620025640642,
        "to": 1621025640642,
        "step": 1000
      }
    },
}
```

`from`: inclusive

`to`: exclusive

`step`: value to increment, default is **1**.

For random sequence value:

```json
{
    "Step1": {
      "type": "Timestamp",
      "randomSequence": {
        "from": 1620025640642,
        "to": 1621025640642
      },
      "useNull": true
    },
}
```

`from`: inclusive

`to`: exclusive

> Will generate random value in range.
> 
> If `useNull` is true then will generate `null` randomly.

For enum value:

```json
{
    "Step1": {
      "type": "Timestamp",
      "enum": [ 1620025640642, 1620025990642 ],
      "useNull": true,
      "random": true
    },
}
```

> Will generate random from list if `random` is true

If `useNull` is true then will generate `null` then `1, 2, 3, 5, 1, 2, 3` or random value from the list.

#### DateTime property

Can generate DatTime (string date Time value in ISO format) by constant, enum, sequence, random or current date.

For Curent datettime:

```json
{
    "Dt": {
      "type": "DateTime",
      "dateType": "now"
    },
}
```

or

```json
{
    "Dt": {
      "type": "DateTime",
      "dateType": "utcNow"
    },
}

For Sequence datettime:

```json
{
    "Dt1": {
      "type": "DateTime",
      "sequence": {
        "from": "2000-01-01",
        "to": "2030-01-01"
      },
      "random": true
    },
}

or

```json
{
    "Dt1": {
      "type": "DateTime",
      "sequence": {
        "from": "2000-01-01",
        "to": "2030-01-01",
        "step" : "01:00:00"
      }
    },
}

> Sequentially increases time by 1 hour.

#### GeoLocation

Generates geolocation data:

```json
{
    "lat": 55.3683346539588,
    "lon": 51
}
```

Configuraiton (latitude and longitude will be generated randomly and range can be restricted by latRandomSequence and lonRandomSequence):

```json
{
    "Location": {
      "type": "GeoLocation",
      "latRandomSequence": {
        "from": 55,
        "to": 57,
        "isDouble": true
      },
      "lonRandomSequence": {
        "from": 47,
        "to": 52,
        "isDouble": false
      }
    }
}

#### Complex type


```
### Telemetry Sender

Telemetry Sender Types:

* Http
* Mqtt
* Azure Iot Hub
* Azure Iot Center
* Azure SignalR

Sender Type configuration:

```json
{
  "telemetrySender": {
    "_types": [ "Http", "Mqtt", "AzureSignalR", "AzureIotHub", "AzureIotCenter" ],
    "type": "AzureIotHub"
  }
}
```

#### Http Sender

Http sender configuration example:

```json
{
  "telemetrySender": {
    "Http": {
      "Path": "https://your.uri",
      "Method": "Post",
      "RequestHeaders": {
        "x-functions-key": "some-key-here",
        "Auth": "Bearer ..."
      }
    }
  }
}
```

`Method`: `Post` or `Put`

`RequestHeaders` can contain any `"key": "value"` property.

#### AzureSignalR

Configuration:

```json
{
  "telemetrySender": {
    "AzureSignalR": {
      "Hub": "telemetry",
      "Method": "Send",
      "ConnectionStringName": "AzureSignalR"
    },
  },
  "connectionStrings": {
    "AzureSignalR": "Endpoint=https://your.uri;AccessKey=...;Version=1.0;"
  }
}
```

`Hub`: SignalR hub name

`Method`: Remote method to invoke (only support one parameter method with object type)

`ConnectionStringName`: SignalR Connection string which should be defined in `connectionStrings` section

#### AzureIotHub

Configuration to send telemetry to Azure Iot Hub.

Configuration:

```json
{
  "telemetrySender": {
    "AzureIotHub": {
      "HostName": "almaz-gdc.azure-devices.net",
      "SymmetricKeys": {
        "deviceId1": "sas-token"
      }
    }
  }
}
```

`HostName`: uri address of Azure Iot Hub

`SymmetricKeys`: `"key": "value"` properties, where `key` is Iot Hub device id and `value` is Iot Hub's device sas-token. You can use multiple Symmetric Keys but only one random key will be used per each telemetry send call.

#### AzureIotCenter

AzureIotCenter configuration is used when you send telemetry to Azure IoT central with unknown address of it's IoT Hub.

```json
{
  "telemetrySender": {
    "AzureIotCenter": {
      "ProvisioningHost": "global.azure-devices-provisioning.net",
      "IdScope": "your-id-scope",
      "SymmetricKeys": {
        "deviceId1": "sas-token"
      }
    }
  }
}
```

`ProvisioningHost`: Iot Center provisioning host.

`IdScope`: unique id of your Iot Central application


`SymmetricKeys`: `"key": "value"` properties, where `key` is Iot Hub device id and `value` is Iot Hub's device sas-token. You can use multiple Symmetric Keys but only one random key will be used per each telemetry send call.
