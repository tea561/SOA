using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using gateway.Models;

namespace gateway.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]

    public class GatewayController : ControllerBase
    {
        public GatewayController()
        {

        }
        // [HttpGet]
        // public async Task<IActionResult> Get()
        // {
        //     var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        //     {
        //         ApiKey = "AIzaSyCfhFoxWD-tZGM7ssl7U2OJVTgmy0GqjxM",
        //         ApplicationName = this.GetType().ToString()
        //     });

        //     var searchListRequest = youtubeService.Search.List("snippet");
        //     searchListRequest.Q = "rock playlist";
        //     searchListRequest.MaxResults = 13;
        //     searchListRequest.Type = "video,playlist";

        //     var searchListResponse = await searchListRequest.ExecuteAsync();

        //     // List<string> results = new List<string>();

        //     // foreach (var searchResult in searchListResponse.Items)
        //     // {
        //     //     switch (searchResult.Id.Kind)
        //     //     {
        //     //         case "youtube#video":
        //     //             results.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
        //     //             break;

        //     //         case "youtube#channel":
        //     //             results.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
        //     //             break;

        //     //         case "youtube#playlist":
        //     //             results.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
        //     //             break;
        //     //     }
        //     // }

        //     return Ok(searchListResponse);


        // }

        /// <summary>
        /// Gets health parameters and recommended YouTube resource for specific user.
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        /// <response code="404">User not found.</response>
        [HttpGet("GetStatus/{userID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatus(int userID)
        {
            
            ResultData resultData = new ResultData();
            using(var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"http://data:3333/getVitals?userID={userID}"))
                {
                    
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var parameters = await response.Content.ReadFromJsonAsync<Parameters>();
                        resultData.HealthParameters = parameters;
                        // long time = (parameters.Timestamp != null) ? long.Parse(parameters.Timestamp) : 0L;
                        // resultData.HealthParameters.Timestamp = (new DateTime(time)).ToString("f");
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
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

            if(resultData.HealthParameters?.Sys > 130 || resultData.HealthParameters?.Dias > 90)
            {   
                if(resultData.HealthParameters?.Pulse > 80)
                    searchTerm = "calm";
                else
                    searchTerm = "classical";
            }
            else if(resultData.HealthParameters?.Pulse > 80) {
                searchTerm = "relaxing";
            }
            else if (resultData.HealthParameters?.Pulse < 60 || resultData.HealthParameters?.Sys < 120 || resultData.HealthParameters?.Dias < 70)
            {
                searchTerm = "upbeat";
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCfhFoxWD-tZGM7ssl7U2OJVTgmy0GqjxM",
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
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Parameters parameters)
        {
            using(var httpClient = new HttpClient())
            {
                var serializedObject = JsonConvert.SerializeObject(parameters);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("http://data:3333/postVitals", content))
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var apiResponse = await response.Content.ReadFromJsonAsync<bool>();
                        return Ok(apiResponse);
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return StatusCode(500);
                    }

                }

                return BadRequest();
            }
        }

        [HttpDelete("DeleteDataForUser/{userID}")]
        public async Task<ActionResult> DeleteDataForUser(int userID)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"http://data:3333/deleteVitals?userID={userID}"))
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        
                        return Ok();
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return StatusCode(500);
                    }
                    else {
                        return BadRequest();
                    }
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Parameters parameters)
        {
            using(var httpClient = new HttpClient())
            {
                var serializedObject = JsonConvert.SerializeObject(parameters);
                StringContent content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("http://data:3333/updateVitals", content))
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var apiResponse = await response.Content.ReadFromJsonAsync<bool>();
                        return Ok(apiResponse);
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return NotFound(errorResponse);
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return StatusCode(500);
                    }
                }

                return BadRequest();
            }
        }
    }

    
}