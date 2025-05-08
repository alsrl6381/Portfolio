using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

//플레이어가 방을 만들거나 조인 했을 때 생기는 방의 변화(세션)을 관리하는 매니저
public class SessionListManager : MonoBehaviour
{
    //sessionList관련 UI 및 sessionInfo에 대한 정보를 받을 List   
    private List<SessionInfo> sessions = new List<SessionInfo>();
    [SerializeField] private GameObject sessionListCanvas;
    [SerializeField] private Transform sessionListContent;
    [SerializeField] private TMP_InputField titleText;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Button createButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject sessionEntryPrefab;
    [SerializeField] private Button confirmButton;

    //초기화 UI들을 설정해줌
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

    // titlePanel키기
    public void OpenCreateSession()
    {
        titlePanel.SetActive(true);
    }

    // titlePanel키기
    public void CloseCreateSession()
    {
        titleText.text = "";
        titlePanel.SetActive(false);
    }

    // 제목 반환하기 방 생성 시 방 제목을 가져옴
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

    //로비에 접속할 때
    public void EnterSession()
    {
        StartCoroutine(SessionListUIDelay());
    }

    //로비 접속 시 서버가 연결되는 시간을 기다림
    IEnumerator SessionListUIDelay()
    {
        //로비에 입장하기 전에 서버가 접속 되는 시간을 주어야 함 나중에 로딩화면으로 처리
        yield return new WaitForSeconds(5f);
        RefreshSessionListUI();
        sessionListCanvas.SetActive(true);
    }

    //세션 리스트 캔버스 끄기
    public void SessionListDisable()
    {
        sessionListCanvas.SetActive(false);
    }

    //세션 정보가 업데이트 되었을 때 정보를 받아옴
    public void ListUpdated(List<SessionInfo> sessionList)
    {
        sessions.Clear();
        sessions = sessionList;

        RefreshSessionListUI();
    }

    //새로고침 버튼
    public void RefreshSessionListUI()
    {
        //새로 고침을 위해 기존 세션들을 삭제
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
