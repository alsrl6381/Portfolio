using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using Photon.Voice.Fusion;         
using UnityEngine.SceneManagement;

public class FusionConnection : MonoBehaviour,INetworkRunnerCallbacks
{
    public static FusionConnection instance;

    #region Reference
    [Header("Runner Reference")] //서버와 사운드에 관련된 데이터들
    [SerializeField] GameObject runnerObject;
    [SerializeField] Photon.Voice.Unity.Recorder recorder;
    [SerializeField] GameObject speaker;
    [HideInInspector] public NetworkRunner runner;
    [HideInInspector] public FusionVoiceClient voiceClient;

    [Header("Player Reference")] //플레이어와 관련된 데이터들
    [SerializeField] NetworkObject playerPrefab;
    [HideInInspector] public NetworkString<_32> _playerName = null;
    [HideInInspector] public NetworkObject playerObject;
    #endregion


    //각종 매니저들
    #region Manager 
    [Header("Session List Manager")] //방 목록 관련 매니저
    [SerializeField] GameObject sessionListManagerPrefab;
    [HideInInspector] public SessionListManager _sessionListManager;

    [Header("Session Manager")] //방 내부 정보를 다루는 매니저
    [SerializeField] GameObject sessionManagerPrefab;
    [HideInInspector] public SessionManager _sessionManager;

    [Header("Select Manager")] //캐릭터 선택 화면에 대한 매니저
    [SerializeField] GameObject selectManager;
    [HideInInspector] public SelectManager _selectManager;

    [Header("Round Manager")]
    [SerializeField] GameObject roundManager;
    #endregion



    private void Awake()
    {
        if (instance == null) { instance = this; }
        _sessionListManager = Instantiate(sessionListManagerPrefab).GetComponent<SessionListManager>();
        runner = GetComponent<NetworkRunner>();
        voiceClient = GetComponent<FusionVoiceClient>();
    }

    
    //서버에 접속 방 목록들을 볼 수 있음
    public void ConnectToLobby(string playerName)
    {
        _playerName = playerName;

        //ruuner가 없으면 초기화
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            voiceClient = gameObject.AddComponent<FusionVoiceClient>();
            voiceClient.PrimaryRecorder = Resources.Load<Photon.Voice.Unity.Recorder>("[Recorder]");
            voiceClient.SpeakerPrefab = Resources.Load<GameObject>("[Speaker]");
        }

        runner.JoinSessionLobby(SessionLobby.Shared);

        _sessionListManager.EnterSession();
    }

    //세션에 접속 - 게임 방에 들어가는 것과 동일
    public async void ConnectToSession(string sessionName)
    {
        //방 목록 UI 없애기
        _sessionListManager.SessionListDisable();

        //ruuner가 없으면 초기화
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            voiceClient = gameObject.AddComponent<FusionVoiceClient>();
            voiceClient.PrimaryRecorder = Resources.Load<Photon.Voice.Unity.Recorder>("[Recorder]");
            voiceClient.SpeakerPrefab = Resources.Load<GameObject>("[Speaker]");
        }
        
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared, //게임 모드를 공유모드로 설정
            SessionName = sessionName, //세션 이름을 설정 
        });

        voiceClient.ConnectAndJoinRoom();
    }

    //방 만들기
    public async void CreatSession()
    {
        //방 목록 UI 없애기
        _sessionListManager.SessionListDisable();
        string title = _sessionListManager.ReturnTitle();

        string randomSessionName;

        //방 생성 시 이름을 적지 않았으면 랜덤 생성 | 적었다면 그 이름으로
        if (title == "")
        {
            int randomInt = UnityEngine.Random.Range(1000, 9999);
            randomSessionName = "Room-" + randomInt.ToString();
        }
        else
        {
            randomSessionName = title;
        }
        _sessionListManager.CloseCreateSession();

        //ruuner가 없으면 초기화
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            voiceClient = gameObject.AddComponent<FusionVoiceClient>();
            voiceClient.PrimaryRecorder = Resources.Load<Photon.Voice.Unity.Recorder>("[Recorder]");
            voiceClient.SpeakerPrefab = Resources.Load<GameObject>("[Speaker]");
        }

        //게임을 시작함
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared, //게임 모드를 공유모드로 설정
            SessionName = randomSessionName, //세션 이름을 설정 
            // Scene = 3, 씬의 인덱스를 정해줄 때
            PlayerCount = 4, //입장 가능한 플레이어의 수
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            
        });

        voiceClient.ConnectAndJoinRoom();
    }

    #region CallBack Method
    //접속자만 실행
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            CreatePlayerNManager(player); //플레이어와 sessionManager를 생성함
            _sessionManager.SetRoomName(runner.SessionInfo.Name);
            StartCoroutine(_sessionManager.UpdatePlayerListUI()); //UI들을 재배치 함 
        }
    }
   
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            _sessionManager.ChangeRoomMaster(); //방장이 바뀐 것을 설정해줌
            _sessionManager.DeleteNameFromDictionaryRpc(player); //sessionManager의 딕셔너리에서 해당 플레이어를 제거
            StartCoroutine(_sessionManager.UpdatePlayerListUI()); //UI들을 재배치 함
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            _sessionListManager.ListUpdated(sessionList);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            if (runner != null) //방을 나가면 서버가 나가지므로 이미 있던 것들을 다 파괴하고 서버를 새로 접속
            {
                Destroy(voiceClient);
                Destroy(runner);
            }
            if (_sessionManager != null)
            {
                _sessionManager.ExitRoom(); //panel 끄기 
            }
            _sessionListManager.RefreshSessionListUI(); //새로고침 버튼
            ConnectToLobby(_playerName.ToString()); //로비에 접속
        }
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
    #endregion

    //세션에서 필요한 매니저와 플레이어를 만든다.
    void CreatePlayerNManager(PlayerRef player)
    {
        _sessionManager = FindObjectOfType<SessionManager>();

        if (_sessionManager == null)
        {
            _sessionManager = runner.Spawn(sessionManagerPrefab, Vector3.zero).GetComponent<SessionManager>();
        }
        if (playerObject == null)
        {
            playerObject = runner.Spawn(playerPrefab, Vector3.zero);
            _sessionManager.AddToDictionaryRpc(player, playerObject.GetComponent<UserData>());
        }   
    }

    //셀렉트 매니저 생성하기
    public void CreateSelectManager()
    {
        _selectManager = runner.Spawn(selectManager).GetComponent<SelectManager>();
    }

    public void CreateRoundtManager()
    {
        if (playerObject.GetComponent<UserData>().isRoomMaster)
        {
            runner.Spawn(roundManager);
        }
    }
}
