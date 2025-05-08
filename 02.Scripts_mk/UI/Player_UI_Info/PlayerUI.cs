using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//플레이어 체력과 방어를 관리하는 UI
public class PlayerUI : MonoBehaviour
{
    private List<Transform> StatUI = new List<Transform>();

    [Range(0, 100)]
    int _hp = 0;

    [Range(0, 50)]
    int _armor = 0;


    private void Awake()
    {
        StatUI = new List<Transform>();

        _hp = 0;
        _armor = 0;

        StatUI.Add(transform.GetChild(0));
        StatUI.Add(transform.GetChild(1));
        StatUI.Add(transform.GetChild(2));

        StatUI[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = 100.ToString();
        StatUI[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = 0.ToString();
        StatUI[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = 0.ToString();
    }

    public void Init()
    {
        StatUI = new List<Transform>();

        _hp = 0;
        _armor = 0;

        StatUI.Add(transform.GetChild(0));
        StatUI.Add(transform.GetChild(1));
        StatUI.Add(transform.GetChild(2));

        StatUI[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = 100.ToString();
        StatUI[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = 0.ToString();
        StatUI[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = 0.ToString();
    }

    public void SetHP(int hp)
    {
        _hp = hp;

        StatUI[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = _hp.ToString();
    }

    public void SetArmor(int armor)
    {
        _armor = armor;

        StatUI[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = armor.ToString();
    }

    public void SetCredit(int credit)
    {
        StatUI[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = credit.ToString();
    }

}
