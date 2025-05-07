using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUI_Manager : MonoBehaviour
{
    public List<Transform> UIList = new List<Transform>();

    public static GUI_Manager instance;

    private Kill_Log_Manager _Kill_Log_Manager;
    private ScoreManager _Score_Manager;
    private NowWeaponManager _NowWeapon_Manager;
    private PlayerUI _PlayerUI;
    private BulletUI _BulletUI;
    private UserResult _ResultScoreUI;

    public bool ChatOn = false;

    public const int Timer = 0, KillLog = 1, Now_Weapon = 2, 
        Bullet = 3, MiniMap = 4, Player = 5,VoiceChat = 6, ESC = 7 ,Result = 8;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        for(int i=0; i<transform.childCount; i++)
        {
            UIList.Add(transform.GetChild(i));
        }
    }

    private void Start()
    {
        UIList[ESC].gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Init()
    {
        RoundStart();

        _NowWeapon_Manager = UIList[Now_Weapon].GetComponent<NowWeaponManager>();
        _Kill_Log_Manager = UIList[KillLog].GetComponent<Kill_Log_Manager>();
        _Score_Manager = UIList[Timer].GetComponent<ScoreManager>();
        _PlayerUI = UIList[Player].GetComponent<PlayerUI>();
        _BulletUI = UIList[Bullet].GetComponent<BulletUI>();
        _ResultScoreUI = UIList[Result].GetChild(1).GetComponent<UserResult>();

        _Kill_Log_Manager.Init();
        _Score_Manager.Init();
        _NowWeapon_Manager.Init();
        _PlayerUI.Init();
        _BulletUI.Init();

        UIList[VoiceChat].GetChild(0).gameObject.SetActive(false);
        UIList[VoiceChat].GetChild(1).gameObject.SetActive(false);
    }

    public void RoundOver()
    {
        UIList[Timer].gameObject.SetActive(false);
        UIList[KillLog].gameObject.SetActive(false);
        UIList[Now_Weapon].gameObject.SetActive(false);
        UIList[Bullet].gameObject.SetActive(false);
        UIList[MiniMap].gameObject.SetActive(false);
        UIList[Player].gameObject.SetActive(false);
        UIList[VoiceChat].gameObject.SetActive(false);
        UIList[ESC].gameObject.SetActive(false);
    }
    public void RoundStart()
    {
        UIList[Timer].gameObject.SetActive(true);
        UIList[KillLog].gameObject.SetActive(true);
        UIList[Now_Weapon].gameObject.SetActive(true);
        UIList[Bullet].gameObject.SetActive(true);
        UIList[MiniMap].gameObject.SetActive(true);
        UIList[Player].gameObject.SetActive(true);
        UIList[VoiceChat].gameObject.SetActive(true);
        UIList[ESC].gameObject.SetActive(true);
    }

    

    public void SetTime(float Time)
    {
        _Score_Manager.TimeSet(Time);
    }
    public void SetKill_Log(string PlayerName_1, string PlayerName_2, int Weapon_Code)
    {
        _Kill_Log_Manager.MakeLog(PlayerName_1, PlayerName_2, Weapon_Code);
    }
    public void SetTalkingVoice(bool _flag)
    {
        UIList[VoiceChat].GetChild(0).gameObject.SetActive(_flag);
    }
    public void SetHearVoice(bool _flag)
    {
        UIList[VoiceChat].GetChild(1).gameObject.SetActive(_flag);
    }
    public void SetScore(int rScore, int bScore, int round)
    {
        _Score_Manager.SetScore(rScore,bScore,round);
    }
    public void SetState(ScoreManager.States state)
    {
        _Score_Manager.SetState(state);
    }
    public void DisableState()
    {
        _Score_Manager.DisablePanel();
    }
    public void SetHpUI(int hp)
    {
        _PlayerUI.SetHP(hp);
    }
    public void SetArmorUI(int armor)
    {
        _PlayerUI.SetArmor(armor);
    }
    public void SetCreditUI(int credit)
    {
        _PlayerUI.SetCredit(credit);
    }
    public void SetHasWeapon(int code)
    {
        _NowWeapon_Manager.SetWeaponUI(code);
    }
    public void SetNowWeapon(int code)
    {
        _NowWeapon_Manager.SetTemplate(code);
    }
    public void Drop(int code)
    {
        _NowWeapon_Manager.DropWeapon(code);
    }
    public void SetCurrentBullet(int count)
    {
        _BulletUI.SetCurrentBullet(count);
    }
    public void SetRemainBullet(int count)
    {
        _BulletUI.SetRemainBullet(count);
    }
    public void SetResultUI(string data,int Winner)
    {
        UIList[Result].gameObject.SetActive(true);
        _ResultScoreUI.gameObject.SetActive(true);
        _ResultScoreUI.SetText(data,Winner);
    }
}
