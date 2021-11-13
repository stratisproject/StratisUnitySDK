using System;
using System.Net.Http;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using NBitcoin;
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

    public void RegisterDeployedNFT(string name, string symbol, bool ownerOnlyMinting, string contractAddress)
    {
        // TODO
    }
}
