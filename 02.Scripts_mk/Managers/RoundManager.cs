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

    public bool isReady;//�غ� �����?
    public bool isPlaying; // �ΰ��� �����?
    public bool isBomb; //��ź ��ġ �ƴ�?
    public bool isOver;//���� ������?
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

    public static RoundManager Instance //�̱��� 
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
        //�ð� �ȿ� ���� �� ���� �� �й�
        //�ð� �ȿ� �� �������� ������ �й�

        //��ź�� ��ġ�Ǹ� �ð��� ����
        //��ź�� ���Ľ�Ű�� ������ �¸�
        //��ź�� ������Ű�� ����� �¸�

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

       
        //��� ���� �� �׾��ٸ� ������ �¸�
        if (isBlueAllDead == true)
        {
            RedWin();
            timer.SkipPlayingTime();
            return;
        }

        if (isBomb == true) return;

        //���� ���� �� �׾��ٸ� ����� �¸� 
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

    //���� ����� �� ������ ���� / ó������ ���� ����
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

    public void RoundOver() //���尡 ������ �Ȱ��� ������ �ʱ�ȭ ���ִ� �Լ�
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
