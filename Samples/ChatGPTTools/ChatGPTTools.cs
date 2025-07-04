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

        [SerializeField] private string chatGPTModel = "gpt-4.1-nano-2025-04-14";

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<InputResponse> messages = new List<InputResponse>();
        private List<ResponseTool> modelTools = new List<ResponseTool>();

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
            var tool = new ResponseTool()
            {
                Type = "function",
                Name = "get_weather",
                Description = "Retrieves current weather for the given location.",
                Parameters = parameters,
                Strict = false
            };

            modelTools.Add(tool);
        }

        private void AppendMessage(InputResponse message)
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
            var newMessage = new InputResponse()
            {
                Role = "user",
                Content = inputField.text
            };

            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

            messages.Add(newMessage);
//Supported values are: 'assistant', 'system', 'developer', and 'user'."
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;

            Debug.Log(messages);
            // Complete the instruction
            var completionResponse = await openai.CreateResponse(new ResponseRequest()
            {
                Model = chatGPTModel,
                Input = messages,
                Tools = modelTools
            });

            HandleResponse(completionResponse, false);

            button.enabled = true;
            inputField.enabled = true;
        }

        private async void HandleResponse(Response completionResponse, bool onlyText = true)
        {
            
            var resp = JsonConvert.SerializeObject(completionResponse);
            Debug.Log(resp);
            
            if (completionResponse.Output.Count > 0)
            {
                foreach (var output in completionResponse.Output)
                {
                    switch (output.Type)
                    {
                        case "message":
                            Debug.Log("Message");
                            if (output.Content == null) return;
                            foreach (var msg in output.Content)
                            {
                                var message = new InputResponse();
                                message.Content = msg.Text;
                                message.Role = output.Role;
                                messages.Add(message);
                                AppendMessage(message);
                            }
                            
                            break;

                        case "function_call":
                            if (onlyText) return;
                            
                            Debug.Log("Function");
                            HandleFunctionCall(output);

                            break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

        } 
        
        private async void HandleFunctionCall(OutputResponse call)
        {
            if (call.Name == "get_weather")
            {
                Debug.Log(call.Arguments);
                var args = JsonConvert.DeserializeObject<Dictionary<string, float>>(call.Arguments);
                Debug.Log(args);
                if (args.ContainsKey("latitude") && args.ContainsKey("longitude"))
                {
                    var result = await WeatherAPI.GetWeather(args["latitude"], args["longitude"]);
                    
                    // AppendMessage( new ChatMessage()
                    // {
                        // Role = "function_call",
                        // Content = result.hourly.temperature_2m[0] + "ºC"
                    // });

                    List<InputResponse> input = new();

                    input.Add(new InputResponse()
                    {
                        Type = call.Type,
                        Id = call.Id,
                        CallId = call.CallId,
                        Name = call.Name,
                        Arguments = call.Arguments
                    });
                    input.Add(new InputResponse()
                    {
                        Type = "function_call_output",
                        CallId = call.CallId,
                        Output = result.hourly.temperature_2m[0] + "ºC"
                    });
                    
                    // Sent result
                    var functionCompletionResponse = await openai.CreateResponse(new ResponseRequest()
                    {
                        Model = chatGPTModel,
                        Input = input,
                        Tools = modelTools,
                        Store = true
                    });

                    HandleResponse(functionCompletionResponse);

                }
            }
        }
    }
    
    
}