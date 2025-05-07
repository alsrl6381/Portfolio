using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshButton : MonoBehaviour
{
    //���ΰ�ħ ��ư
    public Button refreshButton;

    private void Start()
    {
        if(refreshButton == null)
        {
            refreshButton = GetComponent<Button>(); 
        }
        refreshButton.onClick.AddListener(Refresh);
    }


    //���� ��ħ ���
    public void Refresh()
    {
        StartCoroutine(RefreshWait());
        Debug.Log("Refresh");
    }

    //���� ��ħ�� ������ ���� interative�� �����Ͽ� �ݺ������� ���� �� ���� ����
    private IEnumerator RefreshWait()
    {
        refreshButton.interactable = false;
        FusionConnection.instance._sessionListManager.RefreshSessionListUI();

        yield return new WaitForSeconds(0.1f);
        refreshButton.interactable = true;
    }
}
