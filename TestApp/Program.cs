// See https://aka.ms/new-console-template for more information

using DotNetPonies;

var client = new PonyTownClient();
await client.ResolveApiVersionAsync();

var status = await client.GetStatusAsync();

Console.WriteLine(status);