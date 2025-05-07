using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClick : MonoBehaviour
{
    SelectManager selectManager;

    [HideInInspector] public Button button;
    public Sprite image;
    public int index;
    public string characterName;

    [SerializeField] private string position;
    [TextArea][SerializeField] private string description;
    //private CharacterInformation info;

    private void Start()
    {
        StartCoroutine(Init());
    }

    //ĳ���� ���� ������ ��ư�� �ʱ�ȭ �� | �� �� ��ư�� ����� �߰� ��
    IEnumerator Init()
    {
        yield return new WaitForSeconds(3f);

        //info = FindObjectOfType<CharacterInformation>();

        button = GetComponent<Button>();

        SelectManager[] temp = FindObjectsOfType<SelectManager>();

        foreach(SelectManager obj in temp)
        {
            if(obj.HasStateAuthority)
            {
                selectManager = obj;
            }
        }
        button.onClick.AddListener(() => { selectManager.ChangePortrait(index); });
        //button.onClick.AddListener(() => { info.SetData(position, characterName, description); });
    }
}
