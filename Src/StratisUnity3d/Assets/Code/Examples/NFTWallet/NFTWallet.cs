using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
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

    public Network Network => network;

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
            this.StratisUnityManager = new StratisUnityManager(new Unity3dClient(ApiUrl), Network,
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

    public async UniTask AddKnownContractsIfMissingAsync()
    {
        try
        {
            OwnedNFTsModel ownedNfts = await this.StratisUnityManager.Client.GetOwnedNftsAsync(this.StratisUnityManager.GetAddress().ToString());

            var contracts = ownedNfts.OwnedIDsByContractAddress.Keys.ToList();

            var loaded = LoadKnownNfts().Select(x => x.ContractAddress);

            var contractsToAdd = contracts.Where(x => !loaded.Contains(x));

            foreach (string contractToAdd in contractsToAdd)
            {
                bool success = await RegisterKnownNFTAsync(contractToAdd);

                Debug.Log(string.Format("Contract {0} registered. Success: {1}", contractToAdd, success));
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async UniTask<bool> RegisterKnownNFTAsync(string contractAddress)
    {
        NFTWrapper wrapper = new NFTWrapper(NFTWallet.Instance.StratisUnityManager, contractAddress);

        string nftname, ownerAddr, symbol;

        try
        {
            nftname = await wrapper.NameAsync();
            ownerAddr = await wrapper.OwnerAsync();
            symbol = await wrapper.SymbolAsync();
        }
        catch (Exception e)
        {
            return false;
        }

        await NFTWallet.Instance.RegisterKnownNFTAsync(nftname, symbol, null, contractAddress, ownerAddr);

        return true;
    }

    public async UniTask RegisterKnownNFTAsync(string nftName, string symbol, bool? ownerOnlyMinting, string contractAddress, string ownerAddress)
    {
        List<DeployedNFTModel> knownNfts = LoadKnownNfts();

        await this.StratisUnityManager.Client.WatchNftContractAsync(contractAddress);

        if (knownNfts.Any(x => x.ContractAddress == contractAddress))
            return;

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
