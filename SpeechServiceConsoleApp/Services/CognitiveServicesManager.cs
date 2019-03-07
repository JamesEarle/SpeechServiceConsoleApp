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
        public Order _order;

        public CognitiveServicesManager(string key)
        {
            this.key = key;
            recognizedSpeech = String.Empty;
        }

        public void Run()
        {   
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
                    // Console.WriteLine($"Partial: {e.Result.Text}");
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

                // Instantiate new Order object
                _order = new Order();

                Console.WriteLine("Say something to get started, or \"Exit\" to quit.");
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Need more understanding about this part
                Task.WaitAny(new[] { stopRecognition.Task });

            }
        }

        private Boolean ProcessRecognizedText(object s, IntentRecognitionEventArgs e)
        {
            Boolean exit = false;
            if (e.Result.Reason == ResultReason.RecognizedIntent)
            {
                // Console.WriteLine($"Recognized Text: {e.Result.Text}");
                // Console.WriteLine($"Detected Intent: {e.Result.IntentId}");
                
                var rawJason = e.Result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
                LuisWrapper luisResponse = JsonConvert.DeserializeObject<LuisWrapper>(rawJason);

                // Get intent and process entities
                exit = ProcessResponse(luisResponse);
            }
            else if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"Recognized Text: {e.Result.Text}");
                Console.WriteLine($"Detected Intent: <No Intent>");

                if (e.Result.Text.ToUpper().Contains("EXIT") || e.Result.Text.ToUpper().Contains("WINDOW"))
                {
                    exit = true;
                }
            }
            else
            {
                Console.WriteLine("Speech could not be recognized.");
                exit = false;
            }
            return exit;
        }

        public Boolean ProcessResponse(LuisWrapper luisResponse)
        {
            switch(luisResponse.TopScoringIntent.Intent)
            {
                case "AddToOrder":
                    if(luisResponse.CompositeEntities != null) 
                    {
                        if(luisResponse.CompositeEntities.Length > 0) 
                        {
                            var items = GetCompositeItemDetails(luisResponse);
                            _order.AddToOrder(items);
                        }
                    }
                    PrintOrder(_order);                
                    break;
                case "RemoveFromOrder":
                    if(luisResponse.CompositeEntities != null) 
                    {
                        if(luisResponse.CompositeEntities.Length > 0) 
                        {
                            var items = GetCompositeItemDetails(luisResponse);
                            _order.RemoveFromOrder(items);
                        }
                    }
                    PrintOrder(_order);                
                    break;
                case "None":
                    Console.WriteLine("None intent.");
                    break;
                default:
                    break;
            }

            if (luisResponse.Query.ToUpper().Contains("EXIT") || luisResponse.Query.ToUpper().Contains("WINDOW"))
            {
                return true;
            }
            return false;
        }

        public List<MenuItem> GetCompositeItemDetails(LuisWrapper luisWrapper) 
        {
            List<MenuItem> items = new List<MenuItem>();

            foreach(var compositeEntity in luisWrapper.CompositeEntities) 
            {
                var item = new MenuItem();
                foreach(var child in compositeEntity.Children)
                {
                    switch(child.Type)
                    {
                        case "Drink.Item":
                            item.Name = child.Value;
                            break; 
                        case "Drink.Size":
                            item.Size = child.Value;
                            break;
                        case "Drink.Modifier":
                            item.Modifiers.Add(child.Value);
                            break;
                        default:
                            break;
                    }
                }
                items.Add(item);
            }
            return items;
        }

        public void PrintOrder(Order order) 
        {   
            Console.WriteLine("Current Order: ");
            foreach(var o in order.OrderItems)
            {
                Console.WriteLine($"\t{o.Size}");
                Console.WriteLine($"\t{o.Name}");
            }
        }
    }
}
