using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class PopupWindow : WindowBase
{
    public Button Close;
    public Text TitleText, MessageText;

    public async void Awake()
    {
        this.Close.onClick.AddListener(async delegate { await this.HideAsync(); });
    }

    public async UniTask ShowPopupAsync(string message, string title = "")
    {
        this.MessageText.text = message;
        this.TitleText.text = title;

        await this.ShowAsync(false);
    }
}
