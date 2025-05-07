using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowWeaponManager : MonoBehaviour
{
    private List<Transform> primary = new List<Transform>();
    private List<Transform> secondary = new List<Transform>();
    private Transform bomb;
    private RectTransform template;

    private int previousPistol;
    private int previousWeapon;
    private int previousBomb;

    public void Init()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            primary.Add(transform.GetChild(0).GetChild(i));
        }
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            secondary.Add(transform.GetChild(1).GetChild(i));
        }
        bomb = transform.GetChild(3);

        bomb.gameObject.SetActive(false);

        template = transform.GetChild(4).GetComponent<RectTransform>();
    }

    private void Awake()
    {
        for(int i=0; i<transform.GetChild(0).childCount; i++)
        {
            primary.Add(transform.GetChild(0).GetChild(i));
        }
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            secondary.Add(transform.GetChild(1).GetChild(i));
        }
        bomb = transform.GetChild(3);

        bomb.gameObject.SetActive(false);

        template = transform.GetChild(2).GetComponent<RectTransform>();
    }

    public void SetWeaponUI(int code)
    {
        if(code == 0)
        {
            return;
        }
        else if(code < 3)
        {
            if(previousPistol != -1)
            {
                secondary[previousPistol].gameObject.SetActive(false);
            }
            secondary[code - 1].gameObject.SetActive(true);
            previousPistol = code-1;
        }
        else if(code < 8)
        {
            if (previousWeapon != -1)
            {
                primary[previousWeapon].gameObject.SetActive(false);
            }
            primary[code - 3].gameObject.SetActive(true);
            previousWeapon = code-3;
        }
        else
        {
            bomb.gameObject.SetActive(true);
        }
    }

    public void SetTemplate(int code)
    {
        if(code == 0)
        {
            template.anchoredPosition = new Vector3(0, 0, 0);
        }
        else if(code == 1)
        {
            template.anchoredPosition = new Vector3(0, 125f, 0);
        }
        else if(code ==2)
        {
            template.anchoredPosition = new Vector3(0, 225f, 0);
        }
        else if(code ==3)
        {
            template.anchoredPosition = new Vector3(0, 325f, 0);
        }
    }

    public void DropWeapon(int code)
    {
        if (code == 0)
        {
            return;
        }
        else if (code < 3)
        {
            if (previousPistol == -1) return;
            secondary[previousPistol].gameObject.SetActive(false);
            previousPistol = -1;
        }
        else if (code < 8)
        {
            if (previousWeapon == -1) return;
            primary[previousWeapon].gameObject.SetActive(false);
            previousWeapon = -1;
        }
        else
        {
            bomb.gameObject.SetActive(false);         
        }
    }
}
