using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//방에 접속 했을 때 User가 방장인지, 준비상태인지를 표시하는 UI
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
