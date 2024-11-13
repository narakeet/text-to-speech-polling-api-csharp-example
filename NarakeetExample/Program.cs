using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;
using System;

namespace NarakeetExample
{
	public class Program
	{    
		private const string voice = "Ronald";
		private const string script = "Hey there, from C Sharp polling API";
		private const string format = "mp3";

		public static async Task Main(string[] args)
		{
			string apiKey = Environment.GetEnvironmentVariable("NARAKEET_API_KEY")
				?? throw new InvalidOperationException("NARAKEET_API_KEY environment variable is not set");

			var api = new NarakeetExample.PollingApi(apiKey);
			var request = new NarakeetExample.PollingApi.AudioTaskRequest(format, voice, script);

			// Start the build task and wait for it to finish
			NarakeetExample.PollingApi.BuildTask buildTask = await api.RequestAudioTaskAsync(request);
			NarakeetExample.PollingApi.BuildTaskStatus taskResult = await api.PollUntilFinishedAsync(buildTask, buildTaskStatus =>
				{
				// Print progress
				Console.WriteLine($"Progress: {buildTaskStatus.Message} ({buildTaskStatus.Percent}%)");
				});

			// Grab the results
			if (taskResult.Succeeded)
			{
				string filePath = await api.DownloadToTempFileAsync(taskResult.Result, request.Format);
				Console.WriteLine($"Downloaded to {filePath}");
			}
			else
			{
				Console.WriteLine($"Error creating audio: {taskResult.Message}");
			}
		}
	}
}
