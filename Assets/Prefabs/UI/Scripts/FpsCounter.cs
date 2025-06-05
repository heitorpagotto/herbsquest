using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FpsCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        
        InvokeRepeating(nameof(CalcFps), 0, 1);
    }

    void CalcFps()
    {
        float fps = 1 / Time.deltaTime;


        _text.text = $"FPS: {fps:000}";

        if (fps < 10)
            _text.color = Color.red;

        if (fps < 30)
            _text.color = Color.yellow;

        if (fps >= 30)
            _text.color = Color.green;
    }
}
