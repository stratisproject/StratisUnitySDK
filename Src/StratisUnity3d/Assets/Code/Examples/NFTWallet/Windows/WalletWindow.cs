using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WalletWindow : WindowBase
{
    public Button CopyAddressButton, RefreshBalanceButton;

    public Text AddressText, BalanceText;

    void Awake()
    {
        CopyAddressButton.onClick.AddListener(delegate { GUIUtility.systemCopyBuffer = this.AddressText.text; });
        RefreshBalanceButton.onClick.AddListener(async delegate { await this.RefreshBalanceAsync();});
    }

    public override async UniTask ShowAsync(bool hideOtherWindows = true)
    {
        this.AddressText.text = NFTWallet.Instance.StratisUnityManager.GetAddress().ToString();

        await this.RefreshBalanceAsync();

        await base.ShowAsync(hideOtherWindows);
    }

    private async UniTask RefreshBalanceAsync()
    {
        this.BalanceText.text = (await NFTWallet.Instance.StratisUnityManager.GetBalanceAsync()).ToString();
    }
}
