using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject messageUI;

    public void ExpandMessageUI()
    {
        Debug.Log("Expanding message UI...");
    }

    public void CloseMessageUI()
    {
        if (messageUI.activeSelf)
        {
            messageUI.SetActive(false);
            Debug.Log("Message UI closed.");
        }
    }
}