using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore3WebAPI.Data;
using Newtonsoft.Json;

namespace NetCore3WebAPI.Controllers
{
    [Route("api/Test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private HttpClient client;
        public TestController()
        {
             client = new HttpClient();
        }
        /// <summary>
        /// This Gets Best Questions  - this API method will return the 5 top score questions.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBestQuestions")]
        public async Task<IActionResult> GetBestQuestions()
        {
            return await Task.Run(async () =>
            {
                string jsonResponse = string.Empty;
                string url = @"https://api.stackexchange.com/2.2/questions?order=desc&sort=activity&site=stackoverflow";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonResponse = reader.ReadToEnd();
                }
                 ItemsResponse myDeserializedClass = JsonConvert.DeserializeObject<ItemsResponse>(jsonResponse);
                 IEnumerable<Item> topFive = myDeserializedClass.items.OrderByDescending(i => i.score).Take(5);
                 return Ok(topFive);
            });
        }
        /// <summary>
        /// This Gets Best Answer (int questionId) – this method will get a question id as parameter and 
        /// return its 5 accepted and top score answers.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBestAnswer")]
        public async Task<IActionResult> GetBestAnswer(int questionId)
        {
            string jsonResponse = string.Empty;
            string url = $@"https://api.stackexchange.com/2.3/answers/{questionId+1}/questions?order=desc&sort=activity&site=stackoverflow";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                jsonResponse = reader.ReadToEnd();
            }
            ItemsResponse myDeserializedClass = JsonConvert.DeserializeObject<ItemsResponse>(jsonResponse);
            IEnumerable<Item> topOne = myDeserializedClass.items.OrderByDescending(i => i.score).Take(1);
            return Ok(topOne);
        }
        /// <summary>
        /// This Gets Owner (int answerId) – this method will get an answer id and return the owner
        /// information that the answer provides and more(location, user type, creation date, etc...)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOwner")]
        public async Task<IActionResult> GetOwner(int answerId)
        {
            return await Task.Run(() =>
            {
                string jsonResponse = string.Empty;
                string url = $@"https://api.stackexchange.com/2.3/answers/{answerId}?order=desc&sort=activity&site=stackoverflow";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonResponse = reader.ReadToEnd();
                }
                ItemsResponse myDeserializedClass = JsonConvert.DeserializeObject<ItemsResponse>(jsonResponse);
                IEnumerable<Item> answer = myDeserializedClass.items.OrderByDescending(i => i.score).Take(1);
                var owner = answer.FirstOrDefault()?.owner;
                return Ok(owner);
            });
        }
    }
}