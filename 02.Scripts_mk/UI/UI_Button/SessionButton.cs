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

    //�ʱ�ȭ StartOrReadyInit()�Լ��� ȣ�� �Ͽ� �غ�Ϸ� �� �����ϱ� ��ư�� ���� ��
    public IEnumerator Init()
    {
        yield return new WaitForFixedUpdate();

        sessionManager = FusionConnection.instance._sessionManager;
        playerObject = FusionConnection.instance.playerObject;
        StartOrReadyInit();
    }

    //�� ���� ��ư Ŭ�� �� ����
    public void ChangeTeam()
    {
        sessionManager.TeamChange(FusionConnection.instance.runner.LocalPlayer);
        sessionManager.UpdatePlayerListUIRpc();
    }

    //�� ������ ��ư ������ �� ����
    public void ExitSession()
    {
        sessionManager.DeleteNameFromDictionaryRpc(FusionConnection.instance.runner.LocalPlayer);
        StartCoroutine(Exit());
    }

    //��ųʸ����� �÷��̾ ���� �� �� 1�� �ڿ� ������ ����
    IEnumerator Exit()
    {
        yield return new WaitForSeconds(1f);
        FusionConnection.instance.runner.Shutdown();
    }

    //���� �����̶�� �����ϱ� ��ư�� �����ϱ� ��� | �ƴ϶�� �غ��ϱ� ��ư�� �غ��ϱ� ������� ������
    public void StartOrReadyInit()
    {
        StartOrReadyBtn.onClick.RemoveAllListeners();

        if (playerObject.GetComponent<UserData>().isRoomMaster)
        {
            StartOrReadyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "�����ϱ�";
            StartOrReadyBtn.onClick.AddListener(() => { sessionManager.EnterGameRpc(); });
        }
        else
        {
            StartOrReadyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "�غ��ϱ�";
            StartOrReadyBtn.onClick.AddListener(() => { playerObject.GetComponent<UserData>().Ready(); });
        }
    }
}
