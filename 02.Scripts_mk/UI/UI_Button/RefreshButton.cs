using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshButton : MonoBehaviour
{
    //새로고침 버튼
    public Button refreshButton;

    private void Start()
    {
        if(refreshButton == null)
        {
            refreshButton = GetComponent<Button>(); 
        }
        refreshButton.onClick.AddListener(Refresh);
    }


    //새로 고침 기능
    public void Refresh()
    {
        StartCoroutine(RefreshWait());
        Debug.Log("Refresh");
    }

    //새로 고침을 누르고 나서 interative를 조절하여 반복적으로 누를 수 없게 만듬
    private IEnumerator RefreshWait()
    {
        refreshButton.interactable = false;
        FusionConnection.instance._sessionListManager.RefreshSessionListUI();

        yield return new WaitForSeconds(0.1f);
        refreshButton.interactable = true;
    }
}
