using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class CompanionManager : MonoBehaviour
    {
        public GameManager GameManager;
        public int UserId { get; set; } = -1;

        private const string StartConversationUrl = "http://localhost:3007/start";
        private string ContinueConversationUrl => $"http://localhost:3007/chat/{UserId}";

        public void StartConversation()
        {
            StartCoroutine(postRequest(StartConversationUrl, "",
                request =>
                {
                    var jResponse = JObject.Parse(request.downloadHandler.text);
                    
                    UserId = jResponse.GetValue("userID").Value<int>();
                    string message = jResponse.GetValue("response").Value<string>();

                    message = message.Replace("Flying Cat: ", "").Trim().Trim('"');
                    
                    GameManager.SendMessageToChat(message, Message.MessageSender.Companion);
                    return 0;
                }));
        }
        
        public void ContinueConversation(string message)    
        {
            string json = JsonConvert.SerializeObject(new JSONMessage { message = message });
            
            StartCoroutine(postRequest(ContinueConversationUrl, json,
                request =>
            {
                var jResponse = JObject.Parse(request.downloadHandler.text);
                
                string message = jResponse.GetValue("response").Value<string>();
                
                message = message.Replace("Flying Cat: ", "").Trim().Trim('"');
                
                GameManager.SendMessageToChat(message, Message.MessageSender.Companion);
                return 0;
            }));
        }

        IEnumerator postRequest(string url, string json, Func<UnityWebRequest, int> handler)
        {
            var uwr = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
                handler(uwr);
            }
        }

        class JSONMessage
        {
            public string message;
        }
    }
}
