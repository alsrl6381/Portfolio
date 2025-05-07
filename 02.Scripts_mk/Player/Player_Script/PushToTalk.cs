using Fusion;
using Photon.Voice.Unity;
using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PushToTalk : NetworkBehaviour
{
    private Recorder recorder;
    private UserData userData;
    private Speaker speaker;
    private bool otherTalk;

    public override void Spawned()
    {
        if(recorder == null)
        {
            recorder = FindObjectOfType<Recorder>();
            userData = transform.root.GetComponent<UserData>();
        }

        speaker = transform.GetChild(0).GetComponent<Speaker>();

        recorder.TransmitEnabled = false;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) { return; }

        if (!HasStateAuthority)
        {
            return;
        }


        if(Input.GetKeyDown(KeyCode.V)) 
        {
            EnableTalking();
        }

        Detection();

    }

    private void EnableTalking()
    {
        if(recorder.TransmitEnabled == true)
        {
            recorder.TransmitEnabled = false;
        }
        else
        {
            recorder.TransmitEnabled = true;
        }
    }

    public void Detection()
    {
        if (SceneManager.GetActiveScene().buildIndex != 3) return;

        if(recorder.IsCurrentlyTransmitting)
        {
            GUI_Manager.instance.SetTalkingVoice(true);
            TeamVoiceDetected_RPC(userData.WhereTeam,true);
        }
        else
        {
            GUI_Manager.instance.SetTalkingVoice(false);
            TeamVoiceDetected_RPC(userData.WhereTeam, false);
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void TeamVoiceDetected_RPC(UserData.Team team,bool active)
    {
        if (SceneManager.GetActiveScene().buildIndex != 3) return;

        if (FusionConnection.instance.playerObject.GetBehaviour<UserData>().WhereTeam == team && !HasStateAuthority)
        {
            GUI_Manager.instance.SetHearVoice(active);
        }
    } 

    public void EnterTeamVoice()
    {
        UserData.Team team = userData.WhereTeam;

        if (team == UserData.Team.Red)
        {
            byte[] groups = { 1 };
            FusionConnection.instance.voiceClient.Client.OpChangeGroups(null, groups);
            recorder.InterestGroup = 1;
        }

        if (team == UserData.Team.Blue)
        {
            byte[] groups = { 2 };
            FusionConnection.instance.voiceClient.Client.OpChangeGroups(null, groups);
            recorder.InterestGroup = 2;
        }
    }
}
