using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//점수 UI를 관리하는 매니저
public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI RedScore;
    private TextMeshProUGUI BlueScore;
    private TextMeshProUGUI Round;
    private TextMeshProUGUI Timer;
    private GameObject StatePanel;
    private TextMeshProUGUI State;

    public enum States
    {
        Ready = 0,
        Start = 1,
        Bomb = 2,
        Red = 3,
        Blue = 4
    }


    public void Init()
    {
        Round = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Timer = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        RedScore = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        BlueScore = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        StatePanel = transform.GetChild(5).gameObject;
        State = transform.GetChild(5).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        State.text = "";
        Timer.text = "0 : 0";
        RedScore.text = 0.ToString();
        BlueScore.text = 0.ToString();
        Round.text = "Round 0";
        StatePanel.SetActive(false);
    }

    private void Awake()
    {
        Round = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Timer = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        RedScore = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        BlueScore = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        StatePanel = transform.GetChild(5).gameObject;
        State = transform.GetChild(5).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        State.text = "";
        Timer.text = "0 : 0";
        RedScore.text = 0.ToString();
        BlueScore.text = 0.ToString();
        Round.text = "Round 0";
        StatePanel.SetActive(false);
    }

    public void SetScore(int rScore, int bScore, int round)
    {
        RedScore.text = rScore.ToString();
        BlueScore.text = bScore.ToString();
        Round.text = "Round "+round.ToString();
    }

    public void TimeSet(float Time)
    {
        if (Time > 60)
        {
            Timer.text = Mathf.CeilToInt((Time / 60f) - 1).ToString() + " : " + Mathf.CeilToInt(Time % 60f - 1).ToString();
        }
        else
        {
            Timer.text = "0" + " : " + Mathf.CeilToInt(Time - 1).ToString();
        }
    }

    public void SetState(States state)
    {
        StatePanel.SetActive(true);

        if(state == States.Ready)
        {
            State.text = "준비";
            StartCoroutine(DisablePanelDelay(2f));
        }
        if(state == States.Start)
        {
            State.text = "게임 시작";
            StartCoroutine(DisablePanelDelay(2f));
        }
        if(state == States.Bomb)
        {
            State.text = "폭파물 설치";
            StartCoroutine(DisablePanelDelay(2f));
        }
        if (state == States.Red)
        {
            State.text = "레드팀 승리";
        }
        if(state == States.Blue)
        {
            State.text = "블루팀 승리";
        }
    }

    IEnumerator DisablePanelDelay(float time)
    {
        yield return new WaitForSeconds(time);
        StatePanel.SetActive(false);
    }

    public void DisablePanel()
    {
        StatePanel.SetActive(false);
    }
}
