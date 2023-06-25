using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterDisplayer : Singleton<MeterDisplayer>
{
    [SerializeField] Image backgroundPanel;
    [SerializeField] float showTime = 1.5f;

    private Transform textParentTransform;
    float timer = 0;

    private void Start()
    {
        if (ReferenceEquals(backgroundPanel, null)) { throw new UnassignedReferenceException($"{nameof(backgroundPanel)} is not set in the editor!"); }

        textParentTransform = backgroundPanel.transform;
    }

    public void DisplayMeters(int meters)
    {
        textParentTransform.SetChildrenText($"{meters:n0} METERS RAN");
        timer = showTime;
    }

    public void DisplayMeters(int meters, float overrideTime)
    {
        textParentTransform.SetChildrenText($"{meters:n0} METERS RAN");
        timer = overrideTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (backgroundPanel == null) return;
        backgroundPanel.gameObject.SetActive(timer > 0);
    }
}
