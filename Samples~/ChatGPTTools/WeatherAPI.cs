using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenAI
{
    public class WeatherAPI
    {
        public struct WeatherDataHourly
        {
            public List<string> time { get; set; } 
            public List<float> temperature_2m { get; set; } 

        }
        public struct WeatherDataHourlyUnits
        {
            public string temperature_2m { get; set; } 

        }
        public struct WeatherData
        {
            public float latitude { get; set; } 
            public float longitude { get; set; } 
            public float elevation { get; set; } 
            public float generationtime_ms { get; set; } 
            public float utc_offset_seconds { get; set; } 
            public string timezone { get; set; } 
            public string timezone_abbreviation { get; set; } 
            public WeatherDataHourly hourly { get; set; } 
            public WeatherDataHourlyUnits hourly_units { get; set; } 

        }
        
        public static async Task<WeatherData> GetWeather(float latitude, float longitude)
        {
            var url =
                "https://api.open-meteo.com/v1/forecast?latitude=" + latitude + "&longitude=" + longitude 
                + "&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m";

            string data = "";
            WeatherData resp;
        
            using (var request = UnityWebRequest.Get(url))
            {
                request.method = UnityWebRequest.kHttpVerbGET;
                var asyncOperation = request.SendWebRequest();
            
                while (!asyncOperation.isDone) await Task.Yield();
            
                // data = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                data = request.downloadHandler.text;

                Debug.Log(data);
                resp = JsonConvert.DeserializeObject<WeatherData>(data);
            }

            return resp;
        }
    }
}