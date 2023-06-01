using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    bool _dead = false;

    public bool dead => _dead;

    [SerializeField] List<EffectTime> effectTimes = new List<EffectTime>();

    public List<PickupCountdown> activeEvents = new List<PickupCountdown>();

    public bool EffectIsActive(PickupIdentifier identifier)
    {
        foreach (PickupCountdown countdown in activeEvents)
            if (countdown.identifier == identifier)
                return true;

        return false;
    }

    public bool IsInStartup(PickupIdentifier identifier)
    {
        if (!EffectIsActive(identifier)) return false;
        float startupTime = 0;
        bool hasFound = false;
        foreach (EffectTime t in effectTimes)
        {
            if (t.pickupIdentifier == identifier)
            {
                hasFound = true;
                startupTime = t.startupTime;
            }
        }
        if (!hasFound) return false;
        foreach (PickupCountdown cd in activeEvents)
        {
            if (cd.identifier == identifier)
            {
                if (cd.currentTimer <= startupTime)
                    return true;
            }
        }
        return false;
    }

    public bool IsInEnd(PickupIdentifier identifier)
    {
        if (!EffectIsActive(identifier)) return false;
        float breakoutTime = 0;
        float time = 0;
        bool hasFound = false;
        foreach (EffectTime t in effectTimes)
        {
            if (t.pickupIdentifier == identifier)
            {
                hasFound = true;
                breakoutTime = t.breakoutTime;
                time = t.time;
            }
        }
        if (!hasFound) return false;
        foreach (PickupCountdown cd in activeEvents)
        {
            if (cd.identifier == identifier)
            {
                if (cd.currentTimer >= time - breakoutTime)
                    return true;
            }
        }
        return false;
    }

    public void AddPickup(int idd)
    {
        PickupIdentifier id = (PickupIdentifier)idd;
        float time = 0;
        foreach (EffectTime pickupCountdown in effectTimes)
            if (pickupCountdown.pickupIdentifier == id)
                time = pickupCountdown.time;

        if (!EffectIsActive(id)) activeEvents.Add(new PickupCountdown(id, time));
        else
        {
            for (int i = 0; i < activeEvents.Count; i++)
            {
                PickupCountdown countdown = activeEvents[i];
                if (countdown.identifier == id)
                {
                    countdown.maxTimer = time;
                    countdown.currentTimer = 0;
                }
            }
        }
    }

    public void AddPickup(PickupData data)
    {
        if (data.identifier == PickupIdentifier.Speedup)
        {
            if (!EffectIsActive(data.identifier)) activeEvents.Add(new PickupCountdown(data.identifier, data.parameters.decimalNumber));
            else
            {
                for (int i = 0; i < activeEvents.Count; i++)
                {
                    PickupCountdown countdown = activeEvents[i];
                    if (countdown.identifier == data.identifier)
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
        for (int i = activeEvents.Count - 1; i >= 0; i--)
        {
            activeEvents[i].currentTimer += Time.deltaTime;
            if (activeEvents[i].currentTimer >= activeEvents[i].maxTimer)
            {
                activeEvents.RemoveAt(i);
            }
        }

        if (transform.localPosition.z <= -1 || transform.localPosition.y <= -1)
            Kill();

        if (!_dead) return;
        DeathEffect.Instance.Death();
    }
}

[System.Serializable]
public struct EffectTime
{
    public PickupIdentifier pickupIdentifier;
    public float time;
    public float startupTime;
    public float breakoutTime;
}

[System.Serializable]
public class PickupCountdown
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
