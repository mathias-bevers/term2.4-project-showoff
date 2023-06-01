using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterDisplayer : Singleton<MeterDisplayer>
{
    [SerializeField] Image backgroundPanel;
    [SerializeField] Text textBox;

    [SerializeField] float showTime = 1.5f;
    float timer = 0;

    public void DisplayMeters(int meters)
    {
        textBox.text = meters.ToString() + " meters ran!";
        timer = showTime;
    }

    public void DisplayMeters(int meters, float overrideTime)
    {
        textBox.text = meters.ToString() + " meters ran!";
        timer = overrideTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (backgroundPanel == null) return;
        backgroundPanel.gameObject.SetActive(timer > 0);
    }
}
