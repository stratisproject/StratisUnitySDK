using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Stratis.SmartContracts;
using Unity3dApi;
using UnityEngine.UI;

public class SendWindow : WindowBase
{
    public Dropdown NFTContractSelect_Dropdown;

    public InputField SendIdInputField, DestinationAddrInputField;

    public Text OwnedIdsText;

    public Button SendButton;

    private List<string> contractAddresses;

    private OwnedNFTsModel ownedNfts;
    private string selectedContract;

    void Awake()
    {
        NFTContractSelect_Dropdown.onValueChanged.AddListener(delegate (int optionNumber)
        {
            selectedContract = contractAddresses[optionNumber];
            DisplayAvailableIdsForSelectedContract();
        });

        SendButton.onClick.AddListener(async delegate
        {
            if (string.IsNullOrEmpty(selectedContract) || string.IsNullOrEmpty(SendIdInputField.text) || string.IsNullOrEmpty(DestinationAddrInputField.text))
            {
                await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync("No contract or ID or dest address selected", "ERROR");
                return;
            }

            UInt256 sendId = UInt256.Parse(SendIdInputField.text);
            string destAddr = DestinationAddrInputField.text;

            DestinationAddrInputField.text = SendIdInputField.text = string.Empty;


            NFTWrapper wrapper = new NFTWrapper(NFTWallet.Instance.StratisUnityManager, selectedContract);
            
            Task<string> sendTask = wrapper.TransferFromAsync(NFTWallet.Instance.StratisUnityManager.GetAddress().ToString(), destAddr, sendId);
            
            ReceiptResponse receipt = await NFTWalletWindowManager.Instance.WaitTransactionWindow.DisplayUntilSCReceiptReadyAsync(sendTask);
            
            bool success = receipt.Success;
            
            string resultString = string.Format("NFT send success: {0}", success);
            
            await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync(resultString, "NFT SEND");
            
            this.ownedNfts.OwnedIDsByContractAddress.First(x => x.Key == selectedContract).Value.Remove((long)sendId);
            DisplayAvailableIdsForSelectedContract();
        });
    }

    public override async UniTask ShowAsync(bool hideOtherWindows = true)
    {
        await base.ShowAsync(hideOtherWindows);

        string myAddress = NFTWallet.Instance.StratisUnityManager.GetAddress().ToString();
        ownedNfts = await NFTWallet.Instance.StratisUnityManager.Client.GetOwnedNftsAsync(myAddress);
        
        this.contractAddresses = ownedNfts.OwnedIDsByContractAddress.Keys.ToList();
        this.selectedContract = this.contractAddresses.FirstOrDefault();

        List<DeployedNFTModel> knownContracts = NFTWallet.Instance.LoadKnownNfts();
        List<string> options = new List<string>();

        foreach (string contractAddress in contractAddresses)
        {
            string option = contractAddress;

            DeployedNFTModel knownContract = knownContracts.FirstOrDefault(x => x.ContractAddress == contractAddress);

            if (knownContract != null)
                option += string.Format("   : {0} ({1})", knownContract.NftName, knownContract.Symbol);

            options.Add(option);
        }

        NFTContractSelect_Dropdown.ClearOptions();
        NFTContractSelect_Dropdown.AddOptions(options);

        DisplayAvailableIdsForSelectedContract();
    }
    
    private void DisplayAvailableIdsForSelectedContract()
    {
        KeyValuePair<string, ICollection<long>> contractToIds = this.ownedNfts.OwnedIDsByContractAddress.FirstOrDefault(x => x.Key == selectedContract);

        string idsString;
        if (contractToIds.Value == null || !contractToIds.Value.Any())
            idsString = "You don't own any NFTs of that type.";
        else idsString = string.Join(",", contractToIds.Value);

        OwnedIdsText.text = idsString;
    }
}
