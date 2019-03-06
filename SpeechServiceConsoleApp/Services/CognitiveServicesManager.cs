using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;

using SpeechServiceConsoleApp.Models;
using Newtonsoft.Json;

namespace SpeechServiceConsoleApp.Services
{
    class CognitiveServicesManager
    {
        private string key;
        private string recognizedSpeech;
        public Order order;

        public CognitiveServicesManager(string key)
        {
            this.key = key;

            recognizedSpeech = String.Empty;
        }

        public void Run()
        {   // Refactor here if StartContinuous is very different from RecognizeOnce
            RecognizeSpeech().Wait();
        }

        public async Task RecognizeSpeech()
        {
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            var speechConfig = SpeechConfig.FromSubscription(key, "westus2");

            // Creates a speech recognizer.
            using (var recognizer = new IntentRecognizer(speechConfig, audioConfig))
            {
                // Hide user secrets later
                var model = LanguageUnderstandingModel.FromAppId(Environment.GetEnvironmentVariable("LUIS_APP_ID"));
                recognizer.AddAllIntents(model);

                var stopRecognition = new TaskCompletionSource<int>();
                // Can add logic to exit using voice command, "Thanks see you at the window" etc.
                // Subscribe to appropriate events
                recognizer.Recognizing += (s, e) => 
                {
                    // Use this to send partial responses
                    Console.WriteLine($"Partial: {e.Result.Text}");
                };

                recognizer.Recognized += (s, e) =>
                {
                    var exit = ProcessRecognizedText(s, e);
                    if(exit)
                    {
                        recognizer.StopContinuousRecognitionAsync().Wait(); //ConfigureAwait(false);
                    }
                };

                recognizer.SessionStarted += (s, e) => 
                {
                    Console.WriteLine("Session started event.");
                };

                recognizer.SessionStopped += (s, e) => 
                {
                    Console.WriteLine("Session stopped event.");
                    stopRecognition.TrySetResult(0);
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine(e.ErrorDetails);
                    stopRecognition.TrySetResult(0);
                };

                Console.WriteLine("Say something to get started, or \"Exit\" to quit.");
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Need more understanding about this part
                Task.WaitAny(new[] { stopRecognition.Task });

            }
        }

        private Boolean ProcessRecognizedText(object s, IntentRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizedIntent)
            {
                Console.WriteLine($"Recognized Text: {e.Result.Text}");
                Console.WriteLine($"Detected Intent: {e.Result.IntentId}");
                // How to make this show all intent probabilities not just one?
                Console.WriteLine(e.Result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult));
                
                var rawJason = e.Result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
                LuisWrapper luis = JsonConvert.DeserializeObject<LuisWrapper>(rawJason);

                Console.WriteLine(luis.TopScoringIntent);

                if (luis.Query.ToUpper().Contains("EXIT") || luis.Query.ToUpper().Contains("WINDOW"))
                {
                    return true;
                }

            }
            else if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"Recognized Text: {e.Result.Text}");
                Console.WriteLine($"Detected Intent: <No Intent>");

                if (e.Result.Text.ToUpper().Contains("EXIT") || e.Result.Text.ToUpper().Contains("WINDOW"))
                {
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Speech could not be recognized.");
                return false;
            }
            return false;
        }

        public async Task<string> ProcessIntent()
        {
            // TODO
            return null;
        }
    }
}
