using System;
using UnityEngine;
using UnityEngine.Events;

public class EraTimer : Singleton<EraTimer>
{
    public IntUnityEvent onEraChange;

    Era _currentEra = Era.Era1;
    public Era currentEra => _currentEra;

    [SerializeField] int hourChangeEraTime = 2;
    [SerializeField] int eraCount = 2;

    int lastEra = -1;
    int curEra = -1;

    public TimeSpan timeUntilNextEra
    {
        get
        {
            DateTime currentTime = System.DateTime.Now;
            int hours = currentTime.Hour;
            int cur = hours / hourChangeEraTime;
            int next = cur + 1;

            DateTime time = System.DateTime.Today;
            int lolHours = next* hourChangeEraTime;

            DateTime dayTime = time.AddHours(lolHours);

            TimeSpan timeLeft = dayTime.Subtract(System.DateTime.Now);
            return timeLeft;
        }
    }

    private void Update()
    {
        DateTime currentTime = System.DateTime.Now;
        int hours = currentTime.Hour;
        float cur = hours / (float)hourChangeEraTime;

        curEra = (int)(cur % eraCount);
        if (lastEra != curEra)
        {
            lastEra = curEra;
            onEraChange?.Invoke(curEra);
            _currentEra = (Era)curEra;
        }
    }
}

[Serializable]
public class IntUnityEvent : UnityEvent<int> { }