using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity3dApi;
using UnityEngine;
using UnityEngine.UI;

public class WaitTransactionWindow : WindowBase
{
    public Text TxHashText;
    public Button CopyTxHashButton;

    void Awake()
    {
        CopyTxHashButton.onClick.AddListener(delegate { GUIUtility.systemCopyBuffer = this.TxHashText.text; });
    }

    /// <summary>Waits for smart contract call transaction to be mined. Do not call it with normal transaction.</summary>
    public async UniTask<ReceiptResponse> DisplayUntilSCReceiptReadyAsync(Task<string> callSmartContractTask)
    {
        await this.ShowAsync(false);

        string txHash = await callSmartContractTask;
        this.TxHashText.text = txHash;

        ReceiptResponse receipt = await NFTWallet.Instance.StratisUnityManager.WaitTillReceiptAvailable(txHash);

        await this.HideAsync();

        return receipt;
    }

    /// <summary>Waits till target address' balance is changed.</summary>
    public async UniTask DisplayUntilDestBalanceChanges(string address, long currentBalanceSat, Task<string> sendTxTask)
    {
        await this.ShowAsync(false);

        string txHash = await sendTxTask;
        this.TxHashText.text = txHash;

        while (true)
        {
            long balanceSat = await NFTWallet.Instance.StratisUnityManager.Client.GetAddressBalanceAsync(address);

            if (balanceSat != currentBalanceSat)
                break;

            await Task.Delay(500);
        }

        this.TxHashText.text = string.Empty;

        await this.HideAsync();
    }
}
