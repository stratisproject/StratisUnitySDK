using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button Wallet_Button, MyCollection_Button, CreateNFT_Button, Send_Button, Mint_Button, Burn_Button, LogOutButton;

    async void Awake()
    {
        Wallet_Button.onClick.AddListener(async delegate { await NFTWalletWindowManager.Instance.WalletWindow.ShowAsync(); });
        MyCollection_Button.onClick.AddListener(async delegate { await NFTWalletWindowManager.Instance.MyCollectionWindow.ShowAsync(); });
        CreateNFT_Button.onClick.AddListener(async delegate { await NFTWalletWindowManager.Instance.CreateNftWindow.ShowAsync(); });
        Send_Button.onClick.AddListener(async delegate { await NFTWalletWindowManager.Instance.SendWindow.ShowAsync(); });
        Mint_Button.onClick.AddListener(async delegate { await NFTWalletWindowManager.Instance.MintWindow.ShowAsync(); });
        Burn_Button.onClick.AddListener(async delegate { await NFTWalletWindowManager.Instance.BurnWindow.ShowAsync(); });
        LogOutButton.onClick.AddListener(async delegate { await NFTWalletWindowManager.Instance.LoginWindow.ShowAsync(); });
    }
}
