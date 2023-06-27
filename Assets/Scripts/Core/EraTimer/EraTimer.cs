using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EraTimer : Singleton<EraTimer>
{
    public IntUnityEvent onEraChange;

    Era _currentEra = Era.Era1;
    public Era currentEra => _currentEra;

    public List<EraEvent> events = new List<EraEvent>();

    [SerializeField] int hourChangeEraTime = 2;
    [SerializeField] int eraCount = 2;

    int offset = 0;

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
#if DEBUG

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (_currentEra == Era.Era1) offset = hourChangeEraTime;
            else if (_currentEra == Era.Era2) offset = 0;
        }

#endif

        DateTime currentTime = System.DateTime.Now;
        int hours = currentTime.Hour + offset;
        float cur = hours / (float)hourChangeEraTime;

        curEra = (int)(cur % eraCount);
        if (lastEra != curEra)
        {
            lastEra = curEra;
            onEraChange?.Invoke(curEra);
            _currentEra = (Era)curEra;

            foreach(EraEvent e in events)
            {
                if(e.forEra == (Era)curEra)
                {
                    e.onCurrentEra?.Invoke();
                }
            }
        }
    }



}

[Serializable]
public class IntUnityEvent : UnityEvent<int> { }

[Serializable]
public struct EraEvent
{
    public UnityEvent onCurrentEra;
    public Era forEra;
}