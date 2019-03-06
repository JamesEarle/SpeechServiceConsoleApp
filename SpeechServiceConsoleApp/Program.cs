using System;
using SpeechServiceConsoleApp.Services;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace SpeechServiceConsoleApp
{
    class Program
    {
        static void Main()
        {
            CognitiveServicesManager manager = new CognitiveServicesManager(Environment.GetEnvironmentVariable("LUIS_KEY"));
            manager.Run();
        }
    }
}