using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UserData : NetworkBehaviour
{
    //플레이어 이름, 방장 여부, 준비 완료 여부, 팀 여부 등 데이터 변경 시 함수가 실행되는 것들
    [Networked(OnChanged = nameof(UpdateName))] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public bool isRoomMaster { get; set; }
    [Networked(OnChanged = nameof(UpdateReadyState))] public bool isReady { get; set; }
    [Networked(OnChanged = nameof(UpdateMyTeam))] public Team WhereTeam { get { return team; } set { } }

    //채팅 및 보이스 기능
    public ChattingManager chattingManager;
    public PushToTalk voiceController;

    //팀 정보를 enum 타입으로 받음
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

    //이름을 띄울 TMPro
    [SerializeField] TextMeshPro playerNameLabel;

    //생성 시 실행
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

        //현재 sessionManager 함수를 호출하여 팀을 설정
        WhereTeam = instance._sessionManager.PlayerTeamSetting();

        DontDestroyOnLoad(this);
    }

    //일단 현재는 네임 라벨을 안쓰고 있으므로 주석처리
    protected static void UpdateName(Changed<UserData> changed)
    {
       // changed.Behaviour.playerNameLabel.text = changed.Behaviour.PlayerName.ToString();
    }

    //준비완료 버튼을 누르면 UI를 동기화 시켜줌
    protected static void UpdateReadyState(Changed<UserData> changed)
    {
        if(SceneManager.GetActiveScene().name == "StartScene")
        FusionConnection.instance._sessionManager.UpdatePlayerListUIRpc();
    }

    //팀이 변경되면 chattingManager에 MyTeam을 변경
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
