using UnityEngine;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    public GameObject CreditUI;
    public void BBBstudio()
    {
        CreditUI.SetActive(true);
    }
    public void CloseBBB()
    {
        CreditUI.SetActive(false);
    }
}
