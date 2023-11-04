using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZoomDemo.Models;

namespace ZoomDemo.Controllers;
public class HomeController : Controller
{
    private readonly string zoomUserId = "round48special@gmail.com";
    private readonly string accountId = "pf-4N.....";
    private readonly string clientId = "oMy....";
    private readonly string clientSecret = "YG....";
    private  string _accessToken = "";

    public HomeController() =>_accessToken = GetAccessTokenAsync(accountId, clientId, clientSecret).Result;
    
    public IActionResult Index()
    {
       // var tt = await DeleteMeetingAsync("83286765386");
       // Console.WriteLine(tt);
        return View();
    }

    public async Task<IActionResult> CreateMeeting()
    {
        try
        {
            _accessToken = GetAccessTokenAsync(accountId, clientId, clientSecret).Result;
            // Create the request body
            var requestBody = new
            {
                topic = "My Zoom Meeting",
                type = 2, // Scheduled meeting          
                start_time = DateTime.UtcNow.AddMinutes(15).ToString("yyyy-MM-ddTHH:mm:ss"),
                duration = 30, // Meeting duration in minutes
                timezone = "Asia/Dhaka",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                }
            };

            var apiUrl = $"https://api.zoom.us/v2/users/{zoomUserId}/meetings";
            var requestBodyJson = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", _accessToken);
            // Send the POST request to create a meeting
            var response = await httpClient.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var createdMeeting = JsonConvert.DeserializeObject<ZoomMeetingResponse>(responseContent);
                return Ok(createdMeeting);
            }
            return BadRequest("Failed to create the Zoom meeting: " + response.ReasonPhrase);

        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }


    public async Task<bool> DeleteMeetingAsync(string MeetingId)
    {
        var deleteUrl = $"https://api.zoom.us/v2/meetings/{MeetingId}";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", _accessToken);
        var response = await httpClient.DeleteAsync(deleteUrl);
        return response.IsSuccessStatusCode;
    }
    private async Task<string> GetAccessTokenAsync(string AccountId, string ClientId, string ClientSecret)
    {
        try
        {
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret));
            var token = "Basic " + encoded;
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var apiUrl = $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={AccountId}";

            var response = await httpClient.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<ZoomTokenResponse>(responseContent);
                return "Bearer " + tokenResponse?.Access_token;
            }
            else
            {
                throw new Exception("Failed to retrieve access token: " + response.ReasonPhrase);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while obtaining the access token: " + ex.Message);
        }
    }
}