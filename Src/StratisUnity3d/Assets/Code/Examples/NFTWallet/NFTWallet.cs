using UnityEngine;

public class NFTWallet : MonoBehaviour
{
    public static NFTWallet Instance;
    
    void Awake()
    {
        Instance = this;
    }

    public void Initialize(string mnemonic)
    {
        // TODO
    }
}
