# UDP Plug Simulator

A simple Plug Simulator that utilizes UDP protocol

## Configuration

```json
{
    "Settings": {
        "FilePath": "/config/config.json",
        "Name": "UDP-Plug",
        "MacAddress": "0a81d60c2233",
        "HostIpAddress": "0.0.0.0",
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
