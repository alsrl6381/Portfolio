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

    //[Network]는 공유해야 하는 데이터가 있다면 설정해줌

    //스크립터블 오브젝트 사용 가능성 있음
    //Dictionary에 방에 접속한 플레이어들을 저장
    [Networked]
    [Capacity(4)]
    public NetworkDictionary<PlayerRef, UserData> NetPlayerDic { get; }

    //팀 카운트
    [Networked] public int RedTeamCount { get; set; }
    [Networked] public int BlueTeamCount { get; set; }

    //[Rpc]를 사용하여 다른 플레이어들에게 해당 함수를 실행하라고 알림
    #region Rpc Method List
    [Rpc(RpcSources.All,RpcTargets.All)] 
    public void UpdatePlayerListUIRpc() //갱신된 플레이어 리스트로 UI들을 다시 배치함
    {
        StartCoroutine(UpdatePlayerListUI());
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddToDictionaryRpc(PlayerRef playerRef, UserData stats) //DIctionary에 새로 들어온 플레이어 추가
    {
        if (NetPlayerDic.ContainsKey(playerRef))
        {
            Debug.Log("이미 같은 이름이 있습니다.");
        }
        else
        {
            NetPlayerDic.Add(playerRef, stats);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DeleteNameFromDictionaryRpc(PlayerRef playerRef) //Dictionary에 나간 플레이어 제거
    {
        if (NetPlayerDic.ContainsKey(playerRef))
        {
            if (NetPlayerDic[playerRef].WhereTeam == UserData.Team.Red) RedTeamCount--;
            else BlueTeamCount--;


            NetPlayerDic.Remove(playerRef);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void EnterGameRpc() //게임 시작 시 씬 이동
    {
        if (isStartable())
        {
            ClientSceneManager.instance.GoSelectScene(NetPlayerDic[Runner.LocalPlayer]);
            FusionConnection.instance.runner.SessionInfo.IsVisible = false;
            FusionConnection.instance.runner.SessionInfo.IsOpen = false;
        }
        else
        {
            Debug.Log("준비가 되지 않은 플레이어가 있습니다.");
        }
    }

    //첫 번째 플래그가 0이면 레드 팀 카운트 / 1이면 블루팀 / 두 번째 플래그가 true면 반대팀 카운트 마이너스
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void ChangeTeamCountRpc(int _flag_1,bool _flag_2 = false) //플레이어가 새로 들어오거나 팀 변경 시 팀 카운트 변경
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

    //Instatiate로 캔버스 생성
    //해당 스크립트가 생성되면 실행 
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

    //플레이어가 방에 접속 시 해당 해당 플레이어의 팀을 지정해줌 
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
    
    //팀 변경 버튼 눌렀을 때 실행 할 기능
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

    //리스트를 새로 갱신하고 UI를 초기화 함
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

    //Panel을 끔 
    public void ExitRoom()
    {
        sessionCanvas.SetActive(false);
    }

    //모든 플레이어가 준비완료가 되었다면 시작 할 수 있게 True를 리턴함
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
    
    //방장이 바뀌었으면 RoomMaster를 true로 바꾸고 준비완료 버튼을 시작버튼으로 변경
    public void ChangeRoomMaster()
    {
        if(HasStateAuthority)
        {
            //만약 권한을 가지고 있다면 방장이다.
            NetPlayerDic[Runner.LocalPlayer].isRoomMaster = true;
            NetPlayerDic[Runner.LocalPlayer].isReady = false;
            FindObjectOfType<SessionButton>().StartOrReadyInit();

        }
    }
}
