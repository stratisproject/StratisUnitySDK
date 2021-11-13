using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button Wallet_Button, MyCollection_Button, CreateNFT_Button, Send_Button, Mint_Button, Burn_Button;

    void Awake()
    {
        Wallet_Button.onClick.AddListener(delegate { NFTWalletWindowManager.Instance.WalletWindow.Show(); });
        MyCollection_Button.onClick.AddListener(delegate { NFTWalletWindowManager.Instance.MyCollectionWindow.Show(); });
        CreateNFT_Button.onClick.AddListener(delegate { NFTWalletWindowManager.Instance.CreateNftWindow.Show(); });
        Send_Button.onClick.AddListener(delegate { NFTWalletWindowManager.Instance.SendWindow.Show(); });
        Mint_Button.onClick.AddListener(delegate { NFTWalletWindowManager.Instance.MintWindow.Show(); });
        Burn_Button.onClick.AddListener(delegate { NFTWalletWindowManager.Instance.BurnWindow.Show(); });
    }
}
