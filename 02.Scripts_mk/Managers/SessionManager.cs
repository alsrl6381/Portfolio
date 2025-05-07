using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SessionManager : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject sessionCanvas;
    [SerializeField] Transform[] PlayerListContent = new Transform[2];
    [SerializeField] GameObject playerNameLabelUI;
    [SerializeField] private GameObject roomNameText;

    //[Network]�� �����ؾ� �ϴ� �����Ͱ� �ִٸ� ��������

    //��ũ���ͺ� ������Ʈ ��� ���ɼ� ����
    //Dictionary�� �濡 ������ �÷��̾���� ����
    [Networked]
    [Capacity(4)]
    public NetworkDictionary<PlayerRef, UserData> NetPlayerDic { get; }

    //�� ī��Ʈ
    [Networked] public int RedTeamCount { get; set; }
    [Networked] public int BlueTeamCount { get; set; }

    //[Rpc]�� ����Ͽ� �ٸ� �÷��̾�鿡�� �ش� �Լ��� �����϶�� �˸�
    #region Rpc Method List
    [Rpc(RpcSources.All,RpcTargets.All)] 
    public void UpdatePlayerListUIRpc() //���ŵ� �÷��̾� ����Ʈ�� UI���� �ٽ� ��ġ��
    {
        StartCoroutine(UpdatePlayerListUI());
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddToDictionaryRpc(PlayerRef playerRef, UserData stats) //DIctionary�� ���� ���� �÷��̾� �߰�
    {
        if (NetPlayerDic.ContainsKey(playerRef))
        {
            Debug.Log("�̹� ���� �̸��� �ֽ��ϴ�.");
        }
        else
        {
            NetPlayerDic.Add(playerRef, stats);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DeleteNameFromDictionaryRpc(PlayerRef playerRef) //Dictionary�� ���� �÷��̾� ����
    {
        if (NetPlayerDic.ContainsKey(playerRef))
        {
            if (NetPlayerDic[playerRef].WhereTeam == UserData.Team.Red) RedTeamCount--;
            else BlueTeamCount--;


            NetPlayerDic.Remove(playerRef);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void EnterGameRpc() //���� ���� �� �� �̵�
    {
        if (isStartable())
        {
            ClientSceneManager.instance.GoSelectScene(NetPlayerDic[Runner.LocalPlayer]);
            FusionConnection.instance.runner.SessionInfo.IsVisible = false;
            FusionConnection.instance.runner.SessionInfo.IsOpen = false;
        }
        else
        {
            Debug.Log("�غ� ���� ���� �÷��̾ �ֽ��ϴ�.");
        }
    }

    //ù ��° �÷��װ� 0�̸� ���� �� ī��Ʈ / 1�̸� ����� / �� ��° �÷��װ� true�� �ݴ��� ī��Ʈ ���̳ʽ�
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void ChangeTeamCountRpc(int _flag_1,bool _flag_2 = false) //�÷��̾ ���� �����ų� �� ���� �� �� ī��Ʈ ����
    {
        if(_flag_1 == 0)
        {
            RedTeamCount++;
            if(_flag_2) BlueTeamCount--;
        }
        else if(_flag_1 == 1) 
        {
            BlueTeamCount++;
            if (_flag_2) RedTeamCount--;
        }
    }
 #endregion

    //Instatiate�� ĵ���� ����
    //�ش� ��ũ��Ʈ�� �����Ǹ� ���� 
    public override void Spawned()
    {
        //sessionCanvas = GameObject.Find("Session Canvas");

        //MainPanel = sessionCanvas.transform.Find("MainPanel").gameObject;
        sessionCanvas.SetActive(true);

        //roomNameText = MainPanel.transform.Find("TopPanel/RoomName").gameObject;

       // PlayerListContent[0] = MainPanel.transform.Find("MiddlePanel/RedTeamPanel/TeamName");
       // PlayerListContent[1] = MainPanel.transform.Find("MiddlePanel/BlueTeamPanel/TeamName");

        RedTeamCount = 0;
        BlueTeamCount = 0;
        NetPlayerDic.Clear();

        DontDestroyOnLoad(this);
    }

    //�÷��̾ �濡 ���� �� �ش� �ش� �÷��̾��� ���� �������� 
    public UserData.Team PlayerTeamSetting()
    {
        if(RedTeamCount <= BlueTeamCount)
        {
            ChangeTeamCountRpc(0);
            return UserData.Team.Red;
        }
        else
        {
            ChangeTeamCountRpc(1);
            return UserData.Team.Blue;
        }
    }
    
    //�� ���� ��ư ������ �� ���� �� ���
    public void TeamChange(PlayerRef player)
    {
        if (NetPlayerDic.ContainsKey(player))
        {
            UserData _player = NetPlayerDic[player];

            if (_player.isReady == true) return; 

            if (_player.WhereTeam == UserData.Team.Red)
            {
                _player.WhereTeam = UserData.Team.Blue;
                ChangeTeamCountRpc(1,true);
            }
            else 
            {
                _player.WhereTeam = UserData.Team.Red;
                ChangeTeamCountRpc(0,true);
            }
        }
    }
    public void SetRoomName(string name)
    {
        roomNameText.GetComponent<TextMeshProUGUI>().text = name;
    }

    //����Ʈ�� ���� �����ϰ� UI�� �ʱ�ȭ ��
    public IEnumerator UpdatePlayerListUI()
    {
        if (sessionCanvas.activeSelf == false)
        {
            sessionCanvas.SetActive(true);
        }

        yield return new WaitForSeconds(1f);     

        for (int i = 0; i < 2; i++)
        {
            foreach (Transform child in PlayerListContent[i])
            {
                Destroy(child.gameObject);

                //if (child == PlayerListContent[i].GetChild(0)) continue;

            }
        }

        foreach (KeyValuePair<PlayerRef, UserData> items in NetPlayerDic)
        {
            GameObject temp;

            if (NetPlayerDic[items.Key].WhereTeam == UserData.Team.Red)
            {
                temp = Instantiate(playerNameLabelUI, PlayerListContent[0]);
            }
            else
            {
                temp = Instantiate(playerNameLabelUI, PlayerListContent[1]);
            }


            SessionUserUI uiData= temp.GetComponent<SessionUserUI>();

            uiData.nameText.text = NetPlayerDic[items.Key].PlayerName.ToString();
            uiData.roomMasterIcon.SetActive(NetPlayerDic[items.Key].isRoomMaster);
            uiData.Ready(NetPlayerDic[items.Key].isReady);
        }
    }

    //Panel�� �� 
    public void ExitRoom()
    {
        sessionCanvas.SetActive(false);
    }

    //��� �÷��̾ �غ�Ϸᰡ �Ǿ��ٸ� ���� �� �� �ְ� True�� ������
    public bool isStartable()
    {
        foreach (KeyValuePair<PlayerRef, UserData> items in NetPlayerDic)
        {
            if (NetPlayerDic[items.Key].isRoomMaster == false)
            {
                if (NetPlayerDic[items.Key].isReady == false)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    //������ �ٲ������ RoomMaster�� true�� �ٲٰ� �غ�Ϸ� ��ư�� ���۹�ư���� ����
    public void ChangeRoomMaster()
    {
        if(HasStateAuthority)
        {
            //���� ������ ������ �ִٸ� �����̴�.
            NetPlayerDic[Runner.LocalPlayer].isRoomMaster = true;
            NetPlayerDic[Runner.LocalPlayer].isReady = false;
            FindObjectOfType<SessionButton>().StartOrReadyInit();

        }
    }
}
