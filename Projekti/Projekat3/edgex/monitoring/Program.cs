using System.Text;
using monitoring.Services;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<MonitoringService, MonitoringService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var mqttFactory = new MqttFactory();

var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("mqtt-edgex", 1883)
                    .WithClientId("monitoring")
                    .Build();

var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => { f.WithTopic("environment-data"); })
                    .Build();

var client = mqttFactory.CreateMqttClient();
client.ApplicationMessageReceivedAsync += async (e) =>
{
    var monitoringService = app.Services.GetRequiredService<MonitoringService>();
   
    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

    dynamic obj = JObject.Parse(payload);
    var receivedValue = obj.readings[0].value;
    var paramName = obj.readings[0].name;
    Console.WriteLine(receivedValue.GetType());
    

    byte[] bytes = Convert.FromBase64String(receivedValue.ToString());
    Array.Reverse(bytes, 0, 8);
    var convertedValue = BitConverter.ToDouble(bytes, 0);
    Console.WriteLine("Converted val: " + convertedValue);

    var paramNameStr = paramName.ToString();
    var colorName = "";

    if(convertedValue >= monitoringService.limits[paramNameStr])
    {
        Console.WriteLine(monitoringService.colors[paramNameStr]);
        Console.WriteLine(monitoringService.counts[paramNameStr]);
        Console.WriteLine(monitoringService.limits[paramNameStr]);
        monitoringService.counts[paramNameStr]++;
        if (monitoringService.counts[paramNameStr] > 4)
        {
            monitoringService.counts[paramNameStr] = 0;
            colorName = monitoringService.colors[paramNameStr];           
        }
    }
    else if (monitoringService.counts[paramNameStr] > 0) {
        monitoringService.counts[paramNameStr] = 0;
        colorName = "green";
    }

    if(colorName != "")
    {
        using (var httpClient = new HttpClient())
            {
                var jsonObj = new
                {
                    color = colorName,
                    param = paramNameStr
                };
                var c = JsonConvert.SerializeObject(jsonObj);
                Console.WriteLine(c);
                StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
                Console.WriteLine(content);
                using (var response = await httpClient.PutAsync("http://edgex-core-command:48082/api/v1/device/e606acef-b6a9-471a-b2a8-e01e5eb34987/command/0032615f-20f6-4aef-88dd-2256e46937fd", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(apiResponse);
                }
            }
    }

    return;
};

client.ConnectedAsync += async (e) =>
{
    Console.WriteLine("### CONNECTED WITH SERVER, SUBSCRIBING ###");

    await client.SubscribeAsync(mqttSubscribeOptions);
};

client.DisconnectedAsync += async (e) =>
{
    Console.WriteLine("### DISCONNECTED FROM SERVER ###");
    await Task.Delay(TimeSpan.FromSeconds(5));

    try
    {
        await client.ConnectAsync(mqttClientOptions);
    }
    catch
    {
        Console.WriteLine("### RECONNECTING FAILED ###");
    }
};

try
{
    await client.ConnectAsync(mqttClientOptions);
}
catch
{
    Console.WriteLine("### CONNECTING FAILED ###");
}

Console.WriteLine("### WAITING FOR APPLICATION MESSAGES ###");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
