using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIToolkitDemo
{
    public class GameManager : MonoBehaviour
    {
        public const int MAX_MESSAGES = 30;
        public GameObject chatPanel;
        public GameObject chatMessageObject;
        public TMP_InputField chatBox;
        public CompanionManager CompanionManager;

        public Color PlayerColor, CompanionColor, SystemColor;

        private bool isCompanionJoined = false;
        
        
        [SerializeField]
        private List<Message> _messages = new List<Message>();

        void Update()
        {
            if (chatBox.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SendMessageToChat(chatBox.text, Message.MessageSender.Player);
                    CompanionManager.ContinueConversation(chatBox.text);
                    chatBox.text = "";
                }                
            }


            if (!isCompanionJoined && Input.GetKeyDown(KeyCode.Space))
            {
                SendMessageToChat("Conversation started", Message.MessageSender.Sys);
                CompanionManager.StartConversation();
                isCompanionJoined = true;
            }
        }

        public void SendMessageToChat(string text, Message.MessageSender sender)
        {
            if (_messages.Count > MAX_MESSAGES)
            {
                Destroy(_messages[0].textObj.gameObject);
                _messages.Remove(_messages[0]);
            }
            
            GameObject newText = Instantiate(chatMessageObject, chatPanel.transform);

            var message = new Message();
            
            message.text = text;
            message.sender = sender;
            message.textObj = newText.GetComponent<TextMeshProUGUI>();
            message.textObj.text = "\n" + message.text;
            message.senderObj = newText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            message.senderObj.color = GetColorBySender(message.sender);
            message.senderObj.text = message.sender.ToString() + ":";
            
            _messages.Add(message);
        }

        Color GetColorBySender(Message.MessageSender sender)
        {
            switch (sender)
            {
                case Message.MessageSender.Player:
                    return PlayerColor;
                case Message.MessageSender.Companion:
                    return CompanionColor;
                case Message.MessageSender.Sys:
                    return SystemColor;
            }

            return SystemColor;
        }

    }
    
    [System.Serializable]   
    public class Message
    {
        public TextMeshProUGUI textObj;
        public TextMeshProUGUI senderObj;
        public string text;
        public MessageSender sender;


        public enum MessageSender
        {
            Player,
            Companion,
            Sys,
        }
    }
}
