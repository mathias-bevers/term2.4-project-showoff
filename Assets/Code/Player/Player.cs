using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    bool _dead = false;

    public bool dead => _dead;

    public List<PickupCountdown> activeEvents = new List<PickupCountdown>();

    public bool EffectIsActive(PickupIdentifier identifier)
    {
        foreach(PickupCountdown countdown in activeEvents)
            if (countdown.identifier == identifier)
                return true;
        
        return false;
    }

    public void AddPickup(PickupData data)
    {
        if(data.identifier == PickupIdentifier.Speedup)
        {
            if (!EffectIsActive(data.identifier)) activeEvents.Add(new PickupCountdown(data.identifier, data.parameters.decimalNumber));
            else
            {
                for(int i = 0; i < activeEvents.Count; i++)
                {
                    PickupCountdown countdown = activeEvents[i];
                   if(countdown.identifier == data.identifier)
                    {
                        countdown.maxTimer = data.parameters.decimalNumber;
                        countdown.currentTimer = 0;
                    }
                }
            }
        }
    }

    public void Kill()
    {
        _dead = true;
    }

    private void Update()
    {
        if (transform.localPosition.z <= -1 || transform.localPosition.y <= -1)
            Kill();

        if (!_dead) return;
        DeathEffect.Instance.Death();
    }
}

[System.Serializable]
public struct PickupCountdown
{
    public PickupIdentifier identifier;
    public float maxTimer;
    public float currentTimer;


    public PickupCountdown(PickupIdentifier identifier, float maxTimer)
    {
        this.identifier = identifier;
        this.maxTimer = maxTimer;
        this.currentTimer = 0;
    }
}
