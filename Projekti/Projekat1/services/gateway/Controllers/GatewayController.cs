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
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCfhFoxWD-tZGM7ssl7U2OJVTgmy0GqjxM",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = "rock playlist";
            searchListRequest.MaxResults = 13;
            searchListRequest.Type = "video,playlist";

            var searchListResponse = await searchListRequest.ExecuteAsync();

            // List<string> results = new List<string>();

            // foreach (var searchResult in searchListResponse.Items)
            // {
            //     switch (searchResult.Id.Kind)
            //     {
            //         case "youtube#video":
            //             results.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
            //             break;

            //         case "youtube#channel":
            //             results.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
            //             break;

            //         case "youtube#playlist":
            //             results.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
            //             break;
            //     }
            // }

            return Ok(searchListResponse);


        }

        [HttpGet("GetStatus/{userID}")]
        public async Task<IActionResult> GetStatus(int userID)
        {
            //Parameters healthParameters = new Parameters();
            ResultData resultData = new ResultData();
            using(var httpClient = new HttpClient())
            {
                var c = JsonConvert.SerializeObject(userID);
                StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
                using (var response = await httpClient.GetAsync($"http://data:3333/getCurrentValues/{userID}"))
                {
                    var parameters = await response.Content.ReadFromJsonAsync<Parameters>(); 
                    if(parameters != null)
                    {
                        resultData.HealthParameters = parameters;
                    }   
                }
            }

            string searchTerm = "radio";

            if(resultData.HealthParameters?.Sys > 130 || resultData.HealthParameters?.Sys > 90)
            {   
                if(resultData.HealthParameters?.Pulse > 80)
                    searchTerm = "calm music";
                else
                    searchTerm = "classical music";
            }
            else if(resultData.HealthParameters?.Pulse > 80) {
                searchTerm = "relaxing music";
            }
            else if (resultData.HealthParameters?.Pulse < 60 || resultData.HealthParameters?.Sys < 120 || resultData.HealthParameters?.Dias < 70)
            {
                searchTerm = "up beat music";
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCfhFoxWD-tZGM7ssl7U2OJVTgmy0GqjxM",
                ApplicationName = this.GetType().ToString()
            });


            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = searchTerm;
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "video,playlist";

            var searchListResponse = await searchListRequest.ExecuteAsync();

            resultData.ResourceTitle = searchListResponse.Items[0].Snippet.Title;
            string resourceId;
            // if(searchListResponse.Items[0].Id.Kind == "youtube#video")
            //     resourceId = searchListResponse.Items[0].Id.VideoId;
            // else
            //     resourceId = sear
            //     resultData.ResourceTitle = $"https://www.youtube.com/watch?v={searchListResponse.Items[0].Id.VideoId}";

            resourceId = (searchListResponse.Items[0].Id.Kind == "youtube#playlist") ? searchListResponse.Items[0].Id.VideoId : searchListResponse.Items[0].Id.PlaylistId;
            resultData.ResourceUrl = $"https://www.youtube.com/watch?v={resourceId}";

            return Ok(resultData);
            
        }
    }

    
}