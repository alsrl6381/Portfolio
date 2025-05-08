using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//킬 로그가 일정 개수를 유지하도록 하는 킬 로그 매니저
public class Kill_Log_Manager : MonoBehaviour
{
    public List<Kill_Log> Logs = new List<Kill_Log>();

    public void Init()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            Logs.Add(transform.GetChild(i).GetComponent<Kill_Log>());
        }

        for (int i=0; i<Logs.Count; i++)
        {
            Logs[i].GetComponent<Kill_Log>().Init();
            Logs[i].gameObject.SetActive(false);
        }
    }

    /*
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Logs.Add(transform.GetChild(i).GetComponent<Kill_Log>());
        }

        for (int i = 0; i < Logs.Count; i++)
        {
            Logs[i].GetComponent<Kill_Log>().Init();

        }
    }
    */

    private int ActivateLog()
    {
        for (int i = 0; i < Logs.Count; i++)
        {
            if (!Logs[i].gameObject.activeSelf)
            {
                Logs[i].gameObject.SetActive(true);
                return i;
            }
        }

        for (int i = 0; i < Logs.Count; i++)
        {
            Logs[i].gameObject.SetActive(false);
        }

        return 0;
    }

    public void MakeLog(string name_1, string name_2, int code)
    {
        Logs[ActivateLog()].SetKillUI(name_1, name_2, code);
    }
}
