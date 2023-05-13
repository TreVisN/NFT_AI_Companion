using System;
using MetaMask.NEthereum;
using MetaMask.Unity;
using UnityEngine;

namespace Infura.Unity
{
    public class Test : MonoBehaviour
    {
        private InfuraSdk infura;

        private async void Start()
        {
            infura = FindObjectOfType<InfuraSdk>();

            var org = await infura.LinkOrganizationCustody("abc");

            var ethAddress = MetaMaskUnity.Instance.Wallet.SelectedAddress;
            var web3 = MetaMaskUnity.Instance.CreateWeb3();
            
            var listing = await org.GetListingById("abc");
            var intent = await org.CreateMintVoucherPurchaseIntent(listing, ethAddress);
            var tx = await org.SendPurchaseIntent(web3, intent);
            
            Debug.Log(tx.TransactionHash);

            await infura.SdkReadyTask;

            var results = infura.API.SearchNfts("poap");

            results.AsObservable().Subscribe(n => Debug.Log(n.Name));
        }
    }
}