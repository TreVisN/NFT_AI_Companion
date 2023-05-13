using System.Collections;
using System.Collections.Generic;
using MetaMask.SocketIOClient;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class DontDestroyWebSocket : MonoBehaviour
    {
        void Awake()
        {
            var objs = FindObjectsOfType<WebSocketDispatcher>();

            if (objs.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}
