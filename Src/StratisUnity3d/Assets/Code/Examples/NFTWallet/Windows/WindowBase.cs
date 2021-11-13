using Cysharp.Threading.Tasks;
using UnityEngine;

public class WindowBase : MonoBehaviour
{
    public virtual async UniTask ShowAsync(bool hideOtherWindows = true)
    {
        if (hideOtherWindows)
            await NFTWalletWindowManager.Instance.HideAllWindowsAsync();

        this.gameObject.SetActive(true);
    }

    public virtual async UniTask HideAsync()
    {
        this.gameObject.SetActive(false);
    }
}
