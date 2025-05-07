using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Kill_Log : MonoBehaviour
{
    private TextMeshProUGUI killer;
    private TextMeshProUGUI dead;

    [SerializeField] private Transform weapon;
    private List<GameObject> weapons = new List<GameObject>();

    public void Init()
    {
        killer = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        dead = transform.GetChild(3).GetComponent<TextMeshProUGUI>();


        for (int i = 0; i < weapon.childCount; i++)
        {
            weapons.Add(weapon.GetChild(i).gameObject);
            weapons[i].SetActive(false);
        }
    }

    private void Awake()
    {
        killer = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        dead = transform.GetChild(3).GetComponent<TextMeshProUGUI>();


        for (int i=0; i<weapon.childCount; i++)
        {
            weapons.Add(weapon.GetChild(i).gameObject);
            weapons[i].SetActive(false);
        }
    }

    public void SetKillUI(string name1, string name2, int code)
    {
        killer.text = name1;
        dead.text = name2;
        weapons[code].SetActive(true);
    }
}
