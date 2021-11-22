using UnityEngine;
using UnityEngine.UI;

public class CollectionItem : MonoBehaviour
{
    public Text TitleText, DescriptionText;

    public Image NFTImage;

    [HideInInspector]
    public long NFTID;

    [HideInInspector]
    public string ContractAddr;

    [HideInInspector]
    public string NFTUri;

    [HideInInspector] 
    public bool ImageLoaded = false;
}
