# OES Smart Plug Simulator

A straightforward simulator that emulates a fictitious OES Smart Plug device using the UDP protocol. This simulator is included as part of the [sample codes](https://github.com/openenergysolutions/openfmb.adapters.ext), which serve as demonstrations of how to leverage the OpenFMB Adapter Framework to enhance support for various communication protocols.

## Configuration

```json
{
    "Settings": {
        "FilePath": "/config/config.json",
        "Name": "UDP-Plug",
        "MacAddress": "0a81d60c2233",
        "HostIpAddress": "0.0.0.0",
        "BroadcastIntervalMs": 1000,
        "Port": 8556,
        "Voltage": 120,
        "Current": 5,
        "Power": 193
    }
}
Where: FilePath is path to this config.json file
```

## Build Docker Image

```
docker compose build
```

## Run Docker

```
docker compose up
```
