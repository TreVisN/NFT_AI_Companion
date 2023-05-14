using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UIToolkitDemo
{
    public class GameManager : MonoBehaviour
    {
        public const int MAX_MESSAGES = 30;
        public const int MAX_COMPANION_MESSAGES = 1;
        public const int MAX_PLAYER_MESSAGES = 3;
        public GameObject chatPanel;
        public GameObject CompanionChatPanel;
        public GameObject chatMessageObject;
        public GameObject chatMessageObjectForCanvas;
        public TMP_InputField chatBox;
        public CompanionManager CompanionManager;


        public GameObject userChatPanel;

        public Color PlayerColor, CompanionColor, SystemColor;

        private bool sessionStarted = false;
        
        
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


            if (userChatPanel.activeSelf && !sessionStarted)
            {
                SendMessageToChat("Conversation started", Message.MessageSender.Sys);
                CompanionManager.StartConversation();
                sessionStarted = true;
            }
        }

        public void SendMessageToChat(string text, Message.MessageSender sender)
        {
            var panel = sender == Message.MessageSender.Companion ? CompanionChatPanel.transform : chatPanel.transform;
            var messageObject = sender == Message.MessageSender.Companion ? chatMessageObject : chatMessageObjectForCanvas;
            
            
            if (_messages.Count > MAX_MESSAGES) 
            {
                Destroy(_messages[0].textObj.gameObject);
                
                _messages.Remove(_messages[0]);
            }

            if (_messages.Count(x => x.sender == Message.MessageSender.Companion) >= MAX_COMPANION_MESSAGES)
            {
                var messageToDelete = _messages.FirstOrDefault(x => x.sender == Message.MessageSender.Companion);
                Debug.Log("Destroying message:" + messageToDelete.text);
                Destroy(messageToDelete.messageObject);
                _messages.Remove(messageToDelete);
            }

            if (_messages.Count(x => x.sender == Message.MessageSender.Player) >= MAX_PLAYER_MESSAGES)
            {
                var messageToDelete = _messages.FirstOrDefault(x => x.sender == Message.MessageSender.Player);
                Debug.Log("Destroying message:" + messageToDelete.text);
                Destroy(messageToDelete.messageObject);
                _messages.Remove(messageToDelete);
            }
            
            GameObject newMessage = Instantiate(messageObject, panel);

            var message = new Message();
            
            message.text = text;
            message.sender = sender;
            message.messageObject = newMessage;
            message.textObj = message.messageObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            message.textObj.text = "\n" + message.text;
            message.senderObj = message.textObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
        public GameObject messageObject;
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
