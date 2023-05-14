using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Infura.SDK.Common;
using Infura.SDK.Models;
using Infura.SDK.Network;
using Infura.SDK.Organization;
using Newtonsoft.Json;

namespace Infura.SDK
{
    /// <summary>
    /// The Infura NFT API client.
    /// </summary>
    public class ApiClient : IApiClient
    {
        #region Constants
        /// <summary>
        /// The base URL for the Infura NFT API
        /// </summary>
        private const string NftApiUrl = "https://nft.api.infura.io";
        #endregion
        
        #region Fields
        /// <summary>
        /// The Authentication configuration this API Client is using for requests
        /// </summary>
        public Auth Auth { get; }
        
        /// <summary>
        /// The base URL for the Infura NFT API this API Client is using
        /// </summary>
        public string ApiPath { get; private set; }
        
        /// <summary>
        /// The HTTP Client this API Client is using for requests
        /// </summary>
        public IHttpService HttpClient { get; }

        /// <summary>
        /// The IPFS Client this API Client is using for IPFS requests
        /// </summary>
        public Network.Ipfs IpfsClient { get; }
        
        public ApiCallBuilder Builder { get; }
        #endregion

        #region Private Fields
        internal readonly Dictionary<string, OrgApiClient> OrgApiClients = new Dictionary<string, OrgApiClient>();
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new API Client with the given auth credentials and configuration
        /// </summary>
        /// <param name="auth">The authentication configuration/credentials to use</param>
        /// <param name="builder">The <see cref="ApiCallBuilder"/> responsible for building the <see cref="PreparedApiCall{TR,T}"/> objects</param>
        /// <exception cref="ArgumentException">If no auth object is given</exception>
        public ApiClient(Auth auth, ApiCallBuilder builder = null)
        {
            if (builder == null)
                builder = new ApiCallBuilder();

            this.Builder = builder;
            
            this.Auth = auth ?? throw new ArgumentException("Expected non-null Auth object");

            this.ApiPath = $"/networks/{(int) auth.ChainId}";

            this.HttpClient = HttpServiceFactory.NewHttpService(NftApiUrl, Auth.ApiAuth);

            IpfsClient = auth.Ipfs;
        }
        #endregion
        
        #region Transfers
        /// <summary>
        /// Get all transfers from a block to a block.
        /// </summary>
        /// <param name="fromBlock">The start block number to get transfers from</param>
        /// <param name="toBlock">The end block number to get transfers from</param>
        /// <returns>The full list of transfers between Blocks fromBlock to toBlock</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfers(BigInteger fromBlock, BigInteger toBlock) => 
            Builder.BuildApiCall<TransfersResponse, TransfersResult>(
                this, $"{ApiPath}/nfts/transfers?fromBlock={fromBlock}&toBlock={toBlock}"
                );

        /// <summary>
        /// Get all transfers from a single block by block number.
        /// </summary>
        /// <param name="blockNumber">The block number to get transfers from</param>
        /// <returns>The full list of transfers from the block at the specified blockNumber.</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfersByBlock(BigInteger blockNumber) => 
            Builder.BuildApiCall<TransfersResponse, TransfersResult>(
                this, $"{ApiPath}/nfts/block/transfers?blockHashNumber={blockNumber}"
            );

        /// <summary>
        /// Get all transfers from a user by their wallet address.
        /// </summary>
        /// <param name="publicAddress">The wallet address to query transfers from</param>
        /// <returns>The full list of transfers from the specified wallet address.</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfersByWallet(string publicAddress) => 
            Builder.BuildApiCall<TransfersResponse, TransfersResult>(
                this, $"{ApiPath}/accounts/{publicAddress}/assets/transfers"
            );

        /// <summary>
        /// Get all transfers for an NFT from a specific NFT collection.
        /// </summary>
        /// <param name="contractAddress">The contract address of the NFT collection the token belogns to</param>
        /// <param name="tokenId">The token Id of the NFT to get transfers for</param>
        /// <returns>The full list of transfers from the specified NFT.</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfersForNft(string contractAddress, string tokenId) => 
            Builder.BuildApiCall<TransfersResponse, TransfersResult>(
                this, $"{ApiPath}/nfts/{contractAddress}/tokens/{tokenId}/transfers"
            );

        /// <summary>
        /// Get all transfers for an NFT from a specific NFT collection.
        /// </summary>
        /// <param name="nftItem">The NFT to get transfers for</param>
        /// <returns>The full list of transfers from the specified NFT.</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfersForNft(NftItem nftItem) =>
            GetTransfersForNft(nftItem.Contract, nftItem.TokenId.ToString());

        /// <summary>
        /// Get all transfers for all NFTs in a given NFT collection.
        /// </summary>
        /// <param name="contractAddress">The contract address of the NFT collection to get transfers from</param>
        /// <returns>The full list of transfers from the specified NFT collection.</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfersForNftCollection(string contractAddress) => 
            Builder.BuildApiCall<TransfersResponse, TransfersResult>(
                this, $"{ApiPath}/nfts/{contractAddress}/transfers");

        /// <summary>
        /// Get all transfers for all NFTs in a given NFT collection.
        /// </summary>
        /// <param name="collection">The NFT collection to get transfers from</param>
        /// <returns>The full list of transfers from the specified NFT collection.</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfersForNftCollection(NftCollection collection) =>
            GetTransfersForNftCollection(collection.Contract);

        /// <summary>
        /// Get all transfers for all NFTs in a given NFT collection.
        /// </summary>
        /// <param name="nftItem">The NFT that belongs to the NFT collection to get transfers from</param>
        /// <returns>The full list of transfers from the specified NFT collection.</returns>
        public PreparedApiCall<TransfersResponse, TransfersResult> GetTransfersForNftCollection(NftItem nftItem) =>
            GetTransfersForNftCollection(nftItem.Contract);
        #endregion
        
        #region Metadata
        /// <summary>
        /// Get a list of all NFTs a user owns at a given wallet address.
        /// </summary>
        /// <param name="publicAddress">The wallet address to query NFTs for</param>
        /// <returns>The full list of NFTs owned by the given wallet address</returns>
        public PreparedApiCall<NftAssetsResponse, NftItem> GetNfts(string publicAddress) => Builder.BuildApiCall<NftAssetsResponse, NftItem>(this, $"{ApiPath}/accounts/{publicAddress}/assets/nfts");

        /// <summary>
        /// Grab and deserialize specific metadata for a NFT at the given contract address and token ID. This function
        /// will deserialize the JSON into the given type T, where T is of type IMetadata.
        /// </summary>
        /// <param name="contractAddress">The contract address / collection the NFT belongs to</param>
        /// <param name="tokenId">The token ID of the NFT to get Metadata for</param>
        /// <typeparam name="T">The type of Metadata to deserialize and return.</typeparam>
        /// <returns>The Metadata for the NFT as type T</returns>
        /// <exception cref="ArgumentException">If the contract address is not provided or tokenId is not provided</exception>
        public async Task<T> GetTokenMetadata<T>(string contractAddress, string tokenId) where T : IMetadata
        {
            if (string.IsNullOrWhiteSpace(contractAddress) || string.IsNullOrWhiteSpace(tokenId))
                throw new ArgumentException($"Invalid address: contractAddress ({contractAddress}) or tokenId ({tokenId}) is null or empty");

            var apiUrl = $"{ApiPath}/nfts/{contractAddress}/tokens/{tokenId}";

            var data = await Get<GenericMetadataResponse<T>>(apiUrl);

            return data != null ? data.Metadata : default;
        }
        
        /// <summary>
        /// Grab and deserialize specific metadata for a given NFT item. This function
        /// will deserialize the JSON into the given type T, where T is of type IMetadata.
        /// </summary>
        /// <param name="nftItem">The NFT to grab Metadata for</param>
        /// <typeparam name="T">The type of Metadata to deserialize and return.</typeparam>
        /// <returns>The Metadata for the NFT as type T</returns>
        public Task<T> GetTokenMetadata<T>(NftItem nftItem) where T : IMetadata =>
            GetTokenMetadata<T>(nftItem.Contract, nftItem.TokenId.ToString());

        /// <summary>
        /// Grab and deserialize specific metadata for a given NFT item. This function
        /// will deserialize the JSON into the type Metadata. The Metadata class ia a general-purpose
        /// Metadata class that covers most usecases. If you need more specific Metadata, use the
        /// generic version of this function <see cref="GetTokenMetadata{T} (string, string)"/>
        /// </summary>
        /// <param name="contractAddress">The contract address / collection the NFT belongs to</param>
        /// <param name="tokenId">The token ID of the NFT to get Metadata for</param>
        /// <returns>The Metadata for the NFT</returns>
        /// <exception cref="ArgumentException">If the contract address is not provided or tokenId is not provided</exception>
        public Task<Metadata> GetTokenMetadata(string contractAddress, string tokenId) =>
            GetTokenMetadata<Metadata>(contractAddress, tokenId);

        /// <summary>
        /// Grab and deserialize specific metadata for a given NFT item. This function
        /// will deserialize the JSON into the type Metadata. The Metadata class ia a general-purpose
        /// Metadata class that covers most usecases. If you need more specific Metadata, use the
        /// generic version of this function <see cref="GetTokenMetadata{T} (NftItem)"/>
        /// </summary>
        /// <param name="nftItem">The NFT to grab Metadata for</param>
        /// <returns>The Metadata for the NFT</returns>
        public Task<Metadata> GetTokenMetadata(NftItem nftItem) =>
            GetTokenMetadata<Metadata>(nftItem.Contract, nftItem.TokenId.ToString());

        /// <summary>
        /// Search for NFTs using the given query string. The query string can be a simple. For example, "CryptoKitty".
        /// </summary>
        /// <param name="query">The query string to search with</param>
        /// <returns>The full list of NFT results from the search query</returns>
        public PreparedApiCall<SearchNft, SearchNftResult, NftItem> SearchNfts(string query) =>
            Builder.BuildApiCall<SearchNft, SearchNftResult, NftItem>(this, $"{ApiPath}/nfts/search?query={query}", item =>
                new NftItem()
                {
                    BlockNumberMinted = item.BlockNumberMinted,
                    Contract = item.TokenAddress,
                    CreatedAt = item.CreatedAt,
                    Metadata = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.MetadataJson),
                    MinterAddress = item.MinterAddress,
                    TokenHash = item.TokenHash,
                    TokenId = item.TokenId,
                    TransactionMinted = item.TransactionMinted,
                    Type = item.ContractType
                });
        
        #endregion

        #region Collections
        /// <summary>
        /// Get a list of all NFT collections where the given user owns at least one NFT from each collection.
        /// </summary>
        /// <param name="publicAddress">The wallet address of the user to query collections for</param>
        /// <returns>The full list of NFT collections where the user owns at least one NFT from each collection.</returns>
        public PreparedApiCall<NftCollectionResponse, NftCollection> GetNftCollections(string publicAddress) => Builder.BuildApiCall<NftCollectionResponse, NftCollection>(this, $"{ApiPath}/accounts/{publicAddress}/assets/collections");

        /// <summary>
        /// Get a list of all NFTs that are from a specific collection / contract address.
        /// </summary>
        /// <param name="contractAddress">The contract address of the collection to query NFTs for</param>
        /// <returns>The full list of NFTs in the collection at the given contract address</returns>
        public PreparedApiCall<NftAssetsResponse, NftItem> GetNftsForCollection(string contractAddress) => Builder.BuildApiCall<NftAssetsResponse, NftItem> (this, $"{ApiPath}/nfts/{contractAddress}/tokens");

        /// <summary>
        /// Get the NFT Collection for a given NFT Item.
        /// <see cref="GetCollection (string)"/>
        /// </summary>
        /// <param name="item">The item to grab the NFT Collection from</param>
        /// <returns>The NFT Collection that the given NFT item belongs to</returns>
        public Task<NftCollection> GetCollectionForItem(NftItem item) => GetCollection(item.Contract);

        /// <summary>
        /// Get the NFT Collection at the given contract address
        /// </summary>
        /// <param name="contractAddress">The contract address of the collection to obtain</param>
        /// <returns>NFT Collection data at the given contract address</returns>
        /// <exception cref="ArgumentException">If the contract address given is null or empty</exception>
        public async Task<NftCollection> GetCollection(string contractAddress)
        {
            if (string.IsNullOrWhiteSpace(contractAddress))
                throw new ArgumentException("Invalid account address");

            var apiUrl = $"{ApiPath}/nfts/{contractAddress}";

            var collection = await Get<NftCollection>(apiUrl);

            if (collection == null) return null;
            
            foreach (var org in OrgApiClients.Values)
                await collection.TryLinkOrganization(org);

            return collection;
        }
        #endregion
        
        #region Ownership

        /// <summary>
        /// Get all owners of a given NFT collection.
        /// </summary>
        /// <param name="contractAddress">The contract address of the NFT collection to query owners from</param>
        /// <returns>The full list of owners from the specified NFT collection.</returns>
        public PreparedApiCall<OwnersResponse, OwnersResult> GetOwnersOfNftCollection(string contractAddress) =>
            Builder.BuildApiCall<OwnersResponse, OwnersResult>(this, $"{ApiPath}/nfts/{contractAddress}/owners");

        /// <summary>
        /// Get all owners of a given NFT collection.
        /// </summary>
        /// <param name="collection">The NFT collection to query owners from</param>
        /// <returns>The full list of owners from the specified NFT collection.</returns>
        public PreparedApiCall<OwnersResponse, OwnersResult> GetOwnersOfNftCollection(NftCollection collection) =>
            GetOwnersOfNftCollection(collection.Contract);

        /// <summary>
        /// Get a full list of owners of a given NFT.
        /// </summary>
        /// <param name="nftItem">The NFT to query owners from</param>
        /// <returns>A list of owners from the specified NFT.</returns>
        public PreparedApiCall<OwnersResponse, OwnersResult> GetOwnersOfNft(NftItem nftItem) =>
            GetOwnersOfNft(nftItem.Contract, nftItem.TokenId.ToString());

        /// <summary>
        /// Get a full list of owners of a given NFT.
        /// </summary>
        /// <param name="contractAddress">The contract address of the NFT collection the given tokenId belongs to</param>
        /// <param name="tokenId">The token Id of the NFT to query owners from</param>
        /// <returns>A list of owners from the specified NFT.</returns>
        public PreparedApiCall<OwnersResponse, OwnersResult> GetOwnersOfNft(string contractAddress, string tokenId) =>
            Builder.BuildApiCall<OwnersResponse, OwnersResult>(this, $"{ApiPath}/nfts/{contractAddress}/{tokenId}/owners");

        #endregion
        
        #region Market Data

        /// <summary>
        /// Get the lowest ETH based price for the given NFT collection.
        /// </summary>
        /// <param name="tokenAddress">The contract address of the NFT collection to get pricing data for</param>
        /// <returns>Token price information for the given NFT collection</returns>
        public Task<TokenPrice> GetTradePrice(string tokenAddress) =>
            Get<TokenPrice>($"{ApiPath}/nfts/{tokenAddress}/tradePrice");

        /// <summary>
        /// Get the lowest ETH based price for the given NFT collection.
        /// </summary>
        /// <param name="collection">The NFT collection to get pricing data for</param>
        /// <returns>Token price information for the given NFT collection</returns>
        public Task<TokenPrice> GetTradePrice(NftCollection collection) => GetTradePrice(collection.Contract);

        /// <summary>
        /// Get the lowest ETH based price for the NFT collection that the given NFT exists inside of.
        /// </summary>
        /// <param name="item">The NFT get pricing data for.</param>
        /// <returns>Token price information for the NFT collection that the given NFT exists inside of.</returns>
        public Task<TokenPrice> GetTradePrice(NftItem item) => GetTradePrice(item.Contract);
        
        #endregion

        #region IPFS
        /// <summary>
        /// Store a file on IPFS and return the CID of the file. The file given can either
        /// be a path to a file in the filesystem or a URL to a remote file.
        /// </summary>
        /// <param name="file">The path or URL of the file to store on IPFS</param>
        /// <returns>The CID of the uploaded file</returns>
        /// <exception cref="Exception">If no IPFS client is setup</exception>
        public Task<string> StoreFile(string file)
        {
            if (IpfsClient == null)
                throw new Exception("IpfsClient not setup");

            return null;
            //return IpfsClient.UploadFile(file);
        }
        
        /// <summary>
        /// Store metadata string on IPFS and return the CID of the file. The given metadata
        /// must be a JSON string.
        /// </summary>
        /// <param name="metadata">The metadata JSON string to store on IPFS</param>
        /// <returns>The CID of the uploaded metadata</returns>
        /// <exception cref="Exception">If no IPFS client is setup</exception>
        public Task<string> StoreMetadata(string metadata)
        {
            if (IpfsClient == null)
                throw new Exception("IpfsClient not setup");

            return null;
            //return IpfsClient.UploadContent(metadata);
        }
        
        /// <summary>
        /// Store metadata as a JSON string on IPFS and return the CID of the file. The given
        /// metadata object will be converted to JSON before being uploaded to IPFS. The type of
        /// Metadata must implement the IMetadata interface
        /// </summary>
        /// <param name="metadata">The metadata object to store on IPFS</param>
        /// <typeparam name="T">The type of the metadata object</typeparam>
        /// <returns>The CID of the uploaded metadata</returns>
        /// <exception cref="Exception">IF no IPFS client is setup</exception>
        public Task<string> StoreMetadata<T>(T metadata) where T : IMetadata
        {
            if (IpfsClient == null)
                throw new Exception("IpfsClient not setup");

            return StoreMetadata(JsonConvert.SerializeObject(metadata));
        }

        /// <summary>
        /// Create a folder of metadata files. This function will create a new folder where
        /// the folder contains each element passed in the given IEnumerable. Each item in the
        /// IEnumerable will be converted to a JSON string before being uploaded. The type of
        /// Metadata must implement the IMetadata interface
        /// </summary>
        /// <param name="metadata">An enumerable list of metadata to place in the new folder</param>
        /// <typeparam name="T">The type of metadata being stored</typeparam>
        /// <returns>The CID of the new folder</returns>
        /// <exception cref="Exception">If no IPFS client is setup</exception>
        public Task<string> CreateFolder<T>(IEnumerable<T> metadata) where T : IMetadata
        {
            if (IpfsClient == null)
                throw new Exception("IpfsClient not setup");

            return CreateFolder(metadata.Select(m => JsonConvert.SerializeObject(m)));
        }
        
        /// <summary>
        /// Create a folder of metadata files. This function will create a new folder where
        /// the folder contains each element passed in the given IEnumerable.
        /// </summary>
        /// <param name="metadata">An enumerable list of strings, where each string will be a new file in the resulting folder</param>
        /// <returns>The CID of the new folder</returns>
        /// <exception cref="Exception">If no IPFS client is setup</exception>
        public Task<string> CreateFolder(IEnumerable<string> metadata)
        {
            if (IpfsClient == null)
                throw new Exception("IpfsClient not setup");

            return null;
            //return IpfsClient.UploadArray(metadata);
        }
        #endregion

        #region Utils
        /// <summary>
        /// Link an organization API key to this client. This will allow the client to access
        /// specific organization data. This will also return an instance of the Organization API
        /// as an <see cref="OrgApiClient"/>
        /// </summary>
        /// <param name="organizationId">The API key of the organization to link</param>
        /// <returns>The Organization API as an <see cref="OrgApiClient"/></returns>
        public OrgApiClient LinkOrganization(string organizationId)
        {
            if (OrgApiClients.ContainsKey(organizationId))
                return OrgApiClients[organizationId];
            
            var orgApi = new OrgApiClient(organizationId, IpfsClient, Builder);
            OrgApiClients.Add(organizationId, orgApi);
            return orgApi;
        }
        
        /// <summary>
        /// Update the chain this API Client queries against
        /// </summary>
        /// <param name="chains">The new chain to use for queries</param>
        public void UpdateChain(Chains chains)
        {
            this.Auth.ChainId = chains;
            this.ApiPath = $"/networks/{(int) Auth.ChainId}";
        }
        
        public async Task ProcessItem<T>(T item)
        {
            if (item is IOrgLinkable linkable)
            {
                foreach (var org in OrgApiClients.Values)
                {
                    var linkResult = await linkable.TryLinkOrganization(org);
                    if (linkResult)
                        // We don't need to attempt linking anymore if one Organization was successful
                        break;
                }
            }
        }
        #endregion

        #region Protected Methods
        protected virtual async Task<T> Get<T>(string fullPath)
        {
            var json = await HttpClient.Get(fullPath);
            var data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }
        #endregion
    }
}