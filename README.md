# Narakeet Text to Speech Long content (Polling) API example in `C#`

This repository provides a quick example demonstrating how to access the Narakeet [Long Content/Polling Text to Speech API](https://www.narakeet.com/docs/automating/text-to-speech-api/) from C# (.NET Core). 

The example sends a request to generate audio from text and saves it to a MP3 file in the system temporary directory.

Note that there is also a [simpler short content API](https://github.com/narakeet/text-to-speech-api-csharp-example) suitable for smaller requests.

## Prerequisites

This example works with .NET Core 6.0. You can run it inside Docker (then it does not require a local .NET Core installation), or on a system with a .NET Core 6.0 compatible installation.

## Running the example

Without Docker 

1. create an environment variable `NARAKEET_API_KEY` with your api key

2. run
```
dotnet publish -c Release
dotnet run --project NarakeetExample
```

Alternatively, on a system with Docker and GNU Makefile (replace `$NARAKEET_API_KEY` with your api key):

```
make init
make execute NARAKEET_API_KEY=$NARAKEET_API_KEY
```

To change the voice, script or format, modify the variables in [NarakeetExample/Program.cs](NarakeetExample/Program.cs).

## More information

Check out <https://www.narakeet.com/docs/automating/text-to-speech-api/> for more information on the Narakeet Text to Speech API
