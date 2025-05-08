using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using static UserData;

//플레이어간 채팅을 입력하고 채팅 내용을 관리하는 매니저
public class ChattingManager : NetworkBehaviour
{
    [Header("UI")]
    public GameObject chatEntryCanvas;
    public TMP_InputField chatEntryInput;
    public GameObject chatPanel;
    public TextMeshProUGUI chatContext;
    public static TextMeshProUGUI myChatContext;

    [Networked(OnChanged = nameof(IsChatState))]
    public bool isChatOn { get; set; }

    //내가 친 채팅을 저장해놓는 Queue | 플레이어 이름 | 팀 채팅인지 아닌지 확인하는 bool값
    private Queue<string> chatTextQueue = new Queue<string>();
    private NetworkString<_32> playerName;
    private bool isTeamChat;

    //내 팀 정보를 담을 객체 | 그리고 표시해주는 Text
    public static UserData.Team myTeam;
    public UserData.Team team;
    public GameObject TextStatePanel;
    public TextMeshProUGUI TextState;

    [Networked(OnChanged = nameof(LastPublicChatChanged))]
    public NetworkString<_256> lastPublicChat { get; set; }

    [Networked(OnChanged = nameof(LastTeamChatChanged))]
    public NetworkString<_256> lastTeamChat { get; set; }


    public void InGameSet()
    {
        Vector2 chatEntry = chatEntryInput.GetComponent<RectTransform>().anchoredPosition;
        chatEntryInput.GetComponent<RectTransform>().anchoredPosition = new Vector3(chatEntry.x, chatEntry.y + 100f, 0);

        Vector2 textStatePanel = TextStatePanel.GetComponent<RectTransform>().anchoredPosition;
        TextStatePanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(textStatePanel.x, textStatePanel.y + 100f, 0);

        Vector2 ChatPanel = chatPanel.GetComponent<RectTransform>().anchoredPosition;
        chatPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(ChatPanel.x, ChatPanel.y + 100f, 0);
    }

    private void Start()
    {
        //초기화
        playerName = transform.root.GetComponent<UserData>().PlayerName;

        if (HasStateAuthority)
        {
            chatEntryInput.DeactivateInputField();
            chatEntryCanvas.SetActive(false);
            chatPanel.SetActive(true);
            myChatContext = chatContext;
            myChatContext.text = "";
            chatEntryInput.onSubmit.AddListener(delegate { SendChat(); });
            myTeam = team;
            TextState.text = "<color=black>All</color>";
            chatEntryInput.characterLimit = 15;
        }
        StartCoroutine(SetMyTeam());
    }

    //팀 설정해주기 코루틴으로 하지 않으면 플레이어가 생성되고 팀이 부여되기 전에 실행되므로 LateUpdate후에 실행
    IEnumerator SetMyTeam()
    {
        yield return new LateUpdate();
        team = transform.root.GetComponent<UserData>().WhereTeam;
        if (HasStateAuthority)
        {
            myTeam = team;
        }
    }

    //팀이 변경되면 자신의 팀을 초기화
    public void ResetMyTeam()
    {
        if (HasStateAuthority)
        {
            myTeam = team;
        }
    }

    protected static void IsChatState(Changed<ChattingManager> changed)
    {
        if (changed.Behaviour.HasStateAuthority == false) return; 
        if (SceneManager.GetActiveScene().buildIndex != 3) return;

        GUI_Manager.instance.ChatOn = changed.Behaviour.isChatOn;
    }

    //전체 채팅 텍스트가 변경되면 실행
    protected static void LastPublicChatChanged(Changed<ChattingManager> changed)
    {
        string name = changed.Behaviour.playerName.ToString();
        string text = name + " : " + changed.Behaviour.lastPublicChat.ToString() + "\n";

        myChatContext.text += text;
        changed.Behaviour.chatTextQueue.Enqueue(text);

        if (myChatContext.isTextOverflowing)
        {
            string target = changed.Behaviour.chatTextQueue.Dequeue();
            myChatContext.text = myChatContext.text.Remove(0, target.Length-1);
            myChatContext.text = myChatContext.text.Remove(0, 1);
        }
    }

    //팀 채팅이 변경되면 실행
    protected static void LastTeamChatChanged(Changed<ChattingManager> changed)
    {
        if (myTeam != changed.Behaviour.team)
        {
            return;
        }

        if (changed.Behaviour.lastTeamChat == "") return;

        string name = changed.Behaviour.playerName.ToString();
        string text = name + " : " + changed.Behaviour.lastTeamChat.ToString() + "\n";

        string _text = string.Format("<color=blue>{0} : {1}</color>", name, changed.Behaviour.lastTeamChat.ToString() + "\n");

        myChatContext.text += _text;
        changed.Behaviour.chatTextQueue.Enqueue(_text);

        if (myChatContext.isTextOverflowing)
        {
            string target = changed.Behaviour.chatTextQueue.Dequeue();
            myChatContext.text = myChatContext.text.Remove(0, target.Length-1);
            myChatContext.text = myChatContext.text.Remove(0, 1);
        }
    }

    //채팅 보내기
    private void SendChat()
    {
        if (!chatEntryInput.text.IsNullOrEmpty())
        {
            if (!isTeamChat)
            {
                if (lastPublicChat == chatEntryInput.text)
                {
                    lastPublicChat = chatEntryInput.text + " ";
                }
                else
                {
                    lastPublicChat = chatEntryInput.text;
                }
            }
            else
            {
                if (lastTeamChat == chatEntryInput.text)
                {
                    lastTeamChat = chatEntryInput.text + " ";
                }
                else
                {
                    lastTeamChat = chatEntryInput.text;
                }
            }
        }
        chatEntryInput.text = "";
        chatEntryInput.DeactivateInputField();
    }

    //채팅 시작하기
    private void StartChat()
    {
        chatEntryCanvas.SetActive(true);
        chatEntryInput.ActivateInputField();
    }

    private void Update()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        if (chatEntryCanvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (isTeamChat == false)
                {
                    isTeamChat = true;
                    TextState.text = "<color=blue>Team</color>";
                }
                else
                {
                    isTeamChat = false;
                    TextState.text = "<color=black>All</color>";
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatEntryCanvas.activeSelf == false)
            {
                isChatOn = true;
                StartChat();
            }
            else
            {
                isChatOn = false;
                chatEntryCanvas.SetActive(false);
            }
        }
    }
}
