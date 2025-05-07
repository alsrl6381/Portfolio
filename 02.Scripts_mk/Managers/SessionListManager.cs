using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class SessionListManager : MonoBehaviour
{
    //sessionList���� UI �� sessionInfo�� ���� ������ ���� List   
    private List<SessionInfo> sessions = new List<SessionInfo>();
    [SerializeField] private GameObject sessionListCanvas;
    [SerializeField] private Transform sessionListContent;
    [SerializeField] private TMP_InputField titleText;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Button createButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject sessionEntryPrefab;
    [SerializeField] private Button confirmButton;

    //�ʱ�ȭ UI���� ��������
    public void Start()
    {
        /*
        GameObject tempCanvas = GameObject.Find("Session List Canvas");
        sessionListCanvas = tempCanvas.transform.Find("Room_List").gameObject;
        sessionListContent = sessionListCanvas.transform.Find("SessionLIst/Viewport/Session List Content");
        titlePanel = sessionListCanvas.transform.Find("CreateOption").gameObject;
        titleText = sessionListCanvas.transform.Find("CreateOption/CreatePanel/Title").GetComponent<TMP_InputField>();
        createButton = sessionListCanvas.transform.Find("ButtonPanel/Create Button").GetComponent<Button>();
        cancelButton = sessionListCanvas.transform.Find("CreateOption/CreatePanel/Cancel").GetComponent<Button>();
        */

        createButton.onClick.AddListener(() => { OpenCreateSession(); });
        cancelButton.onClick.AddListener(() => { CloseCreateSession(); });
        confirmButton.onClick.AddListener(() => { CreateRoomButton(); });
    }
    public void CreateRoomButton()
    {
        FusionConnection.instance.CreatSession();
    }

    // titlePanelŰ��
    public void OpenCreateSession()
    {
        titlePanel.SetActive(true);
    }

    // titlePanelŰ��
    public void CloseCreateSession()
    {
        titleText.text = "";
        titlePanel.SetActive(false);
    }

    // ���� ��ȯ�ϱ� �� ���� �� �� ������ ������
    public string ReturnTitle()
    {
        if(!titleText.text.IsNullOrEmpty())
        {
            return titleText.text;
        }
        else
        {
            return "";
        }
    }

    //�κ� ������ ��
    public void EnterSession()
    {
        StartCoroutine(SessionListUIDelay());
    }

    //�κ� ���� �� ������ ����Ǵ� �ð��� ��ٸ�
    IEnumerator SessionListUIDelay()
    {
        //�κ� �����ϱ� ���� ������ ���� �Ǵ� �ð��� �־�� �� ���߿� �ε�ȭ������ ó��
        yield return new WaitForSeconds(5f);
        RefreshSessionListUI();
        sessionListCanvas.SetActive(true);
    }

    //���� ����Ʈ ĵ���� ����
    public void SessionListDisable()
    {
        sessionListCanvas.SetActive(false);
    }

    //���� ������ ������Ʈ �Ǿ��� �� ������ �޾ƿ�
    public void ListUpdated(List<SessionInfo> sessionList)
    {
        sessions.Clear();
        sessions = sessionList;

        RefreshSessionListUI();
    }

    //���ΰ�ħ ��ư
    public void RefreshSessionListUI()
    {
        //���� ��ħ�� ���� ���� ���ǵ��� ����
        foreach (Transform child in sessionListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo child in sessions)
        {
            if (child.IsVisible)
            {
                GameObject entry = GameObject.Instantiate(sessionEntryPrefab, sessionListContent);
                SessionEntryPrefab script = entry.GetComponent<SessionEntryPrefab>();
                script.sessionName.text = child.Name;
                script.playerCount.text = child.PlayerCount + "/" + child.MaxPlayers;

                if (child.IsOpen == false || child.PlayerCount >= child.MaxPlayers)
                {
                    script.joinButton.interactable = false;
                }
                else
                {
                    script.joinButton.interactable = true;
                }
            }
        }
    }

    
    

}
