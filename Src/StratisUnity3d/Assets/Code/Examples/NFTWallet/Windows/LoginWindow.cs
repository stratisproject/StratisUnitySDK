using System;
using Cysharp.Threading.Tasks;
using NBitcoin;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : WindowBase
{
    public InputField MnemonicInputField;
    public Button GenerateNewMnemonicButton, LogInButton, RemovePlayerPrefsButton;
    public Text NewMnemonicWarningText;

    public Text MnemonicInputFieldPlaceholderText;

    private const string MnemonicKey = "MnemonicST";

    async void Awake()
    {
        this.GenerateNewMnemonicButton.onClick.AddListener(delegate
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            this.MnemonicInputField.text = mnemonic.ToString();
        });

        this.LogInButton.onClick.AddListener(async delegate
        {
            bool presavedMnemonicExists = PlayerPrefs.HasKey(MnemonicKey);
            bool mnemonicEntered = !string.IsNullOrEmpty(MnemonicInputField.text);


            if (!presavedMnemonicExists && !mnemonicEntered)
            {
                // No mnemonic entered
                await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync("You need to enter or generate mnemonic!", "ERROR");
                return;
            }

            string mnemonic = mnemonicEntered ? MnemonicInputField.text : PlayerPrefs.GetString(MnemonicKey);
            
            // Validate mnemonic
            try
            {
                new Mnemonic(mnemonic, Wordlist.English);
            }
            catch (Exception e)
            {
                await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync(e.Message, "INCORRECT MNEMONIC");
                return;
            }

            MnemonicInputField.text = string.Empty;
            PlayerPrefs.SetString(MnemonicKey, mnemonic);

            bool success = await NFTWallet.Instance.InitializeAsync(mnemonic);

            if (success)
                await NFTWalletWindowManager.Instance.WalletWindow.ShowAsync();
            else
                await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync("Can't initialize NFT wallet. Probably can't reach API server.", "INITIALIZATION ERROR");

            await NFTWallet.Instance.AddKnownContractsIfMissingAsync();
        });

        RemovePlayerPrefsButton.onClick.AddListener(delegate
        {
            PlayerPrefs.DeleteAll();
        });
    }

    public override UniTask ShowAsync(bool hideOtherWindows = true)
    {
        bool mnemonicExists = PlayerPrefs.HasKey(MnemonicKey);

        if (mnemonicExists)
        {
            // Mnemonic was saved before
            MnemonicInputFieldPlaceholderText.text = "Previously saved mnemonic was loaded, click log in.";
        }

        NewMnemonicWarningText.gameObject.SetActive(!mnemonicExists);

        return base.ShowAsync(hideOtherWindows);
    }
}
