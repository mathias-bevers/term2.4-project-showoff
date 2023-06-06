using System;
using UnityEngine;
using UnityEngine.Events;

public class EraTimer : Singleton<EraTimer>
{
    public IntUnityEvent onEraChange;

    [SerializeField] int hourChangeEraTime = 2;

    int lastEra = -1;
    int curEra = -1;

    private void Update()
    {
        DateTime currentTime = System.DateTime.Now;
        int hours = currentTime.Hour;
        float cur = hours / (float)hourChangeEraTime;

        curEra = (int)(cur % 3);
        if (lastEra != curEra)
        {
            lastEra = curEra;
            onEraChange?.Invoke(curEra);
        }
    }
}

[Serializable]
public class IntUnityEvent : UnityEvent<int>
{

}
