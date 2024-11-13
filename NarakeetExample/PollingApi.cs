using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NarakeetExample
{
    public class PollingApi
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly int _pollingIntervalSeconds;

        public PollingApi(string apiKey, string apiUrl = "https://api.narakeet.com", int pollingIntervalSeconds = 5)
        {
            _apiKey = apiKey;
            _apiUrl = apiUrl;
            _pollingIntervalSeconds = pollingIntervalSeconds;
        }

        public class AudioTaskRequest
        {
            public string Voice { get; }
            public string Text { get; }
            public string Format { get; }

            public AudioTaskRequest(string format, string voice, string text)
            {
                Voice = voice;
                Text = text;
                Format = format;
            }
        }

        public class BuildTask
        {
            [JsonPropertyName("statusUrl")]
            public string StatusUrl { get; set; } = string.Empty; // Set default value to avoid nullable warning

            [JsonPropertyName("taskId")]
            public string TaskId { get; set; } = string.Empty; // Set default value to avoid nullable warning
        }

        public class BuildTaskStatus
        {
            [JsonPropertyName("message")]
            public string Message { get; set; } = string.Empty; // Set default value to avoid nullable warning

            [JsonPropertyName("percent")]
            public int Percent { get; set; }

            [JsonPropertyName("succeeded")]
            public bool Succeeded { get; set; }

            [JsonPropertyName("finished")]
            public bool Finished { get; set; }

            [JsonPropertyName("result")]
            public string? Result { get; set; } // Make nullable as it may not always be populated
        }

        public async Task<BuildTask> RequestAudioTaskAsync(AudioTaskRequest audioTaskRequest)
        {
            string url = $"{_apiUrl}/text-to-speech/{audioTaskRequest.Format}?voice={audioTaskRequest.Voice}";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(audioTaskRequest.Text));
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            var response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BuildTask>(responseContent)
                       ?? throw new InvalidOperationException("Failed to deserialize BuildTask.");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP error: {response.StatusCode} {errorContent}");
            }
        }

        public async Task<BuildTaskStatus> PollUntilFinishedAsync(BuildTask buildTask, Action<BuildTaskStatus>? progressCallback = null)
        {
            using var httpClient = new HttpClient();

            while (true)
            {
                var response = await httpClient.GetAsync(buildTask.StatusUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var buildTaskStatus = JsonSerializer.Deserialize<BuildTaskStatus>(responseContent)
                                          ?? throw new InvalidOperationException("Failed to deserialize BuildTaskStatus.");

                    if (buildTaskStatus.Finished)
                    {
                        return buildTaskStatus;
                    }

                    progressCallback?.Invoke(buildTaskStatus);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"HTTP error: {response.StatusCode} {errorContent}");
                }

                await Task.Delay(_pollingIntervalSeconds * 1000);
            }
        }

        public async Task<string> DownloadToTempFileAsync(string url, string extension)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var tempPath = Path.GetTempPath();
                var tempFilePath = Path.Combine(tempPath, $"{Guid.NewGuid()}.{extension}");

                await using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(fileStream);

                return tempFilePath;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP error: {response.StatusCode} {errorContent}");
            }
        }
    }
}

