using Newtonsoft.Json.Linq;
using BinaryPub.Client.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BinaryPub.Client.Helper
{
    public class Timeline
    {
        public string ApiBaseUrl;
        public string AccessToken;
        public List<TimelineMessage> Messages;
        public string ResponseText;

        public Timeline(string host, string access_token)
        {
            ApiBaseUrl = $"https://{host}/api/v1/timelines/tag";
            AccessToken = access_token;
            Messages = new List<TimelineMessage>();
        }

        public static string RemoveHtmlTags(string input)
        {
            // 정규 표현식을 사용하여 HTML 태그 제거
            string pattern = "<.*?>";
            string replacement = "";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(input, replacement);

            return result;
        }

        public static DateTime GetDateTimeFromString(string dateString)
        {
            DateTime localTime;

            // 날짜와 시간을 파싱
            if (DateTime.TryParseExact(dateString, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                // UTC에서 로컬 시간으로 변환
                localTime = parsedDateTime.ToLocalTime();
            }
            else
            {
                localTime = DateTime.Now;
            }

            return localTime;
        }

        public void Fetch(string q)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");

                try
                {
                    // 데이터 요청
                    Task<HttpResponseMessage> responseTask = client.GetAsync($"{ApiBaseUrl}/{q}");
                    responseTask.Wait();

                    // 응답 본문 저장
                    HttpResponseMessage response = responseTask.Result;
                    Task<string> readAsStringTask = response.Content.ReadAsStringAsync();
                    readAsStringTask.Wait();
                    ResponseText = readAsStringTask.Result;

                    // JSON 파싱
                    JArray statuses = JArray.Parse(ResponseText);

                    foreach (var status in statuses)
                    {
                        string createdAt = status["created_at"].Value<string>();
                        string content = status["content"].Value<string>();

                        Messages.Add(new TimelineMessage
                        {
                            CreatedAt = GetDateTimeFromString(createdAt),
                            Content = RemoveHtmlTags(content)
                        });
                    }
                }
                catch { }
            }
        }
    }
}
