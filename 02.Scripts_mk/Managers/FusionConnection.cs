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
    [Header("Runner Reference")] //������ ���忡 ���õ� �����͵�
    [SerializeField] GameObject runnerObject;
    [SerializeField] Photon.Voice.Unity.Recorder recorder;
    [SerializeField] GameObject speaker;
    [HideInInspector] public NetworkRunner runner;
    [HideInInspector] public FusionVoiceClient voiceClient;

    [Header("Player Reference")] //�÷��̾�� ���õ� �����͵�
    [SerializeField] NetworkObject playerPrefab;
    [HideInInspector] public NetworkString<_32> _playerName = null;
    [HideInInspector] public NetworkObject playerObject;
    #endregion


    //���� �Ŵ�����
    #region Manager 
    [Header("Session List Manager")] //�� ��� ���� �Ŵ���
    [SerializeField] GameObject sessionListManagerPrefab;
    [HideInInspector] public SessionListManager _sessionListManager;

    [Header("Session Manager")] //�� ���� ������ �ٷ�� �Ŵ���
    [SerializeField] GameObject sessionManagerPrefab;
    [HideInInspector] public SessionManager _sessionManager;

    [Header("Select Manager")] //ĳ���� ���� ȭ�鿡 ���� �Ŵ���
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

    
    //������ ���� �� ��ϵ��� �� �� ����
    public void ConnectToLobby(string playerName)
    {
        _playerName = playerName;

        //ruuner�� ������ �ʱ�ȭ
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

    //���ǿ� ���� - ���� �濡 ���� �Ͱ� ����
    public async void ConnectToSession(string sessionName)
    {
        //�� ��� UI ���ֱ�
        _sessionListManager.SessionListDisable();

        //ruuner�� ������ �ʱ�ȭ
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            voiceClient = gameObject.AddComponent<FusionVoiceClient>();
            voiceClient.PrimaryRecorder = Resources.Load<Photon.Voice.Unity.Recorder>("[Recorder]");
            voiceClient.SpeakerPrefab = Resources.Load<GameObject>("[Speaker]");
        }
        
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared, //���� ��带 �������� ����
            SessionName = sessionName, //���� �̸��� ���� 
        });

        voiceClient.ConnectAndJoinRoom();
    }

    //�� �����
    public async void CreatSession()
    {
        //�� ��� UI ���ֱ�
        _sessionListManager.SessionListDisable();
        string title = _sessionListManager.ReturnTitle();

        string randomSessionName;

        //�� ���� �� �̸��� ���� �ʾ����� ���� ���� | �����ٸ� �� �̸�����
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

        //ruuner�� ������ �ʱ�ȭ
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            voiceClient = gameObject.AddComponent<FusionVoiceClient>();
            voiceClient.PrimaryRecorder = Resources.Load<Photon.Voice.Unity.Recorder>("[Recorder]");
            voiceClient.SpeakerPrefab = Resources.Load<GameObject>("[Speaker]");
        }

        //������ ������
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared, //���� ��带 �������� ����
            SessionName = randomSessionName, //���� �̸��� ���� 
            // Scene = 3, ���� �ε����� ������ ��
            PlayerCount = 4, //���� ������ �÷��̾��� ��
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            
        });

        voiceClient.ConnectAndJoinRoom();
    }

    #region CallBack Method
    //�����ڸ� ����
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
            CreatePlayerNManager(player); //�÷��̾�� sessionManager�� ������
            _sessionManager.SetRoomName(runner.SessionInfo.Name);
            StartCoroutine(_sessionManager.UpdatePlayerListUI()); //UI���� ���ġ �� 
        }
    }
   
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            _sessionManager.ChangeRoomMaster(); //������ �ٲ� ���� ��������
            _sessionManager.DeleteNameFromDictionaryRpc(player); //sessionManager�� ��ųʸ����� �ش� �÷��̾ ����
            StartCoroutine(_sessionManager.UpdatePlayerListUI()); //UI���� ���ġ ��
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
            if (runner != null) //���� ������ ������ �������Ƿ� �̹� �ִ� �͵��� �� �ı��ϰ� ������ ���� ����
            {
                Destroy(voiceClient);
                Destroy(runner);
            }
            if (_sessionManager != null)
            {
                _sessionManager.ExitRoom(); //panel ���� 
            }
            _sessionListManager.RefreshSessionListUI(); //���ΰ�ħ ��ư
            ConnectToLobby(_playerName.ToString()); //�κ� ����
        }
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
    #endregion

    //���ǿ��� �ʿ��� �Ŵ����� �÷��̾ �����.
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

    //����Ʈ �Ŵ��� �����ϱ�
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
