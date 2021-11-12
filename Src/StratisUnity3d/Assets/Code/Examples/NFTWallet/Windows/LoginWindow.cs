using System;
using NBitcoin;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : WindowBase
{
    public InputField MnemonicInputField;
    public Button GenerateNewMnemonicButton, LogInButton;
    public Text NewMnemonicWarningText;

    public Text MnemonicInputFieldPlaceholderText;

    private const string MnemonicKey = "";

    void Awake()
    {
        this.GenerateNewMnemonicButton.onClick.AddListener(delegate
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            this.MnemonicInputField.text = mnemonic.ToString();
        });

        this.LogInButton.onClick.AddListener(delegate
        {
            bool presavedMnemonicExists = PlayerPrefs.HasKey(MnemonicKey);
            bool mnemonicEntered = !string.IsNullOrEmpty(MnemonicInputField.text);


            if (!presavedMnemonicExists && !mnemonicEntered)
            {
                // No mnemonic entered
                NFTWalletWindowManager.Instance.PopupWindow.ShowPopup("You need to enter or generate mnemonic!", "ERROR");
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
                NFTWalletWindowManager.Instance.PopupWindow.ShowPopup(e.Message, "INCORRECT MNEMONIC");
                return;
            }

            PlayerPrefs.SetString(MnemonicKey, mnemonic);

            NFTWallet.Instance.Initialize(mnemonic);
            NFTWalletWindowManager.Instance.WalletWindow.Show();
        });
    }

    void Start()
    {
        bool mnemonicExists = PlayerPrefs.HasKey(MnemonicKey);

        if (mnemonicExists)
        {
            // Mnemonic was saved before
            MnemonicInputFieldPlaceholderText.text = "Previously saved mnemonic was loaded, click log in.";
        }

        NewMnemonicWarningText.gameObject.SetActive(!mnemonicExists);
    }
}
