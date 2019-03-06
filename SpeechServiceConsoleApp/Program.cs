using System;
using SpeechServiceConsoleApp.Services;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using NAudio.CoreAudioApi;

namespace SpeechServiceConsoleApp
{
    class Program
    {
        static void Main()
        {
            // var enumerator = new MMDeviceEnumerator();
            // var endpoints = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            // foreach(var endpoint in endpoints)
            // {
            //     Console.WriteLine(endpoint.FriendlyName);
            //     Console.WriteLine(endpoint.ID);
            // }

            CognitiveServicesManager manager = new CognitiveServicesManager(Environment.GetEnvironmentVariable("LUIS_KEY"));
            manager.Run();
        }
    }
}