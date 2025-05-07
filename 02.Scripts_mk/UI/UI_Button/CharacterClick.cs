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

    //캐릭터 선택 씬에서 버튼을 초기화 함 | 이 때 버튼에 기능을 추가 함
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
