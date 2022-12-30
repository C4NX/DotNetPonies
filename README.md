<div align="center">
    <img src="images/logo.png">
    <p>A DotNet Standard C# <a href="https://pony.town/">PonyTown</a> API Wrapper</p>
</div>


## Getting Started
- Install the library from [Nuget](#) or this repository

- Retrieving the servers status
```csharp
using DotNetPonies;

var client = new PonyTownClient();
var status = await client.GetStatus();

status.Version // The pony town game version
status.Servers // All the servers status (name, online)
status.Servers[.].Id // Server ID
status.Servers[.].Count // Player count
```

## Issues

###  PonyTownException: Invalid game version

PonyTown API V2 is using a **api-version** header to validate a lot of request, in this case the api-version is not valid anymore for this build, it will be necessary to get it manually.

- First you need to go on [pony town](https://pony.town/) website and login.
- Open the chrome/firefox dev tool with F12.
- Go on the network tab
![](images/001.png)
- Reload your page
- Go on the https://pony.town/api1/account ressource (or https://pony.town/api2/game/status if you have clicked on the server selector)
![](images/002.png)
- Get the **api-version** header
- Create your client with this api version
```csharp
var client = new PonyTownClient("<api-version-here>");
```