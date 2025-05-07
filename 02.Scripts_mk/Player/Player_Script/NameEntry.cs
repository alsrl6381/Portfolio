using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class NameEntry : MonoBehaviour
{
    //UI ���� ����
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] Button submitButton;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject UIcanvas;

    private void OnEnable()
    {
        nameInputField.characterLimit = 6;
    }

    //�̸��� ���� �� �����ϱ� ���� ��
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

    //��ư Ȱ��ȭ ��Ű��
    public void ActivateButton()
    {
        submitButton.interactable = true;
    }
}
