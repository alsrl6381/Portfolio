using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//방을 클릭 했을 때 참여하는 기능 
public class SessionEntryPrefab : MonoBehaviour
{
    public TextMeshProUGUI sessionName;
    public TextMeshProUGUI playerCount;
    public Button joinButton;

    //¹æ¿¡ 
    private void Awake()
    {
        joinButton.onClick.AddListener(JoinSession);
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }

    private void JoinSession()
    {
        FusionConnection.instance.ConnectToSession(sessionName.text);
    }
}
