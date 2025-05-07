using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : NetworkBehaviour
{
    private static RoundManager instance;

    public bool isReady;//준비 라운드니?
    public bool isPlaying; // 인게임 라운드니?
    public bool isBomb; //폭탄 설치 됐니?
    public bool isOver;//라운드 끝났니?
    public bool isSpawned;

    [SerializeField] private GameObject timerPrefab;
    private UITimer timer;

    public int RedWinCount;
    public int BlueWinCount;
    public int RoundCount;

    public int FinalRound = 5;

    [Networked] public bool isRedWin { get; private set; }
    [Networked] public bool isBlueWin { get; private set; }

    [Networked]
    [Capacity(2)]
    NetworkDictionary<int, NetworkBool> RedAlive { get; }

    [Networked]
    [Capacity(2)]
    NetworkDictionary<int, NetworkBool> BlueAlive { get; }

    bool isRedAllDead;
    bool isBlueAllDead;

    private ReadyWall[] wall = new ReadyWall[2];

    public static RoundManager Instance //싱글톤 
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void SendDeadRPC(UserData.Team team, int index)
    {
        if(team == UserData.Team.Red)
        {
            RedAlive.Set(index,false);
        }
        else
        {
            BlueAlive.Set(index, false);
        }

        GameResult();
    }

    public void ResetNetworkProperty()
    {
        isRedWin = false;
        isBlueWin = false;
    }

    public void GameResult()
    {
        //시간 안에 먼저 다 죽은 팀 패배
        //시간 안에 다 못잡으면 레드팀 패배

        //폭탄이 설치되면 시간이 멈춤
        //폭탄을 폭파시키면 레드팀 승리
        //폭탄을 해제시키면 블루팀 승리

        isRedAllDead = true;

        foreach (KeyValuePair<int, NetworkBool> items in RedAlive)
        {
            if (items.Value) 
            {
               isRedAllDead = false;
            }
        }

        isBlueAllDead = true;

        foreach (KeyValuePair<int, NetworkBool> items in BlueAlive)
        {
            if (items.Value)
            {
                isBlueAllDead = false;
            }
        }

       
        //블루 팀이 다 죽었다면 레드팀 승리
        if (isBlueAllDead == true)
        {
            RedWin();
            timer.SkipPlayingTime();
            return;
        }

        if (isBomb == true) return;

        //레드 팀이 다 죽었다면 블루팀 승리 
        if (isRedAllDead == true)
        {
            BlueWin();
            timer.SkipPlayingTime();
            return;
        }
    }


    public override void Spawned()
    {
        if (null == instance && SceneManager.GetActiveScene().buildIndex == 3)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

        FirstGameStart();
    }

    public void RedWin()
    {
        isRedWin = true;
        isBlueWin = false;

        RedWinCount++;
    }

    public void BlueWin()
    {
        isRedWin = false;
        isBlueWin = true;

        BlueWinCount++;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RedWin_RPC()
    {
        isRedWin = true;
        isBlueWin = false;

        RedWinCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void BlueWin_RPC()
    {
        isRedWin = false;
        isBlueWin = true;

        BlueWinCount++;
    }

    public void ResultWindow()
    {
        if (isRedWin)
        {
            GUI_Manager.instance.SetState(ScoreManager.States.Red);
        }
        else
        {
            GUI_Manager.instance.SetState(ScoreManager.States.Blue);
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void DeleteWall_RPC()
    {
        wall[0].ReadyWallOff();
        wall[1].ReadyWallOff();
    }

    //라운드 재시작 할 때마다 실행 / 처음에는 실행 안함
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RoundRestart());
    }

    IEnumerator RoundRestart()
    {
        GUI_Manager.instance.Init();

        if(isFinalRound())
        {
            GUI_Manager.instance.RoundOver();
        }

        wall = new ReadyWall[2];
        wall = FindObjectsOfType<ReadyWall>();

        yield return new WaitForSeconds(1f);

        RoundCount++;
        SpawnManager.Instance.Spawn(true);
        GUI_Manager.instance.SetScore(RedWinCount, BlueWinCount, RoundCount);
        GUI_Manager.instance.SetCreditUI(MoneyManager.Instance.money);

        isSpawned = true;
    }

    public override void FixedUpdateNetwork()
    {
        GUI_Manager.instance.SetTime(timer.time);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void RoundOver() //라운드가 끝나면 똑같은 씬으로 초기화 해주는 함수
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GaveMoney()
    {
        UserData.Team team = FusionConnection.instance.playerObject.GetComponent<UserData>().WhereTeam;

        if (team == UserData.Team.Red)
        {
            if (isRedWin)
            {
                MoneyManager.Instance.RoundWin();
            }
            else
            {
                MoneyManager.Instance.RoundLose();
            }
        }

        else if (team == UserData.Team.Blue)
        {
            if (isBlueWin)
            {
                MoneyManager.Instance.RoundWin();
            }
            else
            {
                MoneyManager.Instance.RoundLose();
            }
        }
    }

    void FirstGameStart()
    {
        GUI_Manager.instance.Init();
        wall = new ReadyWall[2];
        wall = FindObjectsOfType<ReadyWall>();

        if (HasStateAuthority)
        {
            timer = Runner.Spawn(timerPrefab).GetComponent<UITimer>();
            timer.StartTimer();
        }
        else
        {
            timer = FindObjectOfType<UITimer>();
        }
        SpawnManager.Instance.Spawn(false);
        MoneyManager.Instance.existRoundManager = true;
        MoneyManager.Instance.RoundStart();

        isSpawned = true;
        RoundCount = 1;

        StartCoroutine(InitLate());
    }


    IEnumerator InitLate()
    {
        yield return new WaitForSeconds(1f);
        ResultScore.instance.SetPlayer_RPC(Runner.LocalPlayer);
        ResultScore.instance.SetPlayerName_RPC(Runner.LocalPlayer, FusionConnection.instance._playerName.ToString());
        ResultScore.instance.SetPlayerTeam_RPC(Runner.LocalPlayer, FusionConnection.instance.playerObject.GetComponent<UserData>().WhereTeam);
    }

    public bool isFinalRound()
    {
        if (RoundCount > FinalRound) { return true; }
        else return false;
    }

    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void AddAliveDic_RPC(UserData.Team team, int index)
    {
        if(team == UserData.Team.Red)
        {
            RedAlive.Set(index, true);
        }
        else
        {
            BlueAlive.Set(index, true);
        }
    }

    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void BombPlanted_RPC()
    {
        isBomb = true;
        isPlaying = false;
        timer.DefaultTimer();
    }
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void BombDefused_RPC()
    {
        isBomb = false;
        BlueWin_RPC();
        timer.SkipPlayingTime();
    }

    public void GameEnd()
    {
        if(RedWinCount>BlueWinCount)
        {
            ResultScore.instance.ShowResultUI_RPC(0);
        }
        else
        {
            ResultScore.instance.ShowResultUI_RPC(1);
        }
        
    }
}
