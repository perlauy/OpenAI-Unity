using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenAI;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenAI
{
    public class CreateTTSRequest {
        public string model { get; set; }
        public string input { get; set; }
        public string voice { get; set; }
    }
    
    public class OpenAITtsApi : OpenAIApi
    {
        private const string BASE_PATH = "https://api.openai.com/v1";
             
        private Configuration configuration;
        private Configuration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = new Configuration();
                }

                return configuration;
            }
        }

        public OpenAIApiTTS(string apiKey = null, string organization = null)
        {
            if (apiKey != null)
            {
                configuration = new Configuration(apiKey, organization);
            }
        }

        /// Used for serializing and deserializing PascalCase request object fields into snake_case format for JSON. Ignores null fields when creating JSON strings.
        private readonly JsonSerializerSettings jsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore, 
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CustomNamingStrategy()
            },
            MissingMemberHandling = MissingMemberHandling.Error,
            Culture = CultureInfo.InvariantCulture
        };
        
        /// <summary>
        ///     Create byte array payload from the given request object that contains the parameters.
        /// </summary>
        /// <param name="request">The request object that contains the parameters of the payload.</param>
        /// <typeparam name="T">type of the request object.</typeparam>
        /// <returns>Byte array payload.</returns>
        private byte[] CreatePayload<T>(T request)
        {
            var json = JsonConvert.SerializeObject(request, jsonSerializerSettings);
            return Encoding.UTF8.GetBytes(json);
        }
 
        /// <summary>
        /// Turn text into spoken audio.
        /// </summary>
        /// <param name="request">See <see cref="CreateTTSRequest"/></param>
        /// <returns>See <see cref="CreateTTSResponse"/></returns>
        public UnityWebRequest CreateTextToSpeechRequest(CreateTTSRequest request)
        {
            var path = $"{BASE_PATH}/audio/speech";
            var payload = CreatePayload(request);
            
            UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);
            req.method = UnityWebRequest.kHttpVerbPOST;
            req.uploadHandler = new UploadHandlerRaw(payload);
            req.disposeUploadHandlerOnDispose = true;
            req.disposeDownloadHandlerOnDispose = true;
            req.SetHeaders(Configuration, ContentType.ApplicationJson);

            return req;
        }
    }
}