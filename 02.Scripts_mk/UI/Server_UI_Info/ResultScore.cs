using Fusion;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UserData;

//맞춘 부위,미션 완료 등 플레이어의 기록을 저장하고 마지막에 보여주는 기능 
public class ResultScore : NetworkBehaviour
{
    public static ResultScore instance;

    public Dictionary<PlayerRef,UserResultData> UserDatas = new Dictionary<PlayerRef, UserResultData>();

    public override void Spawned()
    {
        if (instance == null) { instance = this; }

        DontDestroyOnLoad(gameObject);
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void SetPlayer_RPC(PlayerRef player)
    {
        UserResultData data = new UserResultData();
        UserDatas.Add(player, data);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetPlayerTeam_RPC(PlayerRef player, Team team)
    {
        if (team == Team.Red)
        {
            UserDatas[player].Team = "°ø°ÝÆÀ";
        }
        else
        {
            UserDatas[player].Team = "¼öºñÆÀ";
        }
        
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetPlayerName_RPC(PlayerRef player ,string name)
    {
        UserDatas[player].Playername = name;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetKill_RPC(PlayerRef player)
    {
        UserDatas[player].Kill_Count++;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetDeath_RPC(PlayerRef player)
    {
        UserDatas[player].death_Count++;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetHead_RPC(PlayerRef player)
    {
        UserDatas[player].Head_Count++;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetBody_RPC(PlayerRef player)
    {
        UserDatas[player].Body_Count++;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetLowerBody_RPC(PlayerRef player)
    {
        UserDatas[player].LowerBody_Count++;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetMissionCount_RPC(PlayerRef player)
    {
        UserDatas[player].Mission_Count++;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ShowResultUI_RPC(int result)
    {
        GUI_Manager.instance.SetResultUI(ResultData(),result);
    }

    public string ResultData()
    {
        string Data = "";
        foreach (KeyValuePair<PlayerRef,UserResultData> items in UserDatas)
        {
            string _data = UserDatas[items.Key].Team + ":" + UserDatas[items.Key].Playername + ":" 
                + UserDatas[items.Key].Kill_Count.ToString() + ":" + UserDatas[items.Key].death_Count.ToString() + ":"
                + UserDatas[items.Key].Mission_Count.ToString() + ":" + UserDatas[items.Key].Head_Count.ToString() + ":"
                + UserDatas[items.Key].Body_Count.ToString() + ":" + UserDatas[items.Key].LowerBody_Count.ToString();

            Data += _data + "/";
        }
       // Data.Remove(Data.Length-1, 1);
        return Data;
    }
}

public class UserResultData
{
    private string team;
    private string playername;
    private int kill_Count;
    public int death_Count;
    private int head_Count;
    private int body_Count;
    private int lowerBody_Count;
    private int mission_Count;

    public string Team { get => team; set => team = value; }
    public string Playername { get => playername; set => playername = value; }
    public int Kill_Count { get => kill_Count; set => kill_Count = value; }
    public int Head_Count { get => head_Count; set => head_Count = value; }
    public int Body_Count { get => body_Count; set => body_Count = value; }
    public int LowerBody_Count { get => lowerBody_Count; set => lowerBody_Count = value; }
    public int Mission_Count { get => mission_Count; set => mission_Count = value; }

    public UserResultData()
    {
        team = "";
        playername = "";
        kill_Count = 0;
        death_Count = 0;
        head_Count = 0;
        body_Count = 0;
        lowerBody_Count = 0;
        mission_Count = 0;
    }
}
