using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortraitData : NetworkBehaviour
{
    private int id;

    private Image image;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI mark;

    public int Id { 
        get { return id; } 
        set { id = value; } 
    }

    public Image Image { 
        get { return image; } 
        set { image = value; } 
    }

    public TextMeshProUGUI NameText
    {
        get { return nameText; }
        set { nameText = value; }
    }

    public TextMeshProUGUI Mark
    {
        get { return mark; }
        set { mark = value; }

    }

    public override void Spawned()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        mark = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void SetData(int id, string name)
    {
        this.id = id;
        nameText.text = name;
    }
}
