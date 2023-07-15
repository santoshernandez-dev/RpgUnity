using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using ElevenLabs;
using ElevenLabs.Voices;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai = new OpenAIApi();
        private string emotion = "";
        private string action = "";

        private List<ChatMessage> messages = new List<ChatMessage>();
        [SerializeField] private string prompt = "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            ChatMessage chatResponse = new ChatMessage();

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                chatResponse = completionResponse.Choices[0].Message;
                chatResponse.Content = chatResponse.Content.Trim();
                messages.Add(chatResponse);
                Debug.Log(chatResponse.Content);

                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            string CompletitionPrompt = "Imagine you are an AI model designed to understand and express human emotions in response to a given sentence. The sentence you need to react to is 'I have finally achieved my goal of running a marathon.' Express your reaction as one of the following emotions: ANGRY, NEUTRAL, HAPPY, SAD. Please respond in the format: 'EMOTION'. You can also undertand what action between the user wants you to execute between these keywords for triggering the animation: 'CAPOEIRA', 'RAP', 'WALK_AWAY' and write it when necesary following the next format: 'ACTION'\nQ: ";
            EmotionAnalysis(chatResponse.Content, CompletitionPrompt);
            PlayAudioElevenLabs(chatResponse);
            button.enabled = true;
            inputField.enabled = true;
        }

        private async void PlayAudioElevenLabs(ChatMessage chatResponse)
        {

            string cleanText = Regex.Replace(chatResponse.Content, @"\*\*.*?\*\*|\(.*?\)", string.Empty);

            var audioSource = GetComponent<AudioSource>();
            var api = new ElevenLabsClient("a77e720722dc18235dfababa458d75a0");
            var voice = (await api.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
            var defaultVoiceSettings = await api.VoicesEndpoint.GetDefaultVoiceSettingsAsync();

            var tupleResult = await api.TextToSpeechEndpoint.StreamTextToSpeechAsync(
                cleanText,
                voice,
                clip =>
                {
                    // Trigger the speaking animation when audio starts playing
                    TriggerSpeakingAnimation();

                    audioSource.PlayOneShot(clip);

                    // Wait for the audio clip to finish playing
                    float clipDuration = clip.length;
                    StartCoroutine(StopSpeakingAnimation(clipDuration));
                },
                defaultVoiceSettings);
            string clipPath = tupleResult.Item1;
            AudioClip audioClip = tupleResult.Item2;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private async void EmotionAnalysis(string userInput, string prompt)
        {
            prompt += $"{userInput}\nA: ";
            var completionResponse = await openai.CreateCompletion(new CreateCompletionRequest()
            {
                Prompt = prompt,
                Model = "text-davinci-003",
                MaxTokens = 128
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                prompt = $"{completionResponse.Choices[0].Text}\nQ: ";
                Debug.Log(completionResponse.Choices[0].Text);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            string inputString = completionResponse.Choices[0].Text;

            // Check if the input string contains any emotion keywords
            if (inputString.IndexOf("ANGRY", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                TriggerAngryAnimation();
                emotion = "ANGRY";
            }
            if (inputString.IndexOf("SAD", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                TriggerSadAnimation();
                emotion = "SAD";
            }
            if (inputString.IndexOf("NEUTRAL", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                TriggerNeutralAnimation();
                emotion = "NEUTRAL";
            }
            if (inputString.IndexOf("HAPPY", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                TriggerHappyAnimation();
                emotion = "HAPPY";
            }

            // Check if the input string contains any action keywords
            if (inputString.IndexOf("CAPOEIRA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("Triggering Capoeira Animation...");
                action = "CAPOEIRA";
                // Code to trigger the capoeira animation
            }
            if (inputString.IndexOf("RAP", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("Triggering Rap Animation...");
                action = "RAP";
                // Code to trigger the rap animation
            }
            if (inputString.IndexOf("WALK_AWAY", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("Triggering Walk Away Animation...");
                action = "WALK_AWAY";
                // Code to trigger the walk away animation
            }
        }

        static void TriggerAngryAnimation()
        {
            Debug.Log("Triggering Angry Animation...");
            // Code to trigger the angry animation
        }

        static void TriggerSadAnimation()
        {
            Debug.Log("Triggering Sad Animation...");
            // Code to trigger the sad animation
        }

        static void TriggerNeutralAnimation()
        {
            Debug.Log("Triggering Neutral Animation...");
            // Code to trigger the neutral animation
        }

        static void TriggerHappyAnimation()
        {
            Debug.Log("Triggering Happy Animation...");
            // Code to trigger the happy animation
        }

        static void TriggerSpeakingAnimation()
        {
            Debug.Log("Triggering Speaking Animation...");
            // Code to trigger the speaking animation
        }

        private IEnumerator StopSpeakingAnimation(float clipDuration)
        {
            yield return new UnityEngine.WaitForSecondsRealtime(clipDuration);
            TriggerStopSpeakingAnimation();
            Debug.Log("Stopping Speaking Animation...");
        }

        static void TriggerStopSpeakingAnimation()
        {
            Debug.Log("Triggering Stop Speaking Animation...");
            // Code to trigger the stop speaking animation
        }

    }
}
