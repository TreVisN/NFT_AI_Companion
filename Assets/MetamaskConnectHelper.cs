using System.Collections;
using System.Collections.Generic;
using MetaMask.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class MetamaskConnectHelper : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MetaMaskUnity.Instance.Initialize();
        var wallet = MetaMaskUnity.Instance.Wallet;
        wallet.Connect();
    }
}
