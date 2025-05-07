using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectManager : NetworkBehaviour
{
    //�÷��̾� ����
    public int playerCount = 4;

    public bool isConfirm = false;

    SelectCanvas selectCanvas;

    GameObject SelectPanel;

    //myPortrait�� �θ� �Ǵ� �г�
    private GameObject UserLayOut;

    // playerPortrait ������ �� ���� �� ��ü
    [SerializeField] private GameObject PlayerPortrait;
    private NetworkObject myPortrait;

    // �� UserData
    private UserData myUserData;

    //��ư ���� ����
    List<Button> buttons = new List<Button>();
    List<bool> selectableButtons = new List<bool>();
    private int currentBtnIndex;
    private Button ConfirmButton;

    //�� �̸� | ���� ��ư �ε��� (���� ������ ��ư)
    private string sceneName;

    //�ð� ���� ����
    [SerializeField] private float TimeLength = 500f;
    [Networked] private TickTimer timer { get; set; }
    private TextMeshProUGUI TimeText;

    #region Rpc List
    [Rpc(RpcSources.All, RpcTargets.All)] //���ʿ� �÷��̾� ������ ����� PlayerPanel�� �ִ� �۾��� ��ο��� ���� / Index�� �÷��̾� �迭�� ���� �����ϱ� ���� ��� 
    public void SetParentPortraitRpc(NetworkObject obj, string name, string _sceneName, int index)
    {
       
        if (sceneName != _sceneName)
        {
            return;
        }

        obj.transform.SetParent(UserLayOut.transform);
        obj.GetComponent<PortraitData>().SetData(index, name);
        StartCoroutine(SortingDelay(obj));
    }

    [Rpc(RpcSources.All, RpcTargets.All)] //�÷��̾ Ŭ�� �ؼ� ������ ������ �� ��� �÷��̾�� ����
    public void ChangePortraitRpc(NetworkObject obj, int previous, int next, string _sceneName)
    {

        if (sceneName != _sceneName)
        {
            return;
        }

        obj.GetComponent<PortraitData>().Image.sprite
            = buttons[next].GetComponent<CharacterClick>().image;

        if (previous != -1)
        {
            buttons[previous].interactable = true;
            selectableButtons[previous] = true;
            FusionConnection.instance._selectManager.selectableButtons[previous] = true;
        }
        buttons[next].interactable = false;
        selectableButtons[next] = false;
        FusionConnection.instance._selectManager.selectableButtons[next] = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ChangeConfirmButtonRpc(bool _flag)
    {
        isConfirm = true;

        if(_flag)
        {
            FusionConnection.instance._selectManager.ResetTimer(1f);
        }
    }
    #endregion

    //�ܺ� ��ũ��Ʈ���� Rpc ȣ�� �� ��
    public void ChangePortrait(int next)
    {
        if (isConfirm) return;

        ChangePortraitRpc(myPortrait ,currentBtnIndex, next,sceneName);
        ChangeCurrentBtn(next);
    }

    public void ChangeCurrentBtn(int index)
    {
        currentBtnIndex = index;
    }

    public void Confirm()
    {
        if (currentBtnIndex == -1) return;

        isConfirm = true;

        ChangeConfirmButtonRpc(isAllConfirmed());
    }

    public bool isAllConfirmed()
    {
        SelectManager[] temps = FindObjectsOfType<SelectManager>();
        foreach (SelectManager temp in temps)
        {
            if (!temp.isConfirm)
            {
                return false;
            }
        }
        return true;
    }

    public void ResetTimer(float time)
    {
        timer = TickTimer.CreateFromSeconds(Runner, time);
    }

    //���� ����
    public override void Spawned()
    {                                     
        //UI ã��
        selectCanvas = FindObjectOfType<SelectCanvas>();

        SelectPanel = selectCanvas.SelectPanel;
        ConfirmButton = selectCanvas.ConfirmButton;
        UserLayOut = selectCanvas.UserLayOut;
        TimeText = selectCanvas.TimeText;


        //��ư ã��
        foreach (Transform btn in SelectPanel.GetComponentsInChildren<Transform>())
        {
            if (btn != SelectPanel.transform)
            {
                buttons.Add(btn.GetComponent<Button>());
                selectableButtons.Add(true);
            }
        }
        
        //���� ���� ��ư , ���� ���� ������ ��ư ����Ʈ
        currentBtnIndex = -1;

        sceneName = SceneManager.GetActiveScene().name;

        //Ȯ�� ��ư
        ConfirmButton.onClick.AddListener(() => { Confirm(); });

        DontDestroyOnLoad(this);
    }

    //�ʻ�ȭ �� ���̽� ����
    private void Start()
    {
        if(!HasStateAuthority)
        {
            return;
        }

        //��� �÷��̾� ������ �����ͼ� �ݺ��ϴµ� ���� �ش� �÷��̾ �� ���̶�� ����
        UserData[] playerList = new UserData[playerCount];
        playerList = FindObjectsOfType<UserData>();

        for (int i=0; i<playerList.Length; i++)
        {
            UserData player = playerList[i];
            
            if (player.HasStateAuthority)
            {
                myUserData = player;
                myPortrait = Runner.Spawn(PlayerPortrait);
                myPortrait.GetComponent<PortraitData>().Mark.text = "��";
                player.voiceController.EnterTeamVoice();
                SetParentPortraitRpc(myPortrait, player.PlayerName.ToString(),sceneName,i);
            }
        }

        //Ÿ�̸� ����
        timer = TickTimer.CreateFromSeconds(Runner,TimeLength);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (timer.IsRunning)
        {
            int time = (int)timer.RemainingTime(Runner);

            string minute = string.Format("{0:00}", time / 60);
            string second = string.Format("{0:00}", time % 60);

            TimeText.text = "���� �ð� " + minute.ToString() + " : " + second.ToString();
           // timeText.text = time.ToString();
        }

        if (timer.Expired(Runner))
        {
            // �� ���� �������� �ʾҴٸ� 
            if (currentBtnIndex == -1)
            {
                for (int i = 0; i < selectableButtons.Count; i++)
                {
                    if (selectableButtons[i]) ChangePortrait(i);
                }
            }

            myUserData.CharacterIndex = currentBtnIndex;

            timer = TickTimer.None;

            //�ð� ���� �� �� �̵�
            ClientSceneManager.instance.GoGameMap("SampleScene");
        }
    }
    //���� �÷��̾� ����� �����ϴ� �ڷ�ƾ
    IEnumerator SortingDelay(NetworkObject obj)
    {
        yield return new WaitForSeconds(2f);
        obj.transform.SetSiblingIndex(obj.GetComponent<PortraitData>().Id%2);
    }
}



