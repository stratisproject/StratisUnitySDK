using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Stratis.SmartContracts;
using Unity3dApi;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyCollectionWindow : WindowBase
{
    public GameObject ContentGameObject;

    public CollectionItem CollectionCopyFromItem;

    public ScrollRect ScrollRect;

    public List<CollectionItem> SpawnedItems = new List<CollectionItem>();

    private float defaultScrollRectVerticalPosition;

    void Awake()
    {
        defaultScrollRectVerticalPosition = ScrollRect.verticalNormalizedPosition;
    }

    public override async UniTask ShowAsync(bool hideOtherWindows = true)
    {
        ScrollRect.verticalNormalizedPosition = this.defaultScrollRectVerticalPosition;

        // Disable prev spawned items.
        foreach (CollectionItem prevSpawn in SpawnedItems)
            prevSpawn.gameObject.SetActive(false);

        await base.ShowAsync(hideOtherWindows);

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

        // Load images
        foreach (CollectionItem collectionItem in this.SpawnedItems.Where(x => !x.ImageLoaded))
        {
            string uri = collectionItem.NFTUri;

            if (!uri.StartsWith("https://") || !(uri.EndsWith(".png") || uri.EndsWith(".jpg")))
                continue;
            
            Texture2D texture = await this.GetRemoteTextureAsync(uri);

            if (texture == null)
                continue;

            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            collectionItem.NFTImage.sprite = sprite;
            collectionItem.ImageLoaded = true;
        }
    }

    private async UniTask<Texture2D> GetRemoteTextureAsync(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOp = www.SendWebRequest();
            
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{www.error}, URL:{www.url}");
                return null;
            }
            
            return DownloadHandlerTexture.GetContent(www);
        }
    }
}
