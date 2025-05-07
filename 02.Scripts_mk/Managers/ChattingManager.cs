using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using static UserData;

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

    //���� ģ ä���� �����س��� Queue | �÷��̾� �̸� | �� ä������ �ƴ��� Ȯ���ϴ� bool��
    private Queue<string> chatTextQueue = new Queue<string>();
    private NetworkString<_32> playerName;
    private bool isTeamChat;

    //�� �� ������ ���� ��ü | �׸��� ǥ�����ִ� Text
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
        //�ʱ�ȭ
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

    //�� �������ֱ� �ڷ�ƾ���� ���� ������ �÷��̾ �����ǰ� ���� �ο��Ǳ� ���� ����ǹǷ� LateUpdate�Ŀ� ����
    IEnumerator SetMyTeam()
    {
        yield return new LateUpdate();
        team = transform.root.GetComponent<UserData>().WhereTeam;
        if (HasStateAuthority)
        {
            myTeam = team;
        }
    }

    //���� ����Ǹ� �ڽ��� ���� �ʱ�ȭ
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

    //��ü ä�� �ؽ�Ʈ�� ����Ǹ� ����
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

    //�� ä���� ����Ǹ� ����
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

    //ä�� ������
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

    //ä�� �����ϱ�
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
