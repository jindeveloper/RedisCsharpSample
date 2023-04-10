using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RedisGetandSetSampleCsharp;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var services = builder.Services;

services.AddSingleton<IConnectionMultiplexer>(option =>
    ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" },
        AbortOnConnectFail = false,
        Ssl = false,
        Password = ""
        
    }));

var app = builder.Build();

app.MapGet("/keys/all" ,async (IConnectionMultiplexer connection) =>
{
    var keys =  (connection.GetServer("localhost", 6379)).Keys();
    var list = new List<string>();
    list.AddRange(keys.Select(key => (string)key).ToList());
    return new { result =  list };

});

app.MapGet("/keys/{key}", async
    (IConnectionMultiplexer connection, string key) =>
{
    var redis = connection.GetDatabase();
    var result = await redis.KeyExistsAsync(key);

    return new { Message = result ? "Key exists" : "Key doesn't exists" };
});

app.MapGet("keys/GetValue", async (IConnectionMultiplexer connection, [FromQuery] string key) =>
{
    var redis = connection.GetDatabase();
    var value = (string)await redis.StringGetAsync(key);
    return new { Message = value };
});

app.MapPost("/keys/AddKeyValue",
    async (IConnectionMultiplexer connection, [FromBody]KeyValue keyValue) =>
{
    var redis = connection.GetDatabase();
    
    var result = await redis.StringSetAsync(keyValue.Key,  keyValue.Value);

    return new { Message = result ? "Added successfully" : $"Unable to add" };
});



app.Run();