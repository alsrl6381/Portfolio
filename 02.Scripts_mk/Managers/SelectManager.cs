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
    //플레이어 숫자
    public int playerCount = 4;

    public bool isConfirm = false;

    SelectCanvas selectCanvas;

    GameObject SelectPanel;

    //myPortrait의 부모가 되는 패널
    private GameObject UserLayOut;

    // playerPortrait 프리팹 및 저장 할 객체
    [SerializeField] private GameObject PlayerPortrait;
    private NetworkObject myPortrait;

    // 내 UserData
    private UserData myUserData;

    //버튼 관련 정보
    List<Button> buttons = new List<Button>();
    List<bool> selectableButtons = new List<bool>();
    private int currentBtnIndex;
    private Button ConfirmButton;

    //씬 이름 | 현재 버튼 인덱스 (내가 선택한 버튼)
    private string sceneName;

    //시간 관련 정보
    [SerializeField] private float TimeLength = 500f;
    [Networked] private TickTimer timer { get; set; }
    private TextMeshProUGUI TimeText;

    #region Rpc List
    [Rpc(RpcSources.All, RpcTargets.All)] //왼쪽에 플레이어 정보를 만들고 PlayerPanel에 넣는 작업을 모두에게 전달 / Index는 플레이어 배열의 순서 구별하기 위해 사용 
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

    [Rpc(RpcSources.All, RpcTargets.All)] //플레이어가 클릭 해서 정보를 변경할 시 모든 플레이어에게 전달
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

    //외부 스크립트에서 Rpc 호출 할 떄
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

    //변수 세팅
    public override void Spawned()
    {                                     
        //UI 찾기
        selectCanvas = FindObjectOfType<SelectCanvas>();

        SelectPanel = selectCanvas.SelectPanel;
        ConfirmButton = selectCanvas.ConfirmButton;
        UserLayOut = selectCanvas.UserLayOut;
        TimeText = selectCanvas.TimeText;


        //버튼 찾기
        foreach (Transform btn in SelectPanel.GetComponentsInChildren<Transform>())
        {
            if (btn != SelectPanel.transform)
            {
                buttons.Add(btn.GetComponent<Button>());
                selectableButtons.Add(true);
            }
        }
        
        //현재 선택 버튼 , 현재 선택 가능한 버튼 리스트
        currentBtnIndex = -1;

        sceneName = SceneManager.GetActiveScene().name;

        //확정 버튼
        ConfirmButton.onClick.AddListener(() => { Confirm(); });

        DontDestroyOnLoad(this);
    }

    //초상화 및 보이스 세팅
    private void Start()
    {
        if(!HasStateAuthority)
        {
            return;
        }

        //모든 플레이어 정보를 가져와서 반복하는데 만약 해당 플레이어가 내 것이라면 실행
        UserData[] playerList = new UserData[playerCount];
        playerList = FindObjectsOfType<UserData>();

        for (int i=0; i<playerList.Length; i++)
        {
            UserData player = playerList[i];
            
            if (player.HasStateAuthority)
            {
                myUserData = player;
                myPortrait = Runner.Spawn(PlayerPortrait);
                myPortrait.GetComponent<PortraitData>().Mark.text = "나";
                player.voiceController.EnterTeamVoice();
                SetParentPortraitRpc(myPortrait, player.PlayerName.ToString(),sceneName,i);
            }
        }

        //타이머 생성
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

            TimeText.text = "남은 시간 " + minute.ToString() + " : " + second.ToString();
           // timeText.text = time.ToString();
        }

        if (timer.Expired(Runner))
        {
            // 한 번도 선택하지 않았다면 
            if (currentBtnIndex == -1)
            {
                for (int i = 0; i < selectableButtons.Count; i++)
                {
                    if (selectableButtons[i]) ChangePortrait(i);
                }
            }

            myUserData.CharacterIndex = currentBtnIndex;

            timer = TickTimer.None;

            //시간 종료 시 씬 이동
            ClientSceneManager.instance.GoGameMap("SampleScene");
        }
    }
    //현재 플레이어 목록을 정렬하는 코루틴
    IEnumerator SortingDelay(NetworkObject obj)
    {
        yield return new WaitForSeconds(2f);
        obj.transform.SetSiblingIndex(obj.GetComponent<PortraitData>().Id%2);
    }
}



