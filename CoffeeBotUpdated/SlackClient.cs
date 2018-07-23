using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CoffeeBotAppUpdated;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static CoffeeBotAppUpdated.Order;

namespace CoffeeBotApp.SlackClient
{
    public class Fields
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Short { get; set; }
    }

    public class Option
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class Action
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public Option[] Options { get; set; }
    }

    public class Attachment
    {
        // public string Title{ get; set; }
        public string Fallback { get; set; }
        public string CallbackId { get; set; }
        public string Color { get; set; }
        public string AttachmentType { get; set; }
        // public string ImageUrl { get; set; }
        public string Text { get; set; }
        public Action[] Actions { get; set; }
    }
    public class SlackPostRequest
    {
        public string Text { get; set; }
        public string ResponseType { get; set; }
        public Attachment[] Attachments { get; set; }
    }

    public class SlackClient
    {
        private readonly Uri _webhookUrl;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly WebClient _webClient = new WebClient();

        public SlackClient()
        {
        }
        public SlackClient(Uri webhookUrl)
        {
            _webhookUrl = webhookUrl;
        }

        public string PostMenu()
        {
            var channel = "CB5NU9NHY";
            var payload = new
            {
                Text = "Another attachment",
                Attachments = new[]
                {
                    new Attachment()
                    {
                        Fallback =
                            "Would you recommend it to customers?",
                        Color = "#7CD197",
                        AttachmentType = "default",
                        CallbackId = "coffee_menu",
                        Actions = new []
                        {
                            new Action
                            {
                                Name = "recommend",
                                Text = "Recommend",
                                Type = "button",
                                Value = "recommend"
                            },
                            new Action
                            {
                                Name = "no",
                                Text = "No",
                                Type = "button",
                                Value = "No"
                            }
                        }
                    }
                },
                channel
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(payload, settings);
        }

        public string CreateMenu(SlackCommand command)
        {
            var channel = command.ChannelId;
            var payload = new
            {
                Text = "Would you like to order your coffees in?",
                ResponseType = "in_channel",
                Attachments = new []
               {
                    new Attachment()
                    {
                         Text = "TChoose a game to play",
                        Fallback =
                             "If you could read this message, you'd be choosing something fun to do right now.",
                        Color = "#3AA3E3",
                        AttachmentType = "default",
                        CallbackId = "coffee_selection",
                        Actions = new []
                        {
                              new Action
                            {
                                Name = "confirm",
                                Text = "Confirm",
                                Type = "button",
                                Value = "confirm"
                            },
                            new Action
                            {
                                Name = "cancel",
                                Text = "Cancel",
                                Type = "button",

                        },
                            new Action
                            {
                                Name = "coffees_list",
                                Text = "Choose you order",
                                Type = "select",
                                Options = new Option[]
                                {
                                    new Option()
                                    {
                                       Text = "Flat White",
                                       Value = "Flat White"
                                    },
                                     new Option()
                                    {
                                       Text = "Long Black",
                                       Value = "Long Black"
                                    },
                                      new Option()
                                    {
                                       Text = "Coffee Latte",
                                       Value = "Coffee Latte"
                                    },
                                       new Option()
                                    {
                                       Text = "Hot Chocolate",
                                       Value = "Hot Chocolate"
                                    }
                                }
                            }
                        }
                    }
               },
                channel
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(payload, settings);
        }

        public async Task<HttpResponseMessage> SendMessageAsync(string message, string channel = null, string username = null)
        {
            var builder = new UriBuilder(_webhookUrl) { Port = -1 };
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["text"] = message;

            query[nameof(channel)] = channel;
            builder.Query = query.ToString();
            var url = builder.ToString();

            var response = await _httpClient.GetAsync(url);
            return response;
        }

        public string SendPostMessage(SlackCommand command)
        {
            var payload = new
            {
                Text = "New comic book alert!",
                Attachments = new []
                {
                    new Attachment()
                    {
                        Fallback =
                             "Would you recommend it to customers?",
                        Color = "#7CD197",
                        // Title = "Would you recommend it to customers?",
                        AttachmentType = "default",
                        Actions = new []
                        {
                            new Action
                            {
                                Name = "recommend",
                                Text = "Recommend",
                                Type = "button",
                                Value = "recommend"
                            },
                            new Action
                            {
                                Name = "no",
                                Text = "No",
                                Type = "button",
                                Value = "No"
                            }
                        }
                    }
                },
                command.ChannelName
            };


            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(payload, settings);
        }
    }
}