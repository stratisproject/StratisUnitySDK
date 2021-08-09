using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stratis.SmartContracts.CLR.Serialization;
using Unity3dApi;
using UnityEngine;

public class NFTWrapper
{
    /// <summary>Deploys StandartToken contract and returns txid of deployment transaction.</summary>
    public static async Task<string> DeployNFTContractAsync(StratisUnityManager stratisUnityManager, string name, string symbol, string tokenURIFormat, bool ownerOnlyMinting)
    {
        List<string> constructorParameter = new List<string>()
        {
            $"{(int)MethodParameterDataType.String}#{name}",
            $"{(int)MethodParameterDataType.String}#{symbol}",
            $"{(int)MethodParameterDataType.String}#{tokenURIFormat}",
            $"{(int)MethodParameterDataType.Bool}#{ownerOnlyMinting}"
        };

        string txId = await stratisUnityManager.SendCreateContractTransactionAsync(WhitelistedContracts.NFTContract.ByteCode, constructorParameter.ToArray(), 0).ConfigureAwait(false);
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
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
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
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
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
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return localCallResult.Return.ToString();
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
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return bool.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Returns the number of NFTs owned by 'owner'. NFTs assigned to the zero address are
    /// considered invalid, and this function throws for queries about the zero address.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<ulong> BalanceOfAsync(string addr)
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
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return ulong.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Returns the address of the owner of the NFT. NFTs assigned to zero address are considered invalid, and queries about them do throw.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> OwnerOfAsync(ulong tokenId)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "OwnerOf",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.ULong}#{tokenId}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return localCallResult.Return.ToString();
    }

    /// <summary>Get the approved address for a single NFT.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> GetApprovedAsync(ulong tokenId)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "GetApproved",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.ULong}#{tokenId}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return localCallResult.Return.ToString();
    }

    /// <summary>Checks if 'operator' is an approved operator for 'owner'.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<bool> GetApprovedForAllAsync(string ownderAddress, string operatorAddress)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "IsApprovedForAll",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.ULong}#{ownderAddress}", $"{(int)MethodParameterDataType.ULong}#{operatorAddress}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return bool.Parse(localCallResult.Return.ToString());
    }

    /// <remarks>Local call.</remarks>
    /// <remarks>Local call.</remarks>
    public async Task<string> TokenURIAsync(ulong tokenId)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "TokenURI",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.ULong}#{tokenId}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return localCallResult.Return.ToString();
    }

    /// <summary>Transfers the ownership of an NFT from one address to another address. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> SafeTransferFromAsync(string addrFrom, string addrTo, ulong tokenId, byte[] data)
    {
        var bytesString = BitConverter.ToString(data).Replace("-", "");

        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrFrom}",
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.ULong}#{tokenId}",
            $"{(int)MethodParameterDataType.ByteArray}#{bytesString}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SafeTransferFrom", parameters.ToArray());
    }

    /// <summary>Transfers the ownership of an NFT from one address to another address. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> SafeTransferFromAsync(string addrFrom, string addrTo, ulong tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrFrom}",
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.ULong}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SafeTransferFrom", parameters.ToArray());
    }

    /// <summary>Transfers the ownership of an NFT from one address to another address. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> TransferFromAsync(string addrFrom, string addrTo, ulong tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrFrom}",
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.ULong}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "TransferFrom", parameters.ToArray());
    }
    
    /// <summary>Set or reaffirm the approved address for an NFT. This function can be changed to payable.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> ApproveAsync(string addr, ulong tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addr}",
            $"{(int)MethodParameterDataType.ULong}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "Approve", parameters.ToArray());
    }

    /// <summary>Enables or disables approval for a third party ("operator") to manage all of sender's assets. It also Logs the ApprovalForAll event.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> SetApprovalForAllAsync(string addr, ulong tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addr}",
            $"{(int)MethodParameterDataType.ULong}#{tokenId}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SetApprovalForAll", parameters.ToArray());
    }

    /// <summary>Sets the contract owner who can mint/burn.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task TransferOwnershipAsync(string addr)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addr}",
        };

        await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "TransferOwnership", parameters.ToArray());
    }

    /// <summary>Mints new tokens</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> MintAsync(string addrTo)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "Mint", parameters.ToArray());
    }

    /// <summary>Mints new tokens</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task SafeMintAsync(string addrTo, byte[] data)
    {
        var bytesString = BitConverter.ToString(data).Replace("-", "");

        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addrTo}",
            $"{(int)MethodParameterDataType.ByteArray}#{bytesString}",
        };

        await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "SafeMint", parameters.ToArray());
    }

    /// <summary>Burns a tokens</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task BurnAsync(ulong tokenId)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.ULong}#{tokenId}",
        };

        await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "Burn", parameters.ToArray());
    }
}
