
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using CoffeeBotApp.SlackClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace CoffeeBotAppUpdated
{
    public static class Order
    {
        [FunctionName("order")]
        public static async Task<object> PostCoffeeOrderTemplate([HttpTrigger(AuthorizationLevel.Function,
            "post",
            Route = "coffeebot/order")]HttpRequestMessage req)
        {

            // extract command
            var requestBody = await req.Content.ReadAsStringAsync(); ;
            var nameValueCollection = HttpUtility.ParseQueryString(requestBody);
            var slackCommand = CreateSlackCommand(nameValueCollection);

            // cook command
            var response = CookCommand(slackCommand);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        [FunctionName("selectCoffee")]
        public static async Task<object> GetCoffeeType([HttpTrigger(AuthorizationLevel.Function,
            "post",
            Route = "coffeebot/selectCoffee")]HttpRequestMessage req)
        {

            // extract command
            var requestBody = await req.Content.ReadAsStringAsync(); ;
            var nameValueCollection = HttpUtility.ParseQueryString(requestBody);
            var slackCommand = CreateSlackCommand(nameValueCollection);

            // cook command
            var slackClient = new SlackClient();
            var response = CookCommand(slackCommand);
            var resultant = JsonConvert.SerializeObject(response);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        private static string CookCommand(SlackCommand command)
        {
            dynamic responseJson = null;
            var slackClient = new SlackClient();

            responseJson = command.Text.StartsWith("order", ignoreCase: true, culture: CultureInfo.CurrentCulture) ? 
                slackClient.PostMenu() : 
                slackClient.SendPostMessage(command);
            return responseJson;
        }

        private static SlackCommand CreateSlackCommand(NameValueCollection nameValueCollection)
        {
            return new SlackCommand
            {
                ChannelId = nameValueCollection["channel_id"],
                ChannelName = nameValueCollection["channel_name"],
                Command = nameValueCollection["command"],
                ResponseUrl = nameValueCollection["response_url"],
                TeamDomain = nameValueCollection["team_domain"],
                Text = nameValueCollection["text"],
                Token = nameValueCollection["token"],
                TriggerId = nameValueCollection["trigger_id"],
                UserId = nameValueCollection["user_id"],
                UserName = nameValueCollection["user_name"]
            };
        }

        public class SlackCommand
        {
            public string Token { get; set; }
            public string TeamDomain { get; set; }
            public string ChannelId { get; set; }
            public string ChannelName { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Command { get; set; }
            public string Text { get; set; }
            public string ResponseUrl { get; set; }
            public string TriggerId { get; set; }
        }
    }
}