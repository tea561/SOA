using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using gateway.Models;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;


namespace gateway.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]

    public class GatewayController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public GatewayController(IConfiguration configuration)
        {
            _configuration = configuration;
            Connect_Client();
            //Disconnect_Client();
        }

        /// <summary>
        /// Gets vitals and recommended YouTube resource for specific user.
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns>Vitals and recommended YouTube resource.</returns>
        /// <response code="404">User not found.</response>
        [HttpGet("GetStatus/{userID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatus(int userID)
        {

            ResultData resultData = new ResultData();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"http://data:3333/getVitals?userID={userID}"))
                {

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var parameters = await response.Content.ReadFromJsonAsync<Parameters>();
                        resultData.HealthParameters = parameters;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        //var errorResponse = await response.Content.ReadAsStringAsync();
                        return StatusCode(500);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }

            string searchTerm = "neutral";

            if (resultData.HealthParameters?.Sys > 130 || resultData.HealthParameters?.Dias > 90)
            {
                if (resultData.HealthParameters?.Pulse > 80)
                    searchTerm = "calm";
                else
                    searchTerm = "classical";
            }
            else if (resultData.HealthParameters?.Pulse > 80)
            {
                searchTerm = "relaxing";
            }
            else if (resultData.HealthParameters?.Pulse < 60 || resultData.HealthParameters?.Sys < 120 || resultData.HealthParameters?.Dias < 70)
            {
                searchTerm = "upbeat";
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                //ApiKey = "AIzaSyCfhFoxWD-tZGM7ssl7U2OJVTgmy0GqjxM",
                ApiKey = _configuration["ApiKey"],
                ApplicationName = this.GetType().ToString()
            });



            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = QueryGenerator.QueryGenerator.getQueryParameter(searchTerm);
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "video,playlist";

            var searchListResponse = await searchListRequest.ExecuteAsync();

            resultData.ResourceTitle = searchListResponse.Items[0].Snippet.Title;
            string resourceId;

            resourceId = (searchListResponse.Items[0].Id.Kind == "youtube#playlist") ? searchListResponse.Items[0].Id.PlaylistId : searchListResponse.Items[0].Id.VideoId;
            resultData.ResourceUrl = $"https://www.youtube.com/watch?v={resourceId}";

            return Ok(resultData);

        }

        /// <summary>
        /// Post vital parameters for specific user.
        /// </summary>
        /// <param name="parameters">Vital parameters</param>
        /// <returns>True if vitals are successfully added. Otherwise, false.</returns>
        /// <remarks>
        ///     Sample request:
        ///         
        ///         POST /Gateway
        ///         {
        ///             "sys": 120,
        ///             "dias": 80,
        ///             "pulse": 90,
        ///             "timestamp": 1652119779,
        ///             "userID": 3
        ///         }
        ///
        ///</remarks>
        /// <response code="400">Post parameters not defined.</response>
        /// <response code="200">Vitals successfully added.</response>

        [HttpPost("PostVitals")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] Parameters parameters)
        {
            using (var httpClient = new HttpClient())
            {
                var serializedObject = JsonConvert.SerializeObject(parameters);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("http://data:3333/postVitals", content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var apiResponse = await response.Content.ReadFromJsonAsync<bool>();
                        return Ok(apiResponse);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return StatusCode(500);
                    }

                }

                return BadRequest();
            }
        }

        /// <summary>
        /// Delete vital parameters for specific user.
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns>True if vitals are successfully added. Otherwise, false.</returns>
        /// <response code="200">Vitals successfully deleted.</response>
        [HttpDelete("DeleteDataForUser/{userID}")]
        public async Task<ActionResult> DeleteDataForUser(int userID)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"http://data:3333/deleteVitals?userID={userID}"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Ok();
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return StatusCode(500);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
        }

        /// <summary>
        /// Update vital parameters for specific user.
        /// </summary>
        /// <param name="parameters">Vital parameters</param>
        /// <returns>True if vitals are successfully updated. Otherwise, false.</returns>
        /// <response code="400">Put parameters not defined.</response>
        /// <response code="200">Vitals successfully added.</response>
        /// <remarks>
        ///     Sample request:
        ///         
        ///         PUT /Gateway
        ///         {
        ///             "sys": 120,
        ///             "dias": 80,
        ///             "pulse": 90,
        ///             "timestamp": 1652119779,
        ///             "userID": 3
        ///         }
        ///
        ///</remarks>
        [HttpPut("UpdateVitals")]
        public async Task<ActionResult> Put([FromBody] Parameters parameters)
        {
            using (var httpClient = new HttpClient())
            {
                var serializedObject = JsonConvert.SerializeObject(parameters);
                StringContent content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("http://data:3333/updateVitals", content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var apiResponse = await response.Content.ReadFromJsonAsync<bool>();
                        return Ok(apiResponse);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return StatusCode(500);
                    }
                }

                return BadRequest();
            }
        }

        public async Task Connect_Client()
        {
            /*
             * This sample creates a simple MQTT client and connects to a public broker.
             *
             * Always dispose the client when it is no longer used.
             * The default version of MQTT is 3.1.1.
             */

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                // Use builder classes where possible in this project.
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("mqtt", 1883)
                    .WithClientId("gateway")
                    .Build();


                // This will throw an exception if the server is not available.
                // The result from this message returns additional data which was sent 
                // from the server. Please refer to the MQTT protocol specification for details.
                var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                Console.WriteLine("The MQTT client is connected.");

                Console.WriteLine(response.ResultCode);

                var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectReason.NormalDisconnection)
                .Build();

                await mqttClient.DisconnectAsync(disconnectOptions);

                Console.WriteLine("The MQTT client is disconnected.");

                // Send a clean disconnect to the server by calling _DisconnectAsync_. Without this the TCP connection
                // gets dropped and the server will handle this as a non clean disconnect (see MQTT spec for details).
                //var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

                //await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
            }
        }

        public async Task Disconnect_Client()
        {
            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectReason.NormalDisconnection)
                .Build();

                await mqttClient.DisconnectAsync(disconnectOptions);

                Console.WriteLine("The MQTT client is disconnected.");
            }
        }
    }
}