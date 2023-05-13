using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infura.SDK.Common;
using Infura.SDK.Models;
using Infura.SDK.Network;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using Truffle.Contracts;
using Truffle.Data;

#pragma warning disable CS0618

namespace Infura.SDK.Organization
{
    /// <summary>
    /// 
    /// </summary>
    public class OrgApiClient : IApiClient
    {
        /// <summary>
        /// 
        /// </summary>
        public const string NFT_API_URL = "https://platform.consensys-nft.com";

        public const string ADMIN_API_URL = "https://admin-api.consensys-nft.com";

        public const string PUBLIC_API_URL = "https://public-api.consensys-nft.com";

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("This should be switched over to PublicHttpClient")]
        public IHttpService NFTHttpClient { get; }

        public IHttpService HttpClient
        {
            get
            {
                return AdminHttpClient;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IHttpService AdminHttpClient { get; }
        
        public IHttpService PublicHttpClient { get; }

        /// <summary>
        /// 
        /// </summary>
        public Network.Ipfs IpfsClient { get; }
        
        public ApiCallBuilder Builder { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="ipfs"></param>
        /// <exception cref="ArgumentException"></exception>
        public OrgApiClient(string apiKey, Network.Ipfs ipfs = null, ApiCallBuilder builder = null)
        {
            if (builder == null)
                builder = new ApiCallBuilder();

            this.Builder = builder;
            
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Expected non-null apiKey string");

            this.NFTHttpClient = HttpServiceFactory.NewHttpService(NFT_API_URL, apiKey, "CNFT-Api-Key");
            this.AdminHttpClient = HttpServiceFactory.NewHttpService(ADMIN_API_URL, apiKey, "CNFT-Api-Key");
            this.PublicHttpClient = HttpServiceFactory.NewHttpService(PUBLIC_API_URL, "");
            
            IpfsClient = ipfs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<CollectionData[]> GetAllCollections()
        {
            var apiUrl = $"/api/v2/collections";

            var json = await this.NFTHttpClient.Get(apiUrl);

            var data = JsonConvert.DeserializeObject<CollectionData[]>(json);

            if (data == null) throw new Exception("Failed to get all collections");
            foreach (var productResponse in data)
            {
                productResponse.Client = this;
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<CollectionData> GetCollection(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId))
                throw new ArgumentException("Invalid collectionId");

            var apiUrl = $"/api/v2/collections/{collectionId}";

            var json = await this.NFTHttpClient.Get(apiUrl);

            var data = JsonConvert.DeserializeObject<CollectionData>(json);

            if (data == null) throw new Exception("Could not find collection");
            data.Client = this;

            return data;
        }

        public async Task<ItemData[]> GetItemsFromCollection(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId))
                throw new ArgumentException("Invalid collectionId");
            
            var apiUrl = $"/api/v2/collections/{collectionId}/items";

            var json = await this.NFTHttpClient.Get(apiUrl);

            var data = JsonConvert.DeserializeObject<ItemData[]>(json);
            if (data == null) throw new Exception("Could not find collection");

            var items = new List<ItemData>();
            foreach (var itemData in data)
            {
                var id = itemData.Id;

                var adminItem = await GetAdminItemById(id);

                if (adminItem != null)
                {
                    adminItem.Client = this;

                    items.Add(adminItem);
                }
            }

            return items.ToArray();
        }

        public async Task<ItemData> GetAdminItemById(string id)
        {
            var itemApiUrl = $"/v1/items/{id}";

            var itemJson = await AdminHttpClient.Get(itemApiUrl);

            var adminItem = JsonConvert.DeserializeObject<ItemData>(itemJson);
            return adminItem;
        }
        
        public async Task<ItemData> GetPublicItemById(string id)
        {
            var itemApiUrl = $"/v1/items/{id}";

            var itemJson = await PublicHttpClient.Get(itemApiUrl);

            var item = JsonConvert.DeserializeObject<ItemData>(itemJson);
            return item;
        }

        /// <summary>
        /// Get a specific item from a given collection. 
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<ItemData> GetItemFromCollection(string collectionId, string itemId)
        {
            return (await GetItemsFromCollection(collectionId)).FirstOrDefault(i => i.Id == itemId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="tokenId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<GenericMetadataResponse<T>> GetTokenMetadata<T>(string collectionId, string tokenId) where T : IMetadata
        {
            if (string.IsNullOrWhiteSpace(collectionId))
                throw new ArgumentException("Invalid collectionId");
            
            if (string.IsNullOrWhiteSpace(tokenId))
                throw new ArgumentException("Invalid tokenId");

            var apiUrl = $"/api/v2/collections/{collectionId}/metadata/{tokenId}";

            var json = await this.NFTHttpClient.Get(apiUrl);

            var data = JsonConvert.DeserializeObject<GenericMetadataResponse<T>>(json);

            if (data == null) throw new Exception("Could not find collection");

            return data;
        }
        
        /// <summary>
        /// Create new Listing 
        /// </summary>
        /// <param name="body"> (optional)</param>
        /// <returns>ListingOutput</returns>
        
        public async Task<ListingOutput> CreateListing(ListingInput body)
        {
            var apiUrl = $"/v1/listings";
            
            var json = await this.AdminHttpClient.Post(apiUrl, JsonConvert.SerializeObject(body));

            var data = JsonConvert.DeserializeObject<ListingOutput>(json);

            if (data == null) throw new Exception("Failed to create listing");
            
            return data;
        }
        
        /// <summary>
        /// Gets all listings
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns>ListingOutput</returns>
        
        public PreparedApiCall<ListingOutputPage, ListingOutput> GetListings()
        {
            var apiUrl = $"/v1/listings";

            return Builder.BuildApiCall<ListingOutputPage, ListingOutput>(this, apiUrl);
        }
        
        /// <summary>
        /// Gets a listing by Id
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns>ListingOutput</returns>
        
        public async Task<ListingOutput> GetListingById(Guid listingId)
        {
            var apiUrl = $"/v1/listings/{listingId}";
            
            var json = await this.AdminHttpClient.Get(apiUrl);

            var data = JsonConvert.DeserializeObject<ListingOutput>(json);

            if (data == null) throw new Exception("Failed to get listing");
            
            return data;
        }
        
        public async Task<ListingOutput> GetListingById(string listingId)
        {
            var apiUrl = $"/v1/listings/{listingId}";
            
            var json = await this.AdminHttpClient.Get(apiUrl);

            var data = JsonConvert.DeserializeObject<ListingOutput>(json);

            if (data == null) throw new Exception("Failed to get listing");
            
            return data;
        }

        public PreparedApiCall<GenericPage<PurchaseIntentDetails>, PurchaseIntentDetails>
            GetMintVoucherPurchaseIntent(ListingOutput listing, string ethAddress)
        {
            var apiUrl = $"/v1/purchase-intents?listing_id={listing.Id}&eth_address={ethAddress}";

            return Builder.BuildApiCall<GenericPage<PurchaseIntentDetails>, PurchaseIntentDetails>(this, apiUrl);
        }

        public async Task<MintVoucherPurchaseIntent> CreateMintVoucherPurchaseIntent(ListingOutput listing, string ethAddress, int quantity = 1)
        {
            string provider = "MINT_VOUCHER";
            
            var intent = new PurchaseIntentRequest()
            {
                Provider = provider,
                ListingId = listing.Id.ToString(),
                Quantity = quantity,
                Buyer = new PurchaseIntentRequest.PurchaseIntentBuyer()
                {
                    EthAddress = ethAddress
                }
            };

            var apiUrl = $"/v1/purchase-intents";

            var @params = JsonConvert.SerializeObject(intent);

            var json = await this.PublicHttpClient.Post(apiUrl, @params);

            return JsonConvert.DeserializeObject<MintVoucherPurchaseIntent>(json);
        }

        public async Task<TransactionReceipt> SendPurchaseIntent(Web3 web3, MintVoucherPurchaseIntent purchaseIntent)
        {
            var service = new FrozenERC1155Service(web3, purchaseIntent.Data.Contract);

            return await service.MintWithVoucherRequestAndWaitForReceiptAsync(new MintVoucher()
            {
                Currency = purchaseIntent.Data.Voucher.CurrencyAddress,
                Expiry = purchaseIntent.Data.Voucher.Expiry,
                InitialRecipient = purchaseIntent.Data.Voucher.InitialRecipient,
                InitialRecipientAmount = purchaseIntent.Data.Voucher.InitialRecipientAmount,
                NetRecipient = purchaseIntent.Data.Voucher.NetRecipient,
                Nonce = purchaseIntent.Data.Voucher.Nonce,
                Price = purchaseIntent.Data.Voucher.Price,
                Quantity = purchaseIntent.Data.Voucher.Quantity,
                TokenId = purchaseIntent.Data.Voucher.TokenId
            }, purchaseIntent.Data.Signature.HexToByteArray());
        }

        public Task ProcessItem<T>(T item)
        {
            return Task.CompletedTask;
        }
    }
}