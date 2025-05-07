using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionUserUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public GameObject roomMasterIcon;
    public GameObject isReady;

    public void Ready(bool state)
    {
        if(state == true)
        {
            isReady.SetActive(true);
        }
        else
        {
            isReady.SetActive(false);
        }
    }
}
