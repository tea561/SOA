using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;

namespace monitoring.Services {
    public class MonitoringService {

        //public double Temp { get; set; }
        //public double Humidity { get; set; }

        //public double Lpg { get; set; }

        //public double Co { get; set; }

        //public double Smoke { get; set; }

        public static async Task<MonitoringService> Create()
        {
            var monitoringService = new MonitoringService();
            await monitoringService.SubscribeOnTopic();
            return monitoringService;
        }


        public MonitoringService()
        {
            //Temp = 0.0;
            //Humidity = 0.0;
            //Lpg = 0.0;
            //Co = 0.0;
            //Smoke = 0.0;

            SubscribeOnTopic();
        }

        private async Task SubscribeOnTopic()
        {
            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("mqtt-edgex", 1883)
                    .WithClientId("monitoring")
                    .Build();

                
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    Console.WriteLine("Received application message. " + e.ToString());
                    //var result = DumpToConsole(e);
                    return Task.CompletedTask;
                };

                mqttClient.ConnectedAsync += conn =>
                {
                    Console.WriteLine("The MQTT client is connected.");
                    return Task.CompletedTask;
                };
                mqttClient.DisconnectedAsync += disc =>
                {
                    Console.WriteLine("The MQTT client is disconnected.");
                    return Task.CompletedTask;
                };

                var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                Console.WriteLine(response.ResultCode);

                // var applicationMessage = new MqttApplicationMessageBuilder()
                //     .WithTopic("environment-data")
                //     .WithPayload(payload)
                //     .Build();
                // await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                // Console.WriteLine("MQTT application message is published.");

                
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => {f.WithTopic("environment-data"); })
                    .Build();

                
                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
                Console.WriteLine("MQTT client subscribed to topic.");
                
                var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
                    .WithReason(MqttClientDisconnectReason.NormalDisconnection)
                    .Build();
                //await mqttClient.DisconnectAsync(disconnectOptions);

            }
        }

        // private TObject DumpToConsole<TObject>(this TObject @object)
        // {
        //     var output = "NULL";
        //     if (@object != null)
        //     {
        //         output = JsonSerializer.Serialize(@object, new JsonSerializerOptions
        //         {
        //             WriteIndented = true
        //         });
        //     }

        //     Console.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");
        //     return @object;
        // }

    }
}