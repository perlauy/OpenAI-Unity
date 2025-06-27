using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.Networking;

namespace OpenAI
{
    public class ChatGPTTools : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;
        [SerializeField] private RectTransform functionCall;

        [SerializeField] private string chatGPTModel = "gpt-4.1";

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private List<Tool> modelTools = new List<Tool>();

        private string prompt =
            "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
            AddWeatherFunction();
        }

        private void AddWeatherFunction()
        {
            var parameters = new ToolParameters()
            {
                Type = "object",
                Properties = new Dictionary<string, ToolProperty>()
                {
                    {
                        "latitude", new ToolProperty()
                        {
                            Type = "number",
                            Description = "Latitude of the location"
                        }
                    },
                    {
                        "longitude", new ToolProperty()
                        {
                            Type = "number",
                            Description = "Longitude of the location"
                        }
                    }
                },
                Required = new() { "location", "units" },
                AdditionalProperties = false
            };
            var toolFunction = new ToolFunction()
            {
                Name = "get_weather",
                Description = "Retrieves current weather for the given location.",
                Parameters = parameters,
                Strict = false
            };
            var tool = new Tool()
            {
                Type = "function",
                Function = toolFunction
            };

            modelTools.Add(tool);
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            RectTransform prefabMessage = message.Role switch
            {
                "user" => sent,
                "function_call" => functionCall,
                _ => received
            };

            var item = Instantiate(prefabMessage, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };

            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

            messages.Add(newMessage);

            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = chatGPTModel,
                Messages = messages,
                Tools = modelTools
            });

            var resp = JsonConvert.SerializeObject(completionResponse);
            
            Debug.Log(resp);
            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;

                if (message.Role == "assistant" && message.ToolCalls != null)
                {
                    foreach (var call in message.ToolCalls)
                    {
                        HandleFunctionCall(call);
                    }
                }
                else
                {
                    message.Content = message.Content.Trim();

                    messages.Add(message);
                    AppendMessage(message);
                }
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }

        private async void HandleFunctionCall(ToolCallsType call)
        {
            // TODO: The calls remains in the messages: check if they have output before resolving again
            if (call.Function.Name == "get_weather")
            {
                Debug.Log(call.Function.Arguments);
                var args = JsonConvert.DeserializeObject<Dictionary<string, float>>(call.Function.Arguments);
                Debug.Log(args);
                if (args.ContainsKey("latitude") && args.ContainsKey("longitude"))
                {
                    var result = await WeatherAPI.GetWeather(args["latitude"], args["longitude"]);
                    
                    AppendMessage( new ChatMessage()
                    {
                        Role = "function_call",
                        Content = result.hourly.temperature_2m[0] + "ºC"
                    });

                    // TODO: This doesn't work because it needs to be sent as "input" for responses, not as "messages"; different endpoint
                    
                    /*messages.Add(new ChatMessage()
                    {
                        Role = "assistant",
                        Content = "",
                        Type = call.Type,
                        CallId = call.Id,
                        Output = result.hourly.temperature_2m.ToString()
                    });
                    
                    // Sent result
                    var functionCompletionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
                    {
                        Model = chatGPTModel,
                        Messages = messages,
                        Tools = modelTools
                    });

                    var functionResp = JsonConvert.SerializeObject(functionCompletionResponse);
                    Debug.Log(functionResp);
                    Debug.Log(functionCompletionResponse);
                    if (functionCompletionResponse.Choices != null && functionCompletionResponse.Choices.Count > 0)
                    {
                        // var functionMessage = functionCompletionResponse.Choices[0].Message;
                        Debug.Log(functionCompletionResponse.Choices.Count);
                        var foo = JsonConvert.SerializeObject(functionCompletionResponse.Choices[0]);
                        Debug.Log(foo);
                    }*/

                }
            }
        }
    }
    
    
}