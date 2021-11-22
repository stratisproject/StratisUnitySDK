using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using Network = NBitcoin.Network;

public class NFTWallet : MonoBehaviour
{
    public static NFTWallet Instance;

    public string ApiUrl = "http://localhost:44336/";
    private readonly Network network = new CirrusTest();

    [HideInInspector]
    public StratisUnityManager StratisUnityManager;

    private const string WatchedNFTsKey = "watchedNFTs";

    void Awake()
    {
        Instance = this;
    }
    
    /// <returns><c>true</c> if success.</returns>
    public async UniTask<bool> InitializeAsync(string mnemonic)
    {
        try
        {
            this.StratisUnityManager = new StratisUnityManager(new Unity3dClient(ApiUrl), network,
                new Mnemonic(mnemonic, Wordlist.English));

            await this.StratisUnityManager.GetBalanceAsync();
        }
        catch (SocketException e)
        {
            return false;
        }
        catch (HttpRequestException e)
        {
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return true;
    }

    public async UniTask RegisterDeployedNFTAsync(string nftName, string symbol, bool ownerOnlyMinting, string contractAddress, string ownerAddress)
    {
        List<DeployedNFTModel> knownNfts = LoadKnownNfts();

        knownNfts.Add(new DeployedNFTModel()
        {
            ContractAddress = contractAddress,
            NftName = nftName,
            OwnerOnlyMinting = ownerOnlyMinting,
            Symbol = symbol,
            OwnerAddress = ownerAddress
        });
        
        this.PersistKnownNfts(knownNfts);

        await this.StratisUnityManager.Client.WatchNftContractAsync(contractAddress);
    }

    public List<DeployedNFTModel> LoadKnownNfts()
    {
        if (!PlayerPrefs.HasKey(WatchedNFTsKey))
            return new List<DeployedNFTModel>();

        string json = PlayerPrefs.GetString(WatchedNFTsKey);

        List<DeployedNFTModel> nfts = JsonConvert.DeserializeObject<List<DeployedNFTModel>>(json);

        return nfts;
    }

    public void PersistKnownNfts(List<DeployedNFTModel> knownNfts)
    {
        string json = JsonConvert.SerializeObject(knownNfts);
        PlayerPrefs.SetString(WatchedNFTsKey, json);
    }
}
