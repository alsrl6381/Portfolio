using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SessionEntryPrefab : MonoBehaviour
{
    public TextMeshProUGUI sessionName;
    public TextMeshProUGUI playerCount;
    public Button joinButton;

    //�濡 
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
