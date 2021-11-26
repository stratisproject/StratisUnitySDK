using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stratis.SmartContracts.CLR.Serialization;
using Unity3dApi;
using UnityEngine;

public class StandartTokenWrapper
{
    /// <summary>Deploys StandartToken contract and returns txid of deployment transaction.</summary>
    public static async Task<string> DeployStandartTokenAsync(StratisUnityManager stratisUnityManager, ulong totalSupply, string name, string symbol, uint decimals)
    {
        List<string> constructorParameter = new List<string>()
        {
            $"{(int)MethodParameterDataType.ULong}#{totalSupply}",
            $"{(int)MethodParameterDataType.String}#{name}",
            $"{(int)MethodParameterDataType.String}#{symbol}",
            $"{(int)MethodParameterDataType.UInt}#{decimals}"
        };

        string txId = await stratisUnityManager.SendCreateContractTransactionAsync(WhitelistedContracts.StandartTokenContract.ByteCode, constructorParameter.ToArray(), 0).ConfigureAwait(false);
        Debug.Log("Contract deployment tx sent. TxId: " + txId);

        return txId;
    }

    private readonly StratisUnityManager stratisUnityManager;
    private readonly string contractAddress;

    public StandartTokenWrapper(StratisUnityManager stratisUnityManager, string contractAddress)
    {
        this.stratisUnityManager = stratisUnityManager;
        this.contractAddress = contractAddress;
    }

    /// <summary>Provides token symbol.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> GetSymbolAsync()
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "Symbol",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>()
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return localCallResult.Return.ToString();
    }

    /// <summary>Provides token name.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<string> GetNameAsync()
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "Name",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>()
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return localCallResult.Return.ToString();
    }

    /// <summary>Provides token total supply.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<ulong> GetTotalSupplyAsync()
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "TotalSupply",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>()
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return ulong.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Provides token balance of a given address.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<ulong> GetBalanceAsync(string address)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "GetBalance",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.Address}#{address}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return ulong.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Provides token decimals count.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<uint> GetDecimalsAsync()
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "GetDecimals",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>()
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return uint.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Provides spending allowance.</summary>
    /// <remarks>Local call.</remarks>
    public async Task<ulong> GetAllowanceAsync(string addressOwner, string addressSpender)
    {
        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = this.contractAddress,
            MethodName = "Allowance",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>() { $"{(int)MethodParameterDataType.Address}#{addressOwner}", $"{(int)MethodParameterDataType.Address}#{addressSpender}" }
        };
        LocalExecutionResult localCallResult = await this.stratisUnityManager.Client.LocalCallAsync(localCallData).ConfigureAwait(false);
        return ulong.Parse(localCallResult.Return.ToString());
    }

    /// <summary>Transfers specified amount of token to the given address.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> TransferToAsync(string address, ulong amount)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{address}", 
            $"{(int)MethodParameterDataType.ULong}#{amount}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "TransferTo", parameters.ToArray());
    }

    /// <summary>Transfers specified amount of token to the given address from another given address.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> TransferFromAsync(string addressFrom, string addressTo, ulong amount)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{addressFrom}",
            $"{(int)MethodParameterDataType.Address}#{addressTo}",
            $"{(int)MethodParameterDataType.ULong}#{amount}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "TransferFrom", parameters.ToArray());
    }

    /// <summary>Sets allowance for the given address.</summary>
    /// <remarks>Normal call. Use returned txId to get receipt in order to get return value once transaction is mined. Return value is of <c>bool</c> type.</remarks>
    public async Task<string> ApproveAsync(string spender, ulong currentAmount, ulong amount)
    {
        List<string> parameters = new List<string>()
        {
            $"{(int)MethodParameterDataType.Address}#{spender}",
            $"{(int)MethodParameterDataType.ULong}#{currentAmount}",
            $"{(int)MethodParameterDataType.ULong}#{amount}"
        };

        return await this.stratisUnityManager.SendCallContractTransactionAsync(this.contractAddress, "Approve", parameters.ToArray());
    }
}
