### ET-Dotnet-API
This is simple RESTful API to store and serve weather metrics. It stores weather metrics data by secure M2M authentication.
See full API documentation by accessing `/swagger` endpoint. This project was created for my Engineering Thesis purposes and this is rather a showdown of my abilities than a real production solution. Feel free to contribute or suggest some tips. Have a good day :)

![example workflow](https://github.com/tkfalarz/ET-Dotnet-API/actions/workflows/tests.yml/badge.svg)

**Prerequisites**
- .NET 5 or higher
- dotnet ef tools https://docs.microsoft.com/en-us/ef/core/cli/dotnet
- Fresh SQL Database
- Environment variable `SQL_CONNECTION_STRING` set
- database migrated

### How to migrate the database

1. Go to the `/Database` directory
2. Open a terminal and execute `dotnet ef database update`


**List of endpoints**
|Method | Path | Description |
|-|-|-|
| GET  | `api/Devices` | Get the list of available devices |
| POST | `api/Devices` | Upload the new device in the body |
| GET  | `api/Devices/{DeviceName}` | Get the particular device info |
| GET  | `api/Devices/{DeviceName}/Readings` | Get the readings of particular device |
| GET  | `api/Devices/{DeviceName}/Readings/Latest` | Get the latest weather readings of particular device |
| GET  | `api/Devices/{DeviceName}/Readings/{ReadingType}` | Get the particular weather readings of particular device |
| GET  | `api/Devices/{DeviceName}/Readings/{ReadingType}/Latest` | Get the latest particular weather reading of particular device |
| POST | `api/Readings` | Upload readings to the API |
| GET  | `api/Readings/Nearest?latitude=x&longitude=y` | Get the latest readings from the nearest device |
