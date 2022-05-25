using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR.Serialization;
using Unity3dApi;
using UnityEngine;

public class NFTWrapper
{
    /// <summary>Deploys StandartToken contract and returns txid of deployment transaction.</summary>
    public static async Task<string> DeployNFTContractAsync(StratisUnityManager stratisUnityManager, string name, string symbol,
        bool ownerOnlyMinting, string royaltyRecipient, double royaltyPercent)
    {
        uint royaltyPercentInt = (uint) (royaltyPercent * 100);

        List<string> constructorParameter = new List<string>()
        {
            $"{(int)MethodParameterDataType.String}#{name}",
            $"{(int)MethodParameterDataType.String}#{symbol}",
            $"{(int)MethodParameterDataType.Bool}#{ownerOnlyMinting}",
            $"{(int)MethodParameterDataType.Address}#{royaltyRecipient}",
            $"{(int)MethodParameterDataType.UInt}#{royaltyPercentInt}"
        };

        string txId = await stratisUnityManager.SendCreateContractTransactionAsync(WhitelistedContracts.NFTContract.ByteCode, constructorParameter.ToArray(), 0);
        Debug.Log("Contract deployment tx sent. TxId: " + txId);

        return txId;
    }

    private readonly StratisUnityManager stratisUnityManager;
    private readonly string contractAddress;

    public NFTWrapper(StratisUnityManager stratisUnityManager, string contractAddress)
    {
        this.stratisUnityManager = stratisUnityManager;
        this.contractAddress = contractAddress;
    }

    /// <summary>Owner of the contract is responsible to for minting/burning.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> OwnerAsync()
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "Owner",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return localCallResult.Return.ToString();
    }

    /// <summary>Name for non-fungible token contract.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> NameAsync()
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "Name",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return localCallResult.Return.ToString();
    }

    /// <summary>Symbol for non-fungible token contract.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> SymbolAsync()
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "Symbol",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return localCallResult.Return.ToString();
    }

    /// <summary>Royalty Info.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<RoyaltyInfo> RoyaltyInfoAsync(ulong salePrice)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "RoyaltyInfo",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>()
            {
                $"{(int)MethodParameterDataType.UInt256}#{0}",
                $"{(int)MethodParameterDataType.ULong}#{salePrice}"
            }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);

        List<object> result = JsonConvert.DeserializeObject<List<object>>(localCallResult.Return.ToString());

        string royaltyAddr = result[0].ToString();
        ulong royaltyAmount = ulong.Parse(result[1].ToString());

        RoyaltyInfo info = new RoyaltyInfo()
        {
            RecipientAddress = royaltyAddr,
            RoyaltyAmount = royaltyAmount
        };

        return info;
    }

    /// <summary>True if provided interface is supported.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<bool> SupportsInterfaceAsync(uint interfaceId)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "SupportsInterface",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.UInt}#{interfaceId}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return bool.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Returns the number of NFTs owned by 'owner'. NFTs assigned to the zero address are
    /// considered invalid, and this function throws for queries about the zero address.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<UInt256> BalanceOfAsync(string addr)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "BalanceOf",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.Address}#{addr}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return UInt256.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Returns the address of the owner of the NFT. NFTs assigned to zero address are considered invalid, and queries about them do throw.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> OwnerOfAsync(UInt256 tokenId)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "OwnerOf",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.UInt256}#{tokenId}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return localCallResult.Return.ToString();
    }

    /// <summary>Get the approved address for a single NFT.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> GetApprovedAsync(UInt256 tokenId)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "GetApproved",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.UInt256}#{tokenId}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return localCallResult.Return.ToString();
    }

    /// <summary>Checks if 'operator' is an approved operator for 'owner'.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<bool> GetApprovedForAllAsync(string ownerAddress, string operatorAddress)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "IsApprovedForAll",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.Address}#{ownerAddress}", $"{(int)MethodParameterDataType.Address}#{operatorAddress}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return bool.Parse(localCallResult.Return.ToString());
    }

    /// <remarks>Local call.</remarks>
    /// <remarks>Local call.</remarks>
    public async Task<string> TokenURIAsync(UInt256 tokenId)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "TokenURI",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.UInt256}#{tokenId}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData);
        return localCallResult.Return.ToString();
    }

    /// <summary>Transfers the ownership of an NFT from one address to another address. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> SafeTransferFromAsync(string addrFrom, string addrTo, UInt256 tokenId, byte[] data)
    {
        var bytesString = BitConverter.ToString(data).Replace("-", "");

        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrFrom}",
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.UInt256}#{tokenId}",
            $"{(int)MethodParameterDataType.ByteArray}#{bytesString}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SafeTransferFrom", parameters.ToArray());
    }

    /// <summary>Transfers the ownership of an NFT from one address to another address. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> SafeTransferFromAsync(string addrFrom, string addrTo, UInt256 tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrFrom}",
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.UInt256}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SafeTransferFrom", parameters.ToArray());
    }

    /// <summary>Transfers the ownership of an NFT from one address to another address. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> TransferFromAsync(string addrFrom, string addrTo, UInt256 tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrFrom}",
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.UInt256}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "TransferFrom", parameters.ToArray());
    }

    /// <summary>Set or reaffirm the approved address for an NFT. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> ApproveAsync(string addr, UInt256 tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addr}",
            $"{(int)MethodParameterDataType.UInt256}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "Approve", parameters.ToArray());
    }

    /// <summary>Enables or disables approval for a third party ("operator") to manage all of sender's assets. It also Logs the ApprovalForAll event.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> SetApprovalForAllAsync(string addr, UInt256 tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addr}",
            $"{(int)MethodParameterDataType.UInt256}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SetApprovalForAll", parameters.ToArray());
    }

    /// <summary>Sets the contract owner who can mint/burn.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> TransferOwnershipAsync(string addr)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addr}",
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "TransferOwnership", parameters.ToArray());
    }

    /// <summary>Mints new tokens</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> MintAsync(string addrTo, string uri)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.String}#{uri}",
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "Mint", parameters.ToArray());
    }

    /// <summary>Mints new tokens</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> SafeMintAsync(string addrTo, string uri, byte[] data)
    {
        var bytesString = BitConverter.ToString(data).Replace("-", "");

        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.String}#{uri}",
            $"{(int)MethodParameterDataType.ByteArray}#{bytesString}",
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SafeMint", parameters.ToArray());
    }

    /// <summary>Burns a tokens</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> BurnAsync(UInt256 tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.UInt256}#{tokenId}",
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "Burn", parameters.ToArray());
    }
}

public partial class TransferLogEvent
{
    [JsonProperty("event")]
    public string Event { get; set; }

    [JsonProperty("from")]
    public string From { get; set; }

    [JsonProperty("to")]
    public string To { get; set; }

    [JsonProperty("tokenId")]
    public UInt256 TokenId { get; set; }
}

public class RoyaltyInfo
{
    public string RecipientAddress { get; set; }

    public ulong RoyaltyAmount { get; set; }
}