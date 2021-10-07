using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json;
using RedRunner;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using UnityEngine.UI;
using Network = NBitcoin.Network;

public class SDKIntegrationManager : MonoBehaviour
{
    public InputField Mnemonic_InputField, DestAddrInputField, AmountInputField;

    public Button Button_SetMnmemonic, Button_NewMnmemonic, RefreshButton, SendStraxButton, SendRRTButton, Button_CopyAddr, Button_SendNFT, Button_MintNFT;

    public Text AddressText, BalanceStraxText, BalanceRRTText, NFTBalanceText;

    public GameObject PopupPanel;

    public Text PopupPanel_Text;

    public Button PopupPanelOk_Button;

    public string ApiUrl = "http://localhost:44336/";

    public string Mnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    public string RedRunnerTokenContractAddress = "t778saxw6Xdgs77Z5ePpaFPCZ9bk4oNrPT";

    public string RedRunnerNFTContractAddress = "tPsCN2Wmu8ER1Bq1vufb6gVKrHQrUC5gu5";

    public static SDKIntegrationManager Instance { get; private set; }

    private Network network = new CirrusTest();

    private StratisUnityManager stratisUnityManager;

    private string address;

    private Unity3dClient client;

    private StandartTokenWrapper tokenRRT;

    private NFTWrapper nft;

    private const int NFTContractLogsStartHeight = 2641320;

    private decimal straxBalance = -1;
    private ulong rrtBalance = 0;
    private ulong NFTBalance = 0;

    async void Awake()
    {
        Instance = this;
        client = new Unity3dClient(ApiUrl);
        this.InitializeUI();

        InitMnemonic(Mnemonic);


        // TODO
        var res = await stratisUnityManager.Client.ReceiptAsync("34229aa18686e1cc2fb73c4c6cef1cbad8288e9a46b65d7195d89d5c4a9c2eb9");
        Debug.Log("Success: " + res.Success + " " + res.Error);
    }

    private void InitMnemonic(string mnemonic)
    {
        Mnemonic m = new Mnemonic(mnemonic, Wordlist.English);
        stratisUnityManager = new StratisUnityManager(client, network, m);

        this.tokenRRT = new StandartTokenWrapper(stratisUnityManager, this.RedRunnerTokenContractAddress);
        this.nft = new NFTWrapper(stratisUnityManager, RedRunnerNFTContractAddress);

        address = stratisUnityManager.GetAddress().ToString();
        Debug.Log("Your address: " + address);
        
        this.Mnemonic_InputField.text = mnemonic;
        this.AddressText.text = address;

        this.StartCoroutine(RefreshBalance());
    }

    private IEnumerator RefreshBalance()
    {
        Task task = Task.Run(async () =>
        {
            try
            {
                this.straxBalance = await stratisUnityManager.GetBalanceAsync();
                this.rrtBalance = await tokenRRT.GetBalanceAsync(address);
                this.NFTBalance = await this.nft.BalanceOfAsync(this.address);

                Debug.Log("STRAXBalance: " + straxBalance + " | TokenBalance: " + rrtBalance + " | NFTBalance: " + NFTBalance);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });

        while (!task.IsCompleted)
            yield return null;


        SynchronizationContext.Current.Post(state =>
        {
            this.BalanceRRTText.text = "RRT: " + rrtBalance;
            this.BalanceStraxText.text = "STRAX: " + Math.Round(straxBalance, 2);
            this.NFTBalanceText.text = "NFT: " + NFTBalance;
        }, null);
    }
    
    public void PlayerDied(int distance, bool isHighestScore)
    {
        Debug.Log("PLAYER DEATH. coins collected = " + GameManager.Singleton.m_Coin.Value + "    distanceRan = " + distance);

        Task.Run(async () =>
        {
            var coinsCollected = GameManager.Singleton.m_Coin.Value;

            if (isHighestScore)
                Debug.Log("New high distance" + distance);


            PlayerNotificationResult result =
                await client.PlayerAchievementsNotificationAsync(address, "NO_NAME", coinsCollected, distance);

            Debug.Log("SEND TOKENS ON DEATH: " + result.SendTokens);
        });
    }

    #region UI
    private void InitializeUI()
    {
        this.PopupPanel.SetActive(false);
        this.Button_NewMnmemonic.onClick.AddListener(NewMnemonic_bnCall);
        this.Button_SetMnmemonic.onClick.AddListener(SetMnemonic_bnCall);

        Button_CopyAddr.onClick.AddListener(CopyAddress_ButtonCall);
        RefreshButton.onClick.AddListener(() => this.StartCoroutine(RefreshBalance()));

        SendStraxButton.onClick.AddListener(() => this.StartCoroutine(SendStrax_ButtonCall()));
        SendRRTButton.onClick.AddListener(() => this.StartCoroutine(SendRRT_ButtonCall()));
        Button_MintNFT.onClick.AddListener(() => this.StartCoroutine(MintNFT_ButtonCall()));
        Button_SendNFT.onClick.AddListener(() => this.StartCoroutine(SendNFT_ButtonCall()));

        PopupPanelOk_Button.onClick.AddListener(() => this.PopupPanel.SetActive(false));
    }

    private void CopyAddress_ButtonCall()
    {
        GUIUtility.systemCopyBuffer = this.address;
    }

    private void SetMnemonic_bnCall()
    {
        InitMnemonic(this.Mnemonic_InputField.text);
    }

    private void NewMnemonic_bnCall()
    {
        Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
        this.Mnemonic_InputField.text = mnemonic.ToString();

        SetMnemonic_bnCall();
    }

    private void DisplayPopup(string text)
    {
        SynchronizationContext.Current.Post(state =>
        {
            this.PopupPanel.SetActive(true);
            this.PopupPanel_Text.text = text;
        }, null);
    }

    private IEnumerator SendStrax_ButtonCall()
    {
        string destAddress = this.DestAddrInputField.text;
        Money amount = new Money(Decimal.Parse(this.AmountInputField.text), MoneyUnit.BTC);

        if (straxBalance < amount.ToUnit(MoneyUnit.BTC))
        {
            this.DisplayPopup("Error sending tx: not enough STRAX");
        }
        else
        {
            string txHash = null;
            string error = null;

            Task task = Task.Run(async () =>
            {
                try
                {
                    txHash = await this.stratisUnityManager.SendTransactionAsync(destAddress, amount);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    error = e.ToString();
                    return;
                }
            });

            while (!task.IsCompleted)
                yield return null;

            if (error != null)
            {
                this.DisplayPopup("Error sending tx: " + error);
            }
            else
            {
                this.DisplayPopup(string.Format("Transaction {0} to {1} with amount: {2} STRAX was sent.", txHash, destAddress, amount));
            }

            this.DestAddrInputField.text = "";
            this.AmountInputField.text = "";
        }
    }

    private IEnumerator SendRRT_ButtonCall()
    {
        string destAddress = this.DestAddrInputField.text;
        ulong amount = ulong.Parse(this.AmountInputField.text);

        if (rrtBalance < amount)
        {
            this.DisplayPopup("Error sending tx: not enough RRT");
        }
        else
        {
            string txHash = null;
            string error = null;

            Task task = Task.Run(async () =>
            {
                try
                {
                    txHash = await this.tokenRRT.TransferToAsync(destAddress, amount);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    error = e.ToString();
                    return;
                }
            });

            while (!task.IsCompleted)
                yield return null;

            if (error != null)
            {
                this.DisplayPopup("Error sending tx: " + error);
            }
            else
            {
                this.DisplayPopup(string.Format("Transaction {0} to {1} with amount: {2} RRT was sent.", txHash, destAddress, amount));
            }

            this.DestAddrInputField.text = "";
            this.AmountInputField.text = "";
        }
    }

    private IEnumerator MintNFT_ButtonCall()
    {
        if (straxBalance <= 0)
        {
            this.DisplayPopup("You need to have some STRAX to mint NFT.");
        }
        else
        {
            string txHash = null;
            string error = null;

            Task task = Task.Run(async () =>
            {
                try
                {
                    string txId = await this.nft.MintAsync(this.address);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    error = e.ToString();
                    return;
                }
            });

            while (!task.IsCompleted)
                yield return null;

            if (error != null)
            {
                this.DisplayPopup("Error minting NFT: " + error);
            }
            else
            {
                this.DisplayPopup(string.Format("Transaction {0} to mint NFT was sent.", txHash));
            }
        }
    }

    private IEnumerator SendNFT_ButtonCall()
    {
        ulong amount = ulong.Parse(this.AmountInputField.text);
        string destAddress = this.DestAddrInputField.text;

        if (NFTBalance <= 0)
        {
            this.DisplayPopup("You dont have any NFTs!");
        }
        else if (amount != 1)
        {
            this.DisplayPopup("Only 1 NFT can be sent at a time.");
        }
        else
        {
            string txHash = null;
            string error = null;

            this.DisplayPopup("Sending NFT transaction. Wait...");

            Task task = Task.Run(async () =>
            {
                try
                {
                    List<ReceiptResponse> receipts = (await client.ReceiptSearchAsync(RedRunnerNFTContractAddress, "TransferLog", null, NFTContractLogsStartHeight, null).ConfigureAwait(false)).ToList();

                    List<TransferInfo> transferLogs = new List<TransferInfo>(receipts.Count);

                    foreach (ReceiptResponse receiptRes in receipts)
                    {
                        var log = receiptRes.Logs.First().Log.ToString();
                        TransferInfo infoObj = JsonConvert.DeserializeObject<TransferInfo>(log);
                        transferLogs.Add(infoObj);
                    }

                    transferLogs.Reverse();
                    
                    long selectedId = -1;

                    for (int i = 0; i < transferLogs.Count; i++)
                    {
                        if (transferLogs[i].To == address)
                        {
                            // Check if it was used already.
                            long id = transferLogs[i].Value;

                            var logsAfter = transferLogs.GetRange(0, i);

                            bool usedAlready = logsAfter.Any(x => x.Value == id && x.From == address);

                            if (usedAlready)
                                continue;

                            selectedId = transferLogs[i].Value;
                            break;
                        }
                    }

                    if (selectedId == -1)
                        throw new Exception("No NFT ID found");
                    
                    Debug.Log(string.Format("Sending NFT from {0} to {1}, NFT ID: {2}", address, destAddress, selectedId));

                    await this.nft.TransferFromAsync(address, destAddress, (ulong)selectedId);

                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    error = e.ToString();
                    return;
                }
            });

            while (!task.IsCompleted)
                yield return null;

            if (error != null)
            {
                this.DisplayPopup("Error sending NFT: " + error);
            }
            else
            {
                this.DisplayPopup(string.Format("NFT transaction {0} to {1} was sent.", txHash, destAddress));
            }
        }
    }
    #endregion
}
