# Narakeet Text to Speech Long content (Polling) API example in `C#`

This repository provides a quick example demonstrating how to access the Narakeet [Long Content/Polling Text to Speech API](https://www.narakeet.com/docs/automating/text-to-speech-api/) from C# (.NET Core). 

The example sends a request to generate audio from text and saves it to a MP3 file in the system temporary directory.

## Prerequisites

This example works with .NET Core 6.0. You can run it inside Docker (then it does not require a local .NET Core installation), or on a system with a .NET Core 6.0 compatible installation.

## Running the example

Without Docker (create an environment variable `NARAKEET_API_KEY` with your api key):

```
dotnet publish -c Release
dotnet run --project NarakeetExample $API_KEY
```

On a system with Docker and GNU Makefile (replace `$NARAKEET_API_KEY` with your api key):

```
make init
make execute NARAKEET_API_KEY=$NARAKEET_API_KEY
```

To change the voice, script or format, modify the variables in [NarakeetExample/Program.cs](NarakeetExample/Program.cs).

## More information

Check out <https://www.narakeet.com/docs/automating/text-to-speech-api/> for more information on the Narakeet Text to Speech API
