using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SessionButton : MonoBehaviour
{
    public Button StartOrReadyBtn;
    public Button TeamChangeBtn;
    public Button ExitBtn;

    private SessionManager sessionManager;
    private NetworkObject playerObject;
    private PlayerRef playerRef;


    private void OnEnable()
    {
        StartCoroutine(Init());
        TeamChangeBtn.onClick.AddListener(() => { ChangeTeam(); });
        ExitBtn.onClick.AddListener(() => { ExitSession(); });
    }

    //초기화 StartOrReadyInit()함수도 호출 하여 준비완료 및 시작하기 버튼을 설정 함
    public IEnumerator Init()
    {
        yield return new WaitForFixedUpdate();

        sessionManager = FusionConnection.instance._sessionManager;
        playerObject = FusionConnection.instance.playerObject;
        StartOrReadyInit();
    }

    //팀 변경 버튼 클릭 시 실행
    public void ChangeTeam()
    {
        sessionManager.TeamChange(FusionConnection.instance.runner.LocalPlayer);
        sessionManager.UpdatePlayerListUIRpc();
    }

    //방 나가기 버튼 눌렀을 시 실행
    public void ExitSession()
    {
        sessionManager.DeleteNameFromDictionaryRpc(FusionConnection.instance.runner.LocalPlayer);
        StartCoroutine(Exit());
    }

    //딕셔너리에서 플레이어를 삭제 한 후 1초 뒤에 서버를 나감
    IEnumerator Exit()
    {
        yield return new WaitForSeconds(1f);
        FusionConnection.instance.runner.Shutdown();
    }

    //내가 방장이라면 시작하기 버튼과 시작하기 기능 | 아니라면 준비하기 버튼과 준비하기 기능으로 설정함
    public void StartOrReadyInit()
    {
        StartOrReadyBtn.onClick.RemoveAllListeners();

        if (playerObject.GetComponent<UserData>().isRoomMaster)
        {
            StartOrReadyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "시작하기";
            StartOrReadyBtn.onClick.AddListener(() => { sessionManager.EnterGameRpc(); });
        }
        else
        {
            StartOrReadyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "준비하기";
            StartOrReadyBtn.onClick.AddListener(() => { playerObject.GetComponent<UserData>().Ready(); });
        }
    }
}
