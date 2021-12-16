using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NFTWalletWindowManager : MonoBehaviour
{
    public static NFTWalletWindowManager Instance;

    public LoginWindow LoginWindow;
    public PopupWindow PopupWindow;

    public WalletWindow WalletWindow;
    public MyCollectionWindow MyCollectionWindow;
    public CreateNFTWindow CreateNftWindow;
    public SendWindow SendWindow;
    public MintWindow MintWindow;
    public BurnWindow BurnWindow;
    public WaitTransactionWindow WaitTransactionWindow;

    public bool IsMobile;

    private List<WindowBase> allWindows;
    
    void Awake()
    {
        Instance = this;

        this.allWindows = new List<WindowBase>() { LoginWindow, PopupWindow, WalletWindow, MyCollectionWindow, CreateNftWindow, SendWindow, MintWindow, BurnWindow, WaitTransactionWindow };
    }

    async void Start()
    {
        await this.LoginWindow.ShowAsync();
    }

    public async UniTask HideAllWindowsAsync()
    {
        foreach (WindowBase window in this.allWindows)
        {
            await window.HideAsync();
        }
    }
}
