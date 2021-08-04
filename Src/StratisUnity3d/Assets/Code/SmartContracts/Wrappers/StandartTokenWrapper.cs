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

    // TODO TransferTo TransferFrom Approve
}
