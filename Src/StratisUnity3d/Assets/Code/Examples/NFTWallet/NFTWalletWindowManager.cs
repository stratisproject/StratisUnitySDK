using System.Collections.Generic;
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

    private List<WindowBase> allWindows;
    
    void Awake()
    {
        Instance = this;

        this.allWindows = new List<WindowBase>() { LoginWindow, PopupWindow, WalletWindow, MyCollectionWindow, CreateNftWindow, SendWindow, MintWindow, BurnWindow };
    }

    void Start()
    {
        this.LoginWindow.Show();
    }

    public void HideAllWindows()
    {
        foreach (WindowBase window in this.allWindows)
        {
            window.Hide();
        }
    }
}
