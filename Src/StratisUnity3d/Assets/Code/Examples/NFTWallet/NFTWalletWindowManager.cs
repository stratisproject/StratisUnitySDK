using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTWalletWindowManager : MonoBehaviour
{
    public static NFTWalletWindowManager Instance;

    public LoginWindow LoginWindow;
    public PopupWindow PopupWindow;
    public HomeWindow HomeWindow;

    private List<WindowBase> allWindows;
    
    void Awake()
    {
        Instance = this;

        this.allWindows = new List<WindowBase>() { LoginWindow, PopupWindow, HomeWindow };
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
