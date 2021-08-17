using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Bitcoin.Networks;
using Unity3dApi;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    public Button TestAPI_Button, GenerateMnemonic_Button, InitializeApi_Button, CopyAddress_Button, RefreshBalance_Button, SendTx_Button, SendOpReturnTx_Button;

    public InputField ApiUrl_InputField, Mnemonic_InputField, DestinationAddress_InputField, Amount_InputField, OpReturnData_InputField;

    public Text Address_Text, Balance_Text;

    public GameObject PopupPanel;

    public Text PopupPanel_Text;

    public Button PopupPanelOk_Button;

    private StratisUnityManager stratisUnityManager;

    void Start()
    {
        this.ApiUrl_InputField.text = "http://localhost:44336/";
        this.Mnemonic_InputField.text = "leopard fire legal door attract stove similar response photo prize seminar frown";

        PopupPanel.SetActive(false);

        TestAPI_Button.onClick.AddListener(() => this.StartCoroutine(TestApi_ButtonCall()));
        GenerateMnemonic_Button.onClick.AddListener(GenerateMnemonic_ButtonCall);
        InitializeApi_Button.onClick.AddListener(InitializeApi_ButtonCall);
        CopyAddress_Button.onClick.AddListener(CopyAddress_ButtonCall);
        RefreshBalance_Button.onClick.AddListener(RefreshBalance_ButtonCall);
        SendTx_Button.onClick.AddListener(() => this.StartCoroutine(SendTx_ButtonCall()));
        SendOpReturnTx_Button.onClick.AddListener(() => this.StartCoroutine(SendOpReturnTx_ButtonCall()));

        PopupPanelOk_Button.onClick.AddListener(() => PopupPanel.SetActive(false));
    }
    
    private IEnumerator TestApi_ButtonCall()
    {
        Unity3dClient client = new Unity3dClient(ApiUrl_InputField.text);

        string resultString = "";

        Task task = Task.Run(async () =>
        {
            try
            {
                await client.TipAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                resultString = "Api error. Check that API URL is correct and can be reached. Exception: " + e.ToString();
                return;
            }

            resultString = "API test successful.";
        });
        
        while (!task.IsCompleted) 
            yield return null;
        
        this.DisplayPopup(resultString);
    }

    private void GenerateMnemonic_ButtonCall()
    {
        Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
        this.Mnemonic_InputField.text = mnemonic.ToString();
    }

    private void InitializeApi_ButtonCall()
    {
        try
        {
            this.stratisUnityManager = new StratisUnityManager(new Unity3dClient(ApiUrl_InputField.text), new StraxMain(), new Mnemonic(this.Mnemonic_InputField.text, Wordlist.English));

            this.DisplayPopup("StratisUnityManager initialized.");

            this.Address_Text.text = this.stratisUnityManager.GetAddress().ToString();
            this.StartCoroutine(RefreshBalance());
        }
        catch (Exception e)
        {
            this.DisplayPopup(e.ToString());
        }
    }

    private IEnumerator RefreshBalance()
    {
        decimal balance = -1;

        Task task = Task.Run(async () =>
        {
            try
            {
                balance = await this.stratisUnityManager.GetBalanceAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return;
            }
        });

        while (!task.IsCompleted)
            yield return null;

        this.Balance_Text.text = balance.ToString();
    }

    private void CopyAddress_ButtonCall()
    {
        GUIUtility.systemCopyBuffer = this.Address_Text.text;
    }

    private void RefreshBalance_ButtonCall()
    {
        this.StartCoroutine(RefreshBalance());
    }

    private IEnumerator SendTx_ButtonCall()
    {
        string destAddress = this.DestinationAddress_InputField.text;
        Money amount = new Money(Decimal.Parse(this.Amount_InputField.text), MoneyUnit.BTC);

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
            this.DisplayPopup(string.Format("Transaction {0} to {1} with amount: {2} was sent.", txHash, destAddress, amount));
        }

        this.DestinationAddress_InputField.text = "";
        this.Amount_InputField.text = "";
    }

    private IEnumerator SendOpReturnTx_ButtonCall()
    {
        string opReturnData = this.OpReturnData_InputField.text;
        string txHash = null;
        string error = null;

        Task task = Task.Run(async () =>
        {
            try
            {
                txHash = await this.stratisUnityManager.SendOpReturnTransactionAsync(opReturnData);
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
            this.DisplayPopup(string.Format("Transaction {0} with op_return data {1} was sent.", txHash, opReturnData));
        }

        this.OpReturnData_InputField.text = "";
    }

    private void DisplayPopup(string text)
    {
        SynchronizationContext.Current.Post(state =>
        {
            this.PopupPanel.SetActive(true);
            this.PopupPanel_Text.text = text;
        }, null);
    }
}
