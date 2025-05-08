using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

//서버에 접속할 때 자신의 이름을 작성하는 기능
public class NameEntry : MonoBehaviour
{
    //UI 관련 정보
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] Button submitButton;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject UIcanvas;

    private void OnEnable()
    {
        nameInputField.characterLimit = 6;
    }

    //이름을 적은 후 제출하기 누를 때
    public void SubmitName()
    {
        if(nameInputField.text.IsNullOrEmpty())
        {
            return;
        }

        FusionConnection.instance.ConnectToLobby(nameInputField.text);
        UIcanvas.SetActive(false);
        canvas.SetActive(false);
    }

    //버튼 활성화 시키기
    public void ActivateButton()
    {
        submitButton.interactable = true;
    }
}
