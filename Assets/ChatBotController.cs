using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Cloud.SDK;
using IBM.Watson.TextToSpeech.V1;
using IBM.Watson.TextToSpeech.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;




public class ChatBotController : MonoBehaviour
{
    public IBM.Watsson.Examples.ExampleStreaming AIListen;
    public IBM.Watson.Examples.ExampleTextToSpeechV1 AISpeak;
    public ChatGPTWrapper.ChatGPTConversation AIThink;

    public string chatBotStart = "I'm ready for conversation! What do you want to talk about?";
    public int MaxInstructionWords = 30;

    public string promptString = "";

    public bool greetingDone = false;
    public bool chatBotSpeaking = false;
    public bool chatBotThinking = false;
    public bool awaitingUser = true;
    public string lastChatBotSpeech = "";

    void Start()
    {
        AIListen.Active = false;
    }

    private IEnumerator ChatBotSpeak(string speech)
    {
        AIListen.Active = false;
        yield return AISpeak.ExampleSynthesize(speech);
        chatBotSpeaking = false;
        AIListen.Active = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Greet User, if we haven't.
        if (greetingDone == false && Time.realtimeSinceStartup > 4)
        {
            greetingDone = true;
            StartCoroutine(ChatBotSpeak(chatBotStart));
        }

        // Is user finished speaking? If so, respond based on our confidence level of what we heard.
        if (AIListen.ResultsField.text.Contains("(Final"))
        {
            promptString = "Answer all prompts as if you are a smart but cheeky chatbot in service to the team at the Emerging Media Lab. Be courteous. Only tell the truth. Your user says: " + AIListen.ResultsField.text;
            AIListen.ResultsField.text = "";
            AIListen.Active = false;
            AIThink.SendToChatGPT(promptString);
        }

        // Does ChatBot have something new to say?
        if (AIThink._lastChatGPTMsg != lastChatBotSpeech)
        {
            lastChatBotSpeech = AIThink._lastChatGPTMsg;
            StartCoroutine(ChatBotSpeak(lastChatBotSpeech));
        }
    }
}
