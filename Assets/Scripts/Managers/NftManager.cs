using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infura.SDK;
using Infura.SDK.Organization;
using Infura.Unity;
using MetaMask.NEthereum;
using MetaMask.Unity;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace UIToolkitDemo
{
    public class NftManager : MonoBehaviour
    {
        public static event Action OwnerNftsRetrieved;

        [SerializeField]
        private InfuraSdk Infura;
        
        [SerializeField]
        private MetaMaskUnity MetaMaskObj;
        
        [SerializeField]
        private string OrganizationApiKey;
        
        [SerializeField]
        private List<NftCollection> NftCollections;
        
        // For testing purposes if skipping login - assign it here
        private string walletAddress;

        private bool organizationLinked;
        private bool ownersItemsReceived;
        private bool gettingOwnersItems;

        // Used for testing and to avoid querying NFT server over and over again
        private List<ItemData> preUnlockedItems = new ();
        private List<ItemData> ownersItems = new ();
        
        // NFT server items and listings
        private OrgApiClient organization;
        private ListingOutput[] listings;
        
        void Awake()
        {
            var objs = FindObjectsOfType<NftManager>();

            if (objs.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            
            LoginManager.WalletReceived += OnWalletReceived;
            LoginManager.EnteredMainMenu += OnEnteredMainMenu;
        }

        private void OnDestroy()
        {
            LoginManager.WalletReceived -= OnWalletReceived;
            LoginManager.EnteredMainMenu -= OnEnteredMainMenu;
        }

        private void Start()
        {
            FindInfuraObject();
            
            LinkNftOrganization();
            GetNftItemCollections();
            GetNftItemListings();
        }

        private void FindInfuraObject()
        {
            if (Infura)
                return;
            
            Infura = FindObjectOfType<InfuraSdk>();

            if (!Infura)
                Debug.LogError("NFTManager: Infura SDK object could not be found, please create one and assign it.");
        }

        private void OnWalletReceived(string address)
        {
            walletAddress = address;
            
            if (gettingOwnersItems || ownersItemsReceived)
                return;
            
            GetOwnersItemCollection();
        }

        private void OnEnteredMainMenu()
        {
            OwnerNftsRetrieved?.Invoke();
        }
        
        private async void GetOwnersItemCollection()
        {
            if (string.IsNullOrEmpty(walletAddress))
                return;
            
            // If the organization has not been linked yet - await
            gettingOwnersItems = true;
            
            while (!organizationLinked)
                await Task.Delay(100);
                
            Debug.Log($"NftManager: Getting NFTs from the address: {walletAddress}");
            var nfts = await Infura.API.GetNfts(walletAddress).AsListAsync();
            
            foreach (var nft in nfts)
                NftReceived(nft);
            
            Debug.Log($"NftManager: Received {nfts.Count} NFTs. They are: {string.Join(",", nfts.Select(x => x.Name))}");

            ownersItemsReceived = true;
            OwnerNftsRetrieved?.Invoke();
        }

        private async void LinkNftOrganization()
        {
            if (!Infura)
                return;

            if (string.IsNullOrEmpty(OrganizationApiKey))
                return;
            
            organization = await Infura.LinkOrganizationCustody(OrganizationApiKey);
            organizationLinked = true;
        }

        private async void GetNftItemCollections()
        {
            while (!organizationLinked)
                await Task.Delay(100);

            foreach (var coll in NftCollections)
            {
                var items = await organization.GetItemsFromCollection(coll.Id);
                items = (from p in items orderby p.TokenId select p).ToArray();
                
                coll.SetItems(items);
                Debug.Log($"NftManager: {coll.Name} NFTs: {string.Join(",", coll.GetItems().Select(x => x.Id))}");
            }
        }

        private async void GetNftItemListings()
        {
            while (!organizationLinked)
                await Task.Delay(100);
            
            Debug.Log("NftManager: Getting NFT listings");
            
            var lists = await organization.GetListings().AsListAsync();
            listings = lists.ToArray();

            Debug.Log($"NftManager: Listings retrieved. {lists.Count}");
            foreach (var listing in listings)
            {
                Debug.Log($"NftManager: Listings: {listing.ItemId} {listing.Price}");
            }
        }
        
        public async Task<string> MintItem(ItemData item)
        {
            var txHash = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(walletAddress)) 
                    txHash = await item.Mint(walletAddress);
            }
            catch (Exception e)
            {
                Debug.LogError($"NftManager: Minting Exception: {e.Message}");
            }
            return txHash;
        }

        public async Task<TransactionReceipt> PurchaseItem(ItemData item)
        {
            var listing = listings.First(x => x.ItemId.ToString() == item.Id);
            
            var purchaseIntent = await organization.CreateMintVoucherPurchaseIntent(listing, MetaMaskUnity.Instance.Wallet.SelectedAddress);
            
            var web3 = MetaMaskObj.CreateWeb3();
            var tx = await organization.SendPurchaseIntent(web3, purchaseIntent);
            
            Debug.Log($"NftManager: PurchaseItem txHash {tx.TransactionHash}");
            return tx;
        }
        
        private void NftReceived(NftItem nftItem)
        {
            var itemData = GetItemByContract(nftItem.Contract, (int)nftItem.TokenId);

            if (itemData == null)
            {
                Debug.Log($"NftManager: NftReceived: NftItem was not found in game's collections. Name: {nftItem.Name}");
                return;
            }

            ownersItems.Add(itemData);
        }

        public bool GetIsItemOwned(string itemId)
        {
            return ownersItems.Any(item => item.Id == itemId);
        }
        
        public bool GetIsItemPreOwned(string itemId)
        {
            return preUnlockedItems.Any(item => item.Id == itemId);
        }

        private ItemData GetItemByContract(string conAddress, int tokenId)
        {
            var collection = NftCollections.FirstOrDefault(x => x.ContractAddress.ToLower() == conAddress);

            if (collection == null)
                return null;

            return collection.GetItem(tokenId - 1, out var itemData) ? itemData : null;
        }
        
        public ItemData GetItemById(string itemId)
        {
            return NftCollections.SelectMany(coll => coll.GetItems()).FirstOrDefault(item => item.Id == itemId);
        }
        

        public void AddUnlockedItem(ItemData itemData)
        {
            if (!preUnlockedItems.Contains(itemData))
                preUnlockedItems.Add(itemData);
        }
    }
    
    [System.Serializable]
    public class NftCollection
    {
        public string Name;
        public string Id;
        public string ContractAddress;

        private ItemData[] items;

        public void SetItems(ItemData[] itemDataArray)
        {
            items = itemDataArray;
        }

        public ItemData[] GetItems()
        {
            return items;
        }

        public bool GetItem(int index, out ItemData itemData)
        {
            if (index >= items.Length)
            {
                itemData = null;
                return false;
            }

            itemData = items[index];
            return true;
        }
    }
}
