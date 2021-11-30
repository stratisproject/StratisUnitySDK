using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NBitcoin;
using Unity3dApi;
using UnityEngine;
using UnityEngine.UI;

public class WalletWindow : WindowBase
{
    public Button CopyAddressButton, RefreshBalanceButton, SendTxButton, FaucetButton;

    public Text AddressText, BalanceText;

    public InputField DestinationAddressInputField, AmountInputField;
    
    void Awake()
    {
        CopyAddressButton.onClick.AddListener(delegate { GUIUtility.systemCopyBuffer = this.AddressText.text; });
        RefreshBalanceButton.onClick.AddListener(async delegate { await this.RefreshBalanceAsync(); });

        SendTxButton.onClick.AddListener(async delegate
        {
            try
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

                await this.RefreshBalanceAsync();
            }
            catch (Exception e)
            {
                await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync(e.ToString(), "ERROR");
            }
        });

        FaucetButton.onClick.AddListener(async delegate
        {
            string faucetReceivedKey = "faucetReceived";

            bool faucetReceived = false;

            if (PlayerPrefs.HasKey(faucetReceivedKey))
                faucetReceived = PlayerPrefs.GetInt(faucetReceivedKey) != 0;

            if (faucetReceived)
            {
                await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync("You've already received faucet funds.", "ERROR");
                return;
            }
            
            string faucetMnemonic = "matter solar quote boat resource peasant home resource sail damage tonight silent";
            Money faucetAmount = new Money(5, MoneyUnit.BTC);
            string destAddr = NFTWallet.Instance.StratisUnityManager.GetAddress().ToString();

            StratisUnityManager stratisUnityManager = new StratisUnityManager(new Unity3dClient(NFTWallet.Instance.ApiUrl), NFTWallet.Instance.Network,
                new Mnemonic(faucetMnemonic, Wordlist.English));
            
            Task<string> sendTxTask = stratisUnityManager.SendTransactionAsync(destAddr, faucetAmount);

            long currentBalanceSat = await NFTWallet.Instance.StratisUnityManager.Client.GetAddressBalanceAsync(destAddr);

            await NFTWalletWindowManager.Instance.WaitTransactionWindow.DisplayUntilDestBalanceChanges(destAddr, currentBalanceSat, sendTxTask);

            await this.RefreshBalanceAsync();

            PlayerPrefs.SetInt(faucetReceivedKey, 1);
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
