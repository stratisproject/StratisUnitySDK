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

    private readonly string apiUrl = "http://localhost:44336/";
    private readonly Network network = new CirrusTest();

    [HideInInspector]
    public StratisUnityManager StratisUnityManager;

    private const string DeployedNFTsKey = "deployedNFTs";

    void Awake()
    {
        Instance = this;
    }
    
    /// <returns><c>true</c> if success.</returns>
    public async UniTask<bool> InitializeAsync(string mnemonic)
    {
        try
        {
            this.StratisUnityManager = new StratisUnityManager(new Unity3dClient(apiUrl), network,
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

    public void RegisterDeployedNFT(string nftName, string symbol, bool ownerOnlyMinting, string contractAddress, string ownerAddress)
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
    }

    public List<DeployedNFTModel> LoadKnownNfts()
    {
        if (!PlayerPrefs.HasKey(DeployedNFTsKey))
            return new List<DeployedNFTModel>();

        string json = PlayerPrefs.GetString(DeployedNFTsKey);

        List<DeployedNFTModel> nfts = JsonConvert.DeserializeObject<List<DeployedNFTModel>>(json);

        return nfts;
    }

    public void PersistKnownNfts(List<DeployedNFTModel> knownNfts)
    {
        string json = JsonConvert.SerializeObject(knownNfts);
        PlayerPrefs.SetString(DeployedNFTsKey, json);
    }
}
