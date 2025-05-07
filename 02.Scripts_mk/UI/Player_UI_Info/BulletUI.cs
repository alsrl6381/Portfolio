using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    private TextMeshProUGUI currentBullet;
    private TextMeshProUGUI remainBullet;

    private void Awake()
    {
        currentBullet = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        remainBullet = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        currentBullet.text = "0 /";
        remainBullet.text = "0";
    }

    public void Init()
    {
        currentBullet = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        remainBullet = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        currentBullet.text = "0 /";
        remainBullet.text = "0";
    }

    public void SetCurrentBullet(int count)
    {
        currentBullet.text = count.ToString()+" /";
    }
    public void SetRemainBullet(int count)
    {
        remainBullet.text = count.ToString();
    }
}
