using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Frame : MonoBehaviour
{

    float deltaTime = 0f;
    int fps = 0;

    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = (int)(1.0f / deltaTime);
        Application.targetFrameRate = 60;

        text.text = fps.ToString();
    }
}
