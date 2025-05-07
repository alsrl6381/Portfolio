using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrNetworkData : NetworkBehaviour
{
    [SerializeField] private Sprite[] RedPortrait;
    [SerializeField] private Sprite[] BluePortrait;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image arImage;
    [SerializeField] private Image shiledImage;
    [SerializeField] private Image pistolImage;
     
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void ChangeData_RPC(string currentMoney,int arIdx,int pistolIdx,int shieldIdx)
    {
        money.text = currentMoney;
        arImage.sprite = MoneyManager.Instance.weaponList[arIdx];
        pistolImage.sprite = MoneyManager.Instance.weaponList[pistolIdx];
        shiledImage.sprite = MoneyManager.Instance.weaponList[arIdx];
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ChangePortraitData_RPC(int index,UserData.Team team)
    {
        if(team == UserData.Team.Red)
        {
            portraitImage.sprite = RedPortrait[index];
        }
        else
        {
            portraitImage.sprite = BluePortrait[index];
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ChangeArData_RPC(int arIdx)
    {
        arImage.sprite = MoneyManager.Instance.weaponList[arIdx];
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ChangePistolData_RPC(int pistolIdx)
    {
        pistolImage.sprite = MoneyManager.Instance.weaponList[pistolIdx];
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ChangeShieldData_RPC(int shieldIdx)
    {
        shiledImage.sprite = MoneyManager.Instance.weaponList[shieldIdx];
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ChangeMoneyData_RPC(int money)
    {
        this.money.text = money.ToString();
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void SetParent_RPC(UserData.Team team)
    {
        transform.SetParent(MoneyManager.Instance.CrPanel.transform);

        if (team != FusionConnection.instance.playerObject.GetBehaviour<UserData>().WhereTeam)
        {
            gameObject.SetActive(false);
        }
    }

    public override void Spawned()
    {
    }
}
