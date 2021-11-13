using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity3dApi;
using UnityEngine;

public class BurnWindow : WindowBase
{
    public override async UniTask ShowAsync(bool hideOtherWindows = true)
    {
        await base.ShowAsync(hideOtherWindows);

        string myAddress = NFTWallet.Instance.StratisUnityManager.GetAddress().ToString();
        OwnedNFTsModel ownedNfts = await NFTWallet.Instance.StratisUnityManager.Client.GetOwnedNftsAsync(myAddress);

        string allOwnedNfts = "Owned NFTs" + Environment.NewLine;

        foreach (KeyValuePair<string, ICollection<long>> contrAddrToIds in ownedNfts.OwnedIDsByContractAddress)
        {
            allOwnedNfts += contrAddrToIds.Key + " " + string.Join(",", contrAddrToIds.Value) + Environment.NewLine;
        }

        Debug.Log(allOwnedNfts);
    }
}
