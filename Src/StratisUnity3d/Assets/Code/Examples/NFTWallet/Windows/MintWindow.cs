using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity3dApi;
using UnityEngine.UI;

public class MintWindow : WindowBase
{
    public Dropdown NFTContractSelect_Dropdown;

    public Button MintButton;

    public InputField MintToAddrInputField, UriInputField, MetadataInputField;

    private List<DeployedNFTModel> nftsForDeployment;

    private DeployedNFTModel selectedNft;

    void Awake()
    {
        NFTContractSelect_Dropdown.onValueChanged.AddListener(delegate(int optionNumber)
        {
            selectedNft = nftsForDeployment[optionNumber];
        });

        MintButton.onClick.AddListener(async delegate
        {
            string mintToAddr = MintToAddrInputField.text;

            string metadata = MetadataInputField.text;
            MetadataInputField.text = string.Empty;

            string uri = UriInputField.text;
            UriInputField.text = "";

            NFTWrapper wrapper = new NFTWrapper(NFTWallet.Instance.StratisUnityManager, selectedNft.ContractAddress);

            Task<string> mintNftTask;

            if (!string.IsNullOrEmpty(metadata))
            {
                byte[] metadataBytes = Encoding.ASCII.GetBytes(metadata);
                mintNftTask = wrapper.SafeMintAsync(mintToAddr, uri, metadataBytes);
            }
            else
            {
                mintNftTask = wrapper.MintAsync(mintToAddr, uri);
            }

            ReceiptResponse receipt = await NFTWalletWindowManager.Instance.WaitTransactionWindow.DisplayUntilSCReceiptReadyAsync(mintNftTask);

            bool success = receipt.Success;
            string nftId = receipt.ReturnValue;

            string resultString = string.Format("NFT mint success: {0}.{2}Minted NFT ID: {1}{2}If you've minted it to your address then this NFT will be shown in MY COLLECTION window.", success, nftId, Environment.NewLine);

            await NFTWalletWindowManager.Instance.PopupWindow.ShowPopupAsync(resultString, "NFT MINT");
        });
    }

    public override UniTask ShowAsync(bool hideOtherWindows = true)
    {
        string myAddress = NFTWallet.Instance.StratisUnityManager.GetAddress().ToString();
        nftsForDeployment = NFTWallet.Instance.LoadKnownNfts().Where(x => !x.OwnerOnlyMinting || x.OwnerAddress == myAddress).ToList();
        selectedNft = nftsForDeployment.FirstOrDefault();

        NFTContractSelect_Dropdown.ClearOptions();
        List<string> options = nftsForDeployment.Select(x => string.Format("{0} ({1})  -  {2}", x.NftName, x.Symbol, x.ContractAddress)).ToList();
        NFTContractSelect_Dropdown.AddOptions(options);

        MintToAddrInputField.text = myAddress;

        return base.ShowAsync(hideOtherWindows);
    }
}
