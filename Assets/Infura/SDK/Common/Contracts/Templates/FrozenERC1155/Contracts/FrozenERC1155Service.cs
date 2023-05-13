using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Truffle.Functions;
using Truffle.Data;

namespace Truffle.Contracts
{
    public partial class FrozenERC1155Service
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, FrozenERC1155Deployment frozenERC1155Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<FrozenERC1155Deployment>().SendRequestAndWaitForReceiptAsync(frozenERC1155Deployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, FrozenERC1155Deployment frozenERC1155Deployment)
        {
            return web3.Eth.GetContractDeploymentHandler<FrozenERC1155Deployment>().SendRequestAsync(frozenERC1155Deployment);
        }

        public static async Task<FrozenERC1155Service> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, FrozenERC1155Deployment frozenERC1155Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, frozenERC1155Deployment, cancellationTokenSource);
            return new FrozenERC1155Service(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public FrozenERC1155Service(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public FrozenERC1155Service(Nethereum.Web3.IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<byte[]> DefaultAdminRoleQueryAsync(DefaultAdminRoleFunction defaultAdminRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DefaultAdminRoleFunction, byte[]>(defaultAdminRoleFunction, blockParameter);
        }

        
        public Task<byte[]> DefaultAdminRoleQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DefaultAdminRoleFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> SignerRoleQueryAsync(SignerRoleFunction signerRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SignerRoleFunction, byte[]>(signerRoleFunction, blockParameter);
        }

        
        public Task<byte[]> SignerRoleQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SignerRoleFunction, byte[]>(null, blockParameter);
        }

        public Task<string> MintWithVoucherRequestAsync(MintWithVoucherFunction mintWithVoucherFunction)
        {
             return ContractHandler.SendRequestAsync(mintWithVoucherFunction);
        }

        public Task<TransactionReceipt> MintWithVoucherRequestAndWaitForReceiptAsync(MintWithVoucherFunction mintWithVoucherFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mintWithVoucherFunction, cancellationToken);
        }

        public Task<string> MintWithVoucherRequestAsync(MintVoucher voucher, byte[] signature)
        {
            var mintWithVoucherFunction = new MintWithVoucherFunction();
                mintWithVoucherFunction.Voucher = voucher;
                mintWithVoucherFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(mintWithVoucherFunction);
        }

        public Task<TransactionReceipt> MintWithVoucherRequestAndWaitForReceiptAsync(MintVoucher voucher, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var mintWithVoucherFunction = new MintWithVoucherFunction
            {
                Voucher = voucher,
                Signature = signature
            };

            if (mintWithVoucherFunction.Voucher.Currency == "0x0000000000000000000000000000000000000000" ||
                mintWithVoucherFunction.Voucher.Currency.ToLower() == "eth")
                {
                    mintWithVoucherFunction.AmountToSend = mintWithVoucherFunction.Voucher.Price *
                                                           mintWithVoucherFunction.Voucher.Quantity;
                }
            
                return ContractHandler.SendRequestAndWaitForReceiptAsync(mintWithVoucherFunction, cancellationToken);
        }

        public Task<string> AdminMintRequestAsync(AdminMintFunction adminMintFunction)
        {
             return ContractHandler.SendRequestAsync(adminMintFunction);
        }

        public Task<TransactionReceipt> AdminMintRequestAndWaitForReceiptAsync(AdminMintFunction adminMintFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(adminMintFunction, cancellationToken);
        }

        public Task<string> AdminMintRequestAsync(string to, BigInteger tokenId, BigInteger amount)
        {
            var adminMintFunction = new AdminMintFunction();
                adminMintFunction.To = to;
                adminMintFunction.TokenId = tokenId;
                adminMintFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(adminMintFunction);
        }

        public Task<TransactionReceipt> AdminMintRequestAndWaitForReceiptAsync(string to, BigInteger tokenId, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var adminMintFunction = new AdminMintFunction();
                adminMintFunction.To = to;
                adminMintFunction.TokenId = tokenId;
                adminMintFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(adminMintFunction, cancellationToken);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction balanceOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        
        public Task<BigInteger> BalanceOfQueryAsync(string account, BigInteger id, BlockParameter blockParameter = null)
        {
            var balanceOfFunction = new BalanceOfFunction();
                balanceOfFunction.Account = account;
                balanceOfFunction.Id = id;
            
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        public Task<List<BigInteger>> BalanceOfBatchQueryAsync(BalanceOfBatchFunction balanceOfBatchFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfBatchFunction, List<BigInteger>>(balanceOfBatchFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> BalanceOfBatchQueryAsync(List<string> accounts, List<BigInteger> ids, BlockParameter blockParameter = null)
        {
            var balanceOfBatchFunction = new BalanceOfBatchFunction();
                balanceOfBatchFunction.Accounts = accounts;
                balanceOfBatchFunction.Ids = ids;
            
            return ContractHandler.QueryAsync<BalanceOfBatchFunction, List<BigInteger>>(balanceOfBatchFunction, blockParameter);
        }

        public Task<string> BurnRequestAsync(BurnFunction burnFunction)
        {
             return ContractHandler.SendRequestAsync(burnFunction);
        }

        public Task<TransactionReceipt> BurnRequestAndWaitForReceiptAsync(BurnFunction burnFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFunction, cancellationToken);
        }

        public Task<string> BurnRequestAsync(BigInteger tokenId, BigInteger amount)
        {
            var burnFunction = new BurnFunction();
                burnFunction.TokenId = tokenId;
                burnFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(burnFunction);
        }

        public Task<TransactionReceipt> BurnRequestAndWaitForReceiptAsync(BigInteger tokenId, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var burnFunction = new BurnFunction();
                burnFunction.TokenId = tokenId;
                burnFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFunction, cancellationToken);
        }

        public Task<string> BurnFixMeOverloadedRequestAsync(BurnFixMeOverloadedFunction burnFixMeOverloadedFunction)
        {
             return ContractHandler.SendRequestAsync(burnFixMeOverloadedFunction);
        }

        public Task<TransactionReceipt> BurnFixMeOverloadedRequestAndWaitForReceiptAsync(BurnFixMeOverloadedFunction burnFixMeOverloadedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFixMeOverloadedFunction, cancellationToken);
        }

        public Task<string> BurnFixMeOverloadedRequestAsync(string owner, BigInteger tokenId, BigInteger amount)
        {
            var burnFixMeOverloadedFunction = new BurnFixMeOverloadedFunction();
                burnFixMeOverloadedFunction.Owner = owner;
                burnFixMeOverloadedFunction.TokenId = tokenId;
                burnFixMeOverloadedFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(burnFixMeOverloadedFunction);
        }

        public Task<TransactionReceipt> BurnFixMeOverloadedRequestAndWaitForReceiptAsync(string owner, BigInteger tokenId, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var burnFixMeOverloadedFunction = new BurnFixMeOverloadedFunction();
                burnFixMeOverloadedFunction.Owner = owner;
                burnFixMeOverloadedFunction.TokenId = tokenId;
                burnFixMeOverloadedFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFixMeOverloadedFunction, cancellationToken);
        }

        public Task<string> CancelVoucherRequestAsync(CancelVoucherFunction cancelVoucherFunction)
        {
             return ContractHandler.SendRequestAsync(cancelVoucherFunction);
        }

        public Task<TransactionReceipt> CancelVoucherRequestAndWaitForReceiptAsync(CancelVoucherFunction cancelVoucherFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(cancelVoucherFunction, cancellationToken);
        }

        public Task<string> CancelVoucherRequestAsync(BigInteger voucherNonce)
        {
            var cancelVoucherFunction = new CancelVoucherFunction();
                cancelVoucherFunction.VoucherNonce = voucherNonce;
            
             return ContractHandler.SendRequestAsync(cancelVoucherFunction);
        }

        public Task<TransactionReceipt> CancelVoucherRequestAndWaitForReceiptAsync(BigInteger voucherNonce, CancellationTokenSource cancellationToken = null)
        {
            var cancelVoucherFunction = new CancelVoucherFunction();
                cancelVoucherFunction.VoucherNonce = voucherNonce;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(cancelVoucherFunction, cancellationToken);
        }

        public Task<string> CreateForAdminMintRequestAsync(CreateForAdminMintFunction createForAdminMintFunction)
        {
             return ContractHandler.SendRequestAsync(createForAdminMintFunction);
        }

        public Task<TransactionReceipt> CreateForAdminMintRequestAndWaitForReceiptAsync(CreateForAdminMintFunction createForAdminMintFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createForAdminMintFunction, cancellationToken);
        }

        public Task<string> CreateForAdminMintRequestAsync(BigInteger tokenid, BigInteger initialsupply, BigInteger maxsupply, string uri)
        {
            var createForAdminMintFunction = new CreateForAdminMintFunction();
                createForAdminMintFunction.Tokenid = tokenid;
                createForAdminMintFunction.Initialsupply = initialsupply;
                createForAdminMintFunction.Maxsupply = maxsupply;
                createForAdminMintFunction.Uri = uri;
            
             return ContractHandler.SendRequestAsync(createForAdminMintFunction);
        }

        public Task<TransactionReceipt> CreateForAdminMintRequestAndWaitForReceiptAsync(BigInteger tokenid, BigInteger initialsupply, BigInteger maxsupply, string uri, CancellationTokenSource cancellationToken = null)
        {
            var createForAdminMintFunction = new CreateForAdminMintFunction();
                createForAdminMintFunction.Tokenid = tokenid;
                createForAdminMintFunction.Initialsupply = initialsupply;
                createForAdminMintFunction.Maxsupply = maxsupply;
                createForAdminMintFunction.Uri = uri;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createForAdminMintFunction, cancellationToken);
        }

        public Task<BigInteger> GetLastNonceQueryAsync(GetLastNonceFunction getLastNonceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetLastNonceFunction, BigInteger>(getLastNonceFunction, blockParameter);
        }

        
        public Task<BigInteger> GetLastNonceQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var getLastNonceFunction = new GetLastNonceFunction();
                getLastNonceFunction.Account = account;
            
            return ContractHandler.QueryAsync<GetLastNonceFunction, BigInteger>(getLastNonceFunction, blockParameter);
        }

        public Task<byte[]> GetRoleAdminQueryAsync(GetRoleAdminFunction getRoleAdminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRoleAdminFunction, byte[]>(getRoleAdminFunction, blockParameter);
        }

        
        public Task<byte[]> GetRoleAdminQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getRoleAdminFunction = new GetRoleAdminFunction();
                getRoleAdminFunction.Role = role;
            
            return ContractHandler.QueryAsync<GetRoleAdminFunction, byte[]>(getRoleAdminFunction, blockParameter);
        }

        public Task<string> GrantRoleRequestAsync(GrantRoleFunction grantRoleFunction)
        {
             return ContractHandler.SendRequestAsync(grantRoleFunction);
        }

        public Task<TransactionReceipt> GrantRoleRequestAndWaitForReceiptAsync(GrantRoleFunction grantRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantRoleFunction, cancellationToken);
        }

        public Task<string> GrantRoleRequestAsync(byte[] role, string account)
        {
            var grantRoleFunction = new GrantRoleFunction();
                grantRoleFunction.Role = role;
                grantRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(grantRoleFunction);
        }

        public Task<TransactionReceipt> GrantRoleRequestAndWaitForReceiptAsync(byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var grantRoleFunction = new GrantRoleFunction();
                grantRoleFunction.Role = role;
                grantRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantRoleFunction, cancellationToken);
        }

        public Task<bool> HasRoleQueryAsync(HasRoleFunction hasRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HasRoleFunction, bool>(hasRoleFunction, blockParameter);
        }

        
        public Task<bool> HasRoleQueryAsync(byte[] role, string account, BlockParameter blockParameter = null)
        {
            var hasRoleFunction = new HasRoleFunction();
                hasRoleFunction.Role = role;
                hasRoleFunction.Account = account;
            
            return ContractHandler.QueryAsync<HasRoleFunction, bool>(hasRoleFunction, blockParameter);
        }

        public Task<bool> IsApprovedForAllQueryAsync(IsApprovedForAllFunction isApprovedForAllFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }

        
        public Task<bool> IsApprovedForAllQueryAsync(string account, string @operator, BlockParameter blockParameter = null)
        {
            var isApprovedForAllFunction = new IsApprovedForAllFunction();
                isApprovedForAllFunction.Account = account;
                isApprovedForAllFunction.Operator = @operator;
            
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }

        public Task<bool> IsCreatedQueryAsync(IsCreatedFunction isCreatedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsCreatedFunction, bool>(isCreatedFunction, blockParameter);
        }

        
        public Task<bool> IsCreatedQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var isCreatedFunction = new IsCreatedFunction();
                isCreatedFunction.TokenId = tokenId;
            
            return ContractHandler.QueryAsync<IsCreatedFunction, bool>(isCreatedFunction, blockParameter);
        }

        public Task<BigInteger> MaxSupplyQueryAsync(MaxSupplyFunction maxSupplyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MaxSupplyFunction, BigInteger>(maxSupplyFunction, blockParameter);
        }

        
        public Task<BigInteger> MaxSupplyQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var maxSupplyFunction = new MaxSupplyFunction();
                maxSupplyFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<MaxSupplyFunction, BigInteger>(maxSupplyFunction, blockParameter);
        }

        public Task<string> NameQueryAsync(NameFunction nameFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(nameFunction, blockParameter);
        }

        
        public Task<string> NameQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(null, blockParameter);
        }

        public Task<bool> OperatorFilteringEnabledQueryAsync(OperatorFilteringEnabledFunction operatorFilteringEnabledFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OperatorFilteringEnabledFunction, bool>(operatorFilteringEnabledFunction, blockParameter);
        }

        
        public Task<bool> OperatorFilteringEnabledQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OperatorFilteringEnabledFunction, bool>(null, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> RenounceOwnershipRequestAsync(RenounceOwnershipFunction renounceOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(renounceOwnershipFunction);
        }

        public Task<string> RenounceOwnershipRequestAsync()
        {
             return ContractHandler.SendRequestAsync<RenounceOwnershipFunction>();
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(RenounceOwnershipFunction renounceOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceOwnershipFunction, cancellationToken);
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<RenounceOwnershipFunction>(null, cancellationToken);
        }

        public Task<string> RenounceRoleRequestAsync(RenounceRoleFunction renounceRoleFunction)
        {
             return ContractHandler.SendRequestAsync(renounceRoleFunction);
        }

        public Task<TransactionReceipt> RenounceRoleRequestAndWaitForReceiptAsync(RenounceRoleFunction renounceRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceRoleFunction, cancellationToken);
        }

        public Task<string> RenounceRoleRequestAsync(byte[] role, string account)
        {
            var renounceRoleFunction = new RenounceRoleFunction();
                renounceRoleFunction.Role = role;
                renounceRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(renounceRoleFunction);
        }

        public Task<TransactionReceipt> RenounceRoleRequestAndWaitForReceiptAsync(byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var renounceRoleFunction = new RenounceRoleFunction();
                renounceRoleFunction.Role = role;
                renounceRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceRoleFunction, cancellationToken);
        }

        public Task<string> ResetTokenRoyaltyRequestAsync(ResetTokenRoyaltyFunction resetTokenRoyaltyFunction)
        {
             return ContractHandler.SendRequestAsync(resetTokenRoyaltyFunction);
        }

        public Task<TransactionReceipt> ResetTokenRoyaltyRequestAndWaitForReceiptAsync(ResetTokenRoyaltyFunction resetTokenRoyaltyFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(resetTokenRoyaltyFunction, cancellationToken);
        }

        public Task<string> ResetTokenRoyaltyRequestAsync(BigInteger tokenId)
        {
            var resetTokenRoyaltyFunction = new ResetTokenRoyaltyFunction();
                resetTokenRoyaltyFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAsync(resetTokenRoyaltyFunction);
        }

        public Task<TransactionReceipt> ResetTokenRoyaltyRequestAndWaitForReceiptAsync(BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var resetTokenRoyaltyFunction = new ResetTokenRoyaltyFunction();
                resetTokenRoyaltyFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(resetTokenRoyaltyFunction, cancellationToken);
        }

        public Task<string> RevokeRoleRequestAsync(RevokeRoleFunction revokeRoleFunction)
        {
             return ContractHandler.SendRequestAsync(revokeRoleFunction);
        }

        public Task<TransactionReceipt> RevokeRoleRequestAndWaitForReceiptAsync(RevokeRoleFunction revokeRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeRoleFunction, cancellationToken);
        }

        public Task<string> RevokeRoleRequestAsync(byte[] role, string account)
        {
            var revokeRoleFunction = new RevokeRoleFunction();
                revokeRoleFunction.Role = role;
                revokeRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(revokeRoleFunction);
        }

        public Task<TransactionReceipt> RevokeRoleRequestAndWaitForReceiptAsync(byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var revokeRoleFunction = new RevokeRoleFunction();
                revokeRoleFunction.Role = role;
                revokeRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeRoleFunction, cancellationToken);
        }

        public Task<RoyaltyInfoOutputDTO> RoyaltyInfoQueryAsync(RoyaltyInfoFunction royaltyInfoFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<RoyaltyInfoFunction, RoyaltyInfoOutputDTO>(royaltyInfoFunction, blockParameter);
        }

        public Task<RoyaltyInfoOutputDTO> RoyaltyInfoQueryAsync(BigInteger tokenId, BigInteger salePrice, BlockParameter blockParameter = null)
        {
            var royaltyInfoFunction = new RoyaltyInfoFunction();
                royaltyInfoFunction.TokenId = tokenId;
                royaltyInfoFunction.SalePrice = salePrice;
            
            return ContractHandler.QueryDeserializingToObjectAsync<RoyaltyInfoFunction, RoyaltyInfoOutputDTO>(royaltyInfoFunction, blockParameter);
        }

        public Task<string> SafeBatchTransferFromRequestAsync(SafeBatchTransferFromFunction safeBatchTransferFromFunction)
        {
             return ContractHandler.SendRequestAsync(safeBatchTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeBatchTransferFromRequestAndWaitForReceiptAsync(SafeBatchTransferFromFunction safeBatchTransferFromFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeBatchTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeBatchTransferFromRequestAsync(string from, string to, List<BigInteger> ids, List<BigInteger> amounts, byte[] data)
        {
            var safeBatchTransferFromFunction = new SafeBatchTransferFromFunction();
                safeBatchTransferFromFunction.From = from;
                safeBatchTransferFromFunction.To = to;
                safeBatchTransferFromFunction.Ids = ids;
                safeBatchTransferFromFunction.Amounts = amounts;
                safeBatchTransferFromFunction.Data = data;
            
             return ContractHandler.SendRequestAsync(safeBatchTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeBatchTransferFromRequestAndWaitForReceiptAsync(string from, string to, List<BigInteger> ids, List<BigInteger> amounts, byte[] data, CancellationTokenSource cancellationToken = null)
        {
            var safeBatchTransferFromFunction = new SafeBatchTransferFromFunction();
                safeBatchTransferFromFunction.From = from;
                safeBatchTransferFromFunction.To = to;
                safeBatchTransferFromFunction.Ids = ids;
                safeBatchTransferFromFunction.Amounts = amounts;
                safeBatchTransferFromFunction.Data = data;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeBatchTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(SafeTransferFromFunction safeTransferFromFunction)
        {
             return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(SafeTransferFromFunction safeTransferFromFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(string from, string to, BigInteger tokenId, BigInteger amount, byte[] data)
        {
            var safeTransferFromFunction = new SafeTransferFromFunction();
                safeTransferFromFunction.From = from;
                safeTransferFromFunction.To = to;
                safeTransferFromFunction.TokenId = tokenId;
                safeTransferFromFunction.Amount = amount;
                safeTransferFromFunction.Data = data;
            
             return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger tokenId, BigInteger amount, byte[] data, CancellationTokenSource cancellationToken = null)
        {
            var safeTransferFromFunction = new SafeTransferFromFunction();
                safeTransferFromFunction.From = from;
                safeTransferFromFunction.To = to;
                safeTransferFromFunction.TokenId = tokenId;
                safeTransferFromFunction.Amount = amount;
                safeTransferFromFunction.Data = data;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(SetApprovalForAllFunction setApprovalForAllFunction)
        {
             return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(SetApprovalForAllFunction setApprovalForAllFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(string @operator, bool approved)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
                setApprovalForAllFunction.Operator = @operator;
                setApprovalForAllFunction.Approved = approved;
            
             return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(string @operator, bool approved, CancellationTokenSource cancellationToken = null)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
                setApprovalForAllFunction.Operator = @operator;
                setApprovalForAllFunction.Approved = approved;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<string> SetDefaultRoyaltyRequestAsync(SetDefaultRoyaltyFunction setDefaultRoyaltyFunction)
        {
             return ContractHandler.SendRequestAsync(setDefaultRoyaltyFunction);
        }

        public Task<TransactionReceipt> SetDefaultRoyaltyRequestAndWaitForReceiptAsync(SetDefaultRoyaltyFunction setDefaultRoyaltyFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setDefaultRoyaltyFunction, cancellationToken);
        }

        public Task<string> SetDefaultRoyaltyRequestAsync(string receiver, BigInteger feeNumerator)
        {
            var setDefaultRoyaltyFunction = new SetDefaultRoyaltyFunction();
                setDefaultRoyaltyFunction.Receiver = receiver;
                setDefaultRoyaltyFunction.FeeNumerator = feeNumerator;
            
             return ContractHandler.SendRequestAsync(setDefaultRoyaltyFunction);
        }

        public Task<TransactionReceipt> SetDefaultRoyaltyRequestAndWaitForReceiptAsync(string receiver, BigInteger feeNumerator, CancellationTokenSource cancellationToken = null)
        {
            var setDefaultRoyaltyFunction = new SetDefaultRoyaltyFunction();
                setDefaultRoyaltyFunction.Receiver = receiver;
                setDefaultRoyaltyFunction.FeeNumerator = feeNumerator;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setDefaultRoyaltyFunction, cancellationToken);
        }

        public Task<string> SetOperatorFilteringEnabledRequestAsync(SetOperatorFilteringEnabledFunction setOperatorFilteringEnabledFunction)
        {
             return ContractHandler.SendRequestAsync(setOperatorFilteringEnabledFunction);
        }

        public Task<TransactionReceipt> SetOperatorFilteringEnabledRequestAndWaitForReceiptAsync(SetOperatorFilteringEnabledFunction setOperatorFilteringEnabledFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOperatorFilteringEnabledFunction, cancellationToken);
        }

        public Task<string> SetOperatorFilteringEnabledRequestAsync(bool value)
        {
            var setOperatorFilteringEnabledFunction = new SetOperatorFilteringEnabledFunction();
                setOperatorFilteringEnabledFunction.Value = value;
            
             return ContractHandler.SendRequestAsync(setOperatorFilteringEnabledFunction);
        }

        public Task<TransactionReceipt> SetOperatorFilteringEnabledRequestAndWaitForReceiptAsync(bool value, CancellationTokenSource cancellationToken = null)
        {
            var setOperatorFilteringEnabledFunction = new SetOperatorFilteringEnabledFunction();
                setOperatorFilteringEnabledFunction.Value = value;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOperatorFilteringEnabledFunction, cancellationToken);
        }

        public Task<string> SetTokenRoyaltyRequestAsync(SetTokenRoyaltyFunction setTokenRoyaltyFunction)
        {
             return ContractHandler.SendRequestAsync(setTokenRoyaltyFunction);
        }

        public Task<TransactionReceipt> SetTokenRoyaltyRequestAndWaitForReceiptAsync(SetTokenRoyaltyFunction setTokenRoyaltyFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setTokenRoyaltyFunction, cancellationToken);
        }

        public Task<string> SetTokenRoyaltyRequestAsync(BigInteger tokenId, string receiver, BigInteger feeNumerator)
        {
            var setTokenRoyaltyFunction = new SetTokenRoyaltyFunction();
                setTokenRoyaltyFunction.TokenId = tokenId;
                setTokenRoyaltyFunction.Receiver = receiver;
                setTokenRoyaltyFunction.FeeNumerator = feeNumerator;
            
             return ContractHandler.SendRequestAsync(setTokenRoyaltyFunction);
        }

        public Task<TransactionReceipt> SetTokenRoyaltyRequestAndWaitForReceiptAsync(BigInteger tokenId, string receiver, BigInteger feeNumerator, CancellationTokenSource cancellationToken = null)
        {
            var setTokenRoyaltyFunction = new SetTokenRoyaltyFunction();
                setTokenRoyaltyFunction.TokenId = tokenId;
                setTokenRoyaltyFunction.Receiver = receiver;
                setTokenRoyaltyFunction.FeeNumerator = feeNumerator;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setTokenRoyaltyFunction, cancellationToken);
        }

        public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction supportsInterfaceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        
        public Task<bool> SupportsInterfaceQueryAsync(byte[] interfaceId, BlockParameter blockParameter = null)
        {
            var supportsInterfaceFunction = new SupportsInterfaceFunction();
                supportsInterfaceFunction.InterfaceId = interfaceId;
            
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        public Task<string> SymbolQueryAsync(SymbolFunction symbolFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(symbolFunction, blockParameter);
        }

        
        public Task<string> SymbolQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> TotalSupplyQueryAsync(TotalSupplyFunction totalSupplyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(totalSupplyFunction, blockParameter);
        }

        
        public Task<BigInteger> TotalSupplyQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var totalSupplyFunction = new TotalSupplyFunction();
                totalSupplyFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(totalSupplyFunction, blockParameter);
        }

        public Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction transferOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction transferOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(string newOwner)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> UriQueryAsync(UriFunction uriFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UriFunction, string>(uriFunction, blockParameter);
        }

        
        public Task<string> UriQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var uriFunction = new UriFunction();
                uriFunction.TokenId = tokenId;
            
            return ContractHandler.QueryAsync<UriFunction, string>(uriFunction, blockParameter);
        }
    }
}
