using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;

namespace monitoring.Services {
    public class MonitoringService {

        public IDictionary<string, double> dict;

        //public static async Task<MonitoringService> Create()
        //{
        //    var monitoringService = new MonitoringService();
        //    await monitoringService.SubscribeOnTopic();
        //    return monitoringService;
        //}


        public MonitoringService()
        {
            dict = new Dictionary<string, double>();
            dict.Add("temp", 0.0);
            dict.Add("humidity", 0.0);
            dict.Add("smoke", 0.0);
            dict.Add("co", 0.0);
            dict.Add("lpg", 0.0);
            //SubscribeOnTopic();
        }

        public async void SubscribeOnTopic()
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
                    //Console.WriteLine("Received application message. " + e.ToString());
                    Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    Console.WriteLine(e.ApplicationMessage.Topic);
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