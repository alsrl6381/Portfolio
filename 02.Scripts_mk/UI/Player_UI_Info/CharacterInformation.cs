using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//캐릭터 선택창의 캐릭터 설명에 관한 UI
public class CharacterInformation : MonoBehaviour
{
    public TextMeshProUGUI position;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI description;

    public void SetData(string txt1, string txt2, string txt3)
    {
        position.text = txt1;
        characterName.text = txt2;
        description.text = txt3;
    }
}
