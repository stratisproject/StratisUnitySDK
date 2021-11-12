using UnityEngine.UI;

public class PopupWindow : WindowBase
{
    public Button Close;
    public Text TitleText, MessageText;

    public void Start()
    {
        this.Close.onClick.AddListener(delegate { this.Hide(); });
    }

    public void ShowPopup(string message, string title = "")
    {
        this.MessageText.text = message;
        this.TitleText.text = title;

        this.Show(false);
    }
}
