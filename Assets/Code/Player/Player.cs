using System;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    bool _dead = false;

    public bool dead => _dead;

    public event Action deathEvent;
    public Client client { get; set; }

    [SerializeField] List<EffectImage> _images = new List<EffectImage>();
    [SerializeField] List<EffectTime> effectTimes = new List<EffectTime>();

    public List<PickupCountdown> activeEvents = new List<PickupCountdown>();

    [SerializeField] public Animator animator;

    public int EffectCount()
    {
        return activeEvents.Count;
    }

    public bool EffectIsActive(PickupIdentifier identifier)
    {
        foreach (PickupCountdown countdown in activeEvents)
            if (countdown.identifier == identifier)
                return true;

        return false;
    }

    public Sprite GetSprite(PickupIdentifier identifier)
    {
        foreach (EffectImage image in _images)
            if (image.identifier == identifier)
                return image.sprite;
        return null;
    }

    public float EffectNearEnd(PickupIdentifier identifier)
    {
        if (!EffectIsActive(identifier)) return 0;

        foreach (PickupCountdown cd in activeEvents)
        {
            if (cd.identifier == identifier)
            {
                return Utils.Map(cd.currentTimer, 0, cd.maxTimer, 0, 1);
            }
        }
        return 0;
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


    [SerializeField]int maxHearts = 3;
    int _currentHearts;
    public int hearts { get => _currentHearts; private set => _currentHearts = value; }

    public override void Awake()
    {
        hearts = maxHearts;
    }

    public void AddHeart()
    {
        _currentHearts++;
    }

    public void RemoveHeart(bool destructive = false)
    {
        if (_currentHearts == 1 && !destructive) return;
        _currentHearts--;
    }

    public void AddPickup(int idd)
    {
        PickupIdentifier id = (PickupIdentifier)idd;
        //Debug.Log($"Adding pickup {id.ToString()}");
        float time = 0;
        foreach (EffectTime pickupCountdown in effectTimes)
            if (pickupCountdown.pickupIdentifier == id)
                time = pickupCountdown.time;

        if (!EffectIsActive(id)) { activeEvents.Add(new PickupCountdown(id, time)); PowerupDisplayHandler.Instance.AddPowerup(id); }
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
            if (!EffectIsActive(data.identifier))
            {
                activeEvents.Add(new PickupCountdown(data.identifier, data.parameters.decimalNumber)); 
                PowerupDisplayHandler.Instance.AddPowerup(data.identifier);
            }
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
        if (_dead) { return; }
        
        _dead = true;
        deathEvent?.Invoke();
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
        {
            if (EffectIsActive(PickupIdentifier.Invincible))
            {
                FakeDeath();
                return;
            }

            if (!EffectIsActive(PickupIdentifier.Speedup))
                _currentHearts--;
            if(_currentHearts <= 0) Kill();
            else FakeDeath();
        }

        if (!_dead) return;
        DeathEffect.Instance.Death();
    }

    [HideInInspector]
    public bool oopsIDied = false;

    void FakeDeath()
    {
        oopsIDied = true;
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
public struct EffectImage
{
    public PickupIdentifier identifier;
    public Sprite sprite;
}

[System.Serializable]
public class PickupCountdown
{
    public PickupIdentifier identifier;
    public float maxTimer;
    public float currentTimer;
    public bool hasReceivedFromServer = false;


    public PickupCountdown(PickupIdentifier identifier, float maxTimer)
    {
        this.identifier = identifier;
        this.maxTimer = maxTimer;
        this.currentTimer = 0;
    }

    public PickupCountdown(PickupIdentifier identifier, float maxTimer, bool hasReceivedFromServer)
    {
        this.identifier = identifier;
        this.maxTimer = maxTimer;
        this.currentTimer = 0;
        this.hasReceivedFromServer = hasReceivedFromServer;
    }
}
