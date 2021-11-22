using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Stratis.SmartContracts;
using Unity3dApi;
using UnityEngine;
using UnityEngine.UI;

public class MyCollectionWindow : WindowBase
{
    public GameObject ContentGameObject;

    public CollectionItem CollectionCopyFromItem;

    public List<CollectionItem> SpawnedItems = new List<CollectionItem>();

    public override async UniTask ShowAsync(bool hideOtherWindows = true)
    {
        // Disable prev spawned items.
        foreach (CollectionItem prevSpawn in SpawnedItems)
            prevSpawn.gameObject.SetActive(false);

        List<DeployedNFTModel> knownNfts = NFTWallet.Instance.LoadKnownNfts();
        OwnedNFTsModel myNfts = await NFTWallet.Instance.StratisUnityManager.Client.GetOwnedNftsAsync(NFTWallet.Instance.StratisUnityManager.GetAddress().ToString());
        
        foreach (KeyValuePair<string, ICollection<long>> contrAddrToOwnedIds in myNfts.OwnedIDsByContractAddress)
        {
            string contractAddr = contrAddrToOwnedIds.Key;
            List<long> ownedIds = contrAddrToOwnedIds.Value.ToList();

            string nftName = knownNfts.First(x => x.ContractAddress == contractAddr).NftName;

            NFTWrapper wrapper = new NFTWrapper(NFTWallet.Instance.StratisUnityManager, contractAddr);

            List<string> uris = new List<string>(ownedIds.Count);

            foreach (long ownedId in ownedIds)
            {
                string uri = await wrapper.TokenURIAsync((UInt256) ownedId);

                uris.Add(uri);
            }

            // Create item for each owned ID or enable already spawned item
            for (int i = 0; i < ownedIds.Count; i++)
            {
                long currentId = ownedIds[i];

                var alreadySpawnedItem = SpawnedItems.SingleOrDefault(x => x.ContractAddr == contractAddr && x.NFTID == currentId);

                if (alreadySpawnedItem != null)
                {
                    alreadySpawnedItem.gameObject.SetActive(true);
                    continue;
                }

                CollectionItem cItem = GameObject.Instantiate(CollectionCopyFromItem, ContentGameObject.transform);
                cItem.gameObject.SetActive(true);
                SpawnedItems.Add(cItem);

                cItem.NFTID = currentId;
                cItem.ContractAddr = contractAddr;
                cItem.NFTUri = uris[i];

                cItem.TitleText.text = nftName;
                cItem.DescriptionText.text = string.Format("ID: {0}", currentId);
            }
        }

        RectTransform contentTransform = ContentGameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayoutGroup = ContentGameObject.GetComponent<GridLayoutGroup>();
        float cellSize = gridLayoutGroup.cellSize.y;
        float spacing = gridLayoutGroup.spacing.y;
        int rows = (int)Math.Ceiling(((decimal) SpawnedItems.Count / 3));
        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, (cellSize + spacing) * rows);

        await base.ShowAsync(hideOtherWindows);
    }
}
