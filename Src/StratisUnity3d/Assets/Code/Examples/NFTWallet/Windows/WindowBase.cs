using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBase : MonoBehaviour
{
    public void Show(bool hideOtherWindows = true)
    {
        if (hideOtherWindows)
            NFTWalletWindowManager.Instance.HideAllWindows();

        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
