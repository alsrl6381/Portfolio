using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UserData : NetworkBehaviour
{
    //�÷��̾� �̸�, ���� ����, �غ� �Ϸ� ����, �� ���� �� ������ ���� �� �Լ��� ����Ǵ� �͵�
    [Networked(OnChanged = nameof(UpdateName))] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public bool isRoomMaster { get; set; }
    [Networked(OnChanged = nameof(UpdateReadyState))] public bool isReady { get; set; }
    [Networked(OnChanged = nameof(UpdateMyTeam))] public Team WhereTeam { get { return team; } set { } }

    //ä�� �� ���̽� ���
    public ChattingManager chattingManager;
    public PushToTalk voiceController;

    //�� ������ enum Ÿ������ ����
    public enum Team
    {
        Default,
        Red,
        Blue
    }
    private Team team;

    private Vector3 spawnPosition;

    private int characterIndex;

    public Vector3 SpawnPosition { 
        get{ return spawnPosition; } 
        set{ spawnPosition = value; }
    }

    public int CharacterIndex { 
        get { return characterIndex; } 
        set { characterIndex = value; } 
    }

    //�̸��� ��� TMPro
    [SerializeField] TextMeshPro playerNameLabel;

    //���� �� ����
    public override void Spawned()
    {
        if (this.HasStateAuthority)
        {
            FusionConnection runner = FusionConnection.instance;

            PlayerName = runner._playerName;

            if (runner._sessionManager.HasStateAuthority)
            {
                isRoomMaster = true;
            }
        }
    }

    private void Start()
    {
        var instance = FusionConnection.instance;

        //���� sessionManager �Լ��� ȣ���Ͽ� ���� ����
        WhereTeam = instance._sessionManager.PlayerTeamSetting();

        DontDestroyOnLoad(this);
    }

    //�ϴ� ����� ���� ���� �Ⱦ��� �����Ƿ� �ּ�ó��
    protected static void UpdateName(Changed<UserData> changed)
    {
       // changed.Behaviour.playerNameLabel.text = changed.Behaviour.PlayerName.ToString();
    }

    //�غ�Ϸ� ��ư�� ������ UI�� ����ȭ ������
    protected static void UpdateReadyState(Changed<UserData> changed)
    {
        if(SceneManager.GetActiveScene().name == "StartScene")
        FusionConnection.instance._sessionManager.UpdatePlayerListUIRpc();
    }

    //���� ����Ǹ� chattingManager�� MyTeam�� ����
    protected static void UpdateMyTeam(Changed<UserData> changed)
    {
        changed.Behaviour.chattingManager.team = changed.Behaviour.WhereTeam;
        changed.Behaviour.chattingManager.ResetMyTeam();
    }

    public void InGameChattingSet()
    {
        chattingManager.InGameSet();
    }

    public void Ready()
    {
        if(isReady)
        {
            isReady = false;
        }
        else
        {
            isReady = true;
        }
    }
}
