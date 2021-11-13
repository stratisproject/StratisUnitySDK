using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NBitcoin;
using UnityEngine;
using UnityEngine.UI;

public class WalletWindow : WindowBase
{
    public Button CopyAddressButton, RefreshBalanceButton, SendTxButton;

    public Text AddressText, BalanceText;

    public InputField DestinationAddressInputField, AmountInputField;

    void Awake()
    {
        CopyAddressButton.onClick.AddListener(delegate { GUIUtility.systemCopyBuffer = this.AddressText.text; });
        RefreshBalanceButton.onClick.AddListener(async delegate { await this.RefreshBalanceAsync(); });

        SendTxButton.onClick.AddListener(async delegate
        {
            string destAddress = this.DestinationAddressInputField.text;
            Money amount = new Money(Decimal.Parse(this.AmountInputField.text), MoneyUnit.BTC);

            this.DestinationAddressInputField.text = this.AmountInputField.text = string.Empty;

            if (destAddress == NFTWallet.Instance.StratisUnityManager.GetAddress().ToString())
            {
                await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync("Destination address can't be your address.", "ERROR");
                return;
            }

            long currentBalanceSat = await NFTWallet.Instance.StratisUnityManager.Client.GetAddressBalanceAsync(destAddress);

            Task<string> sendTxTask = NFTWallet.Instance.StratisUnityManager.SendTransactionAsync(destAddress, amount);
            
            await NFTWalletWindowManager.Instance.WaitTransactionWindow.DisplayUntilDestBalanceChanges(destAddress, currentBalanceSat, sendTxTask);
        });
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
