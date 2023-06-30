using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : Singleton<PickupManager>
{
    public event Action<PickupData> pickedupPowerupEvent;
    [SerializeField] private PickupData[] pickups;
    [SerializeField] Image animationImage;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PickupHelper helper = hit.transform.GetComponent<PickupHelper>();
        if (helper == null) { return; }

        if (helper.isPendingDestroy) { return; }

        PickUpPickup(helper.pickupType);
        
        PowerupSounds.Instance.PlayPickupSound();

        helper.isPendingDestroy = true;
        Destroy(hit.transform.gameObject);
    }

    List<PickupCountdown> pickupCountdowns = new List<PickupCountdown>();

    public void PickUpPickup(PickupIdentifier pickupType, bool hasReceivedFromServer = false)
    {
        Player.Instance.animator.ResetTrigger("PlayThrowRight");
        Player.Instance.animator.ResetTrigger("PlayThrowLeft");
        Player.Instance.animator.ResetTrigger("GoodPickup");
        Player.Instance.animator.ResetTrigger("ReceiveRight");
        Player.Instance.animator.ResetTrigger("ReceiveLeft");
        foreach (PickupData pickup in pickups)
        {
            if (pickup.identifier != pickupType) { continue; }
            {
                if (pickupCountdowns.Count == 0)
                {
                    animationImage.sprite = Player.Instance.GetSprite(pickupType);
                }
            }

            if (!hasReceivedFromServer)
            {
                if (pickup.shouldSendToServer)
                {
                    pickupCountdowns.Add(new PickupCountdown(pickup.identifier, 2.80f, false));
                    if (pickupCountdowns.Count == 1)
                    {
                        if (GameSettings.IsLeftClient)
                            Player.Instance.animator.SetTrigger("PlayThrowRight");
                        else
                            Player.Instance.animator.SetTrigger("PlayThrowLeft");
                    }
                }
                else
                {
                    pickupCountdowns.Add(new PickupCountdown(pickup.identifier, 1.60f, false));
                    if (pickupCountdowns.Count == 1)
                    {
                        Player.Instance.animator.SetTrigger("GoodPickup");
                    }
                }
            }
            else
            {
                if (pickupCountdowns.Count == 1)
                {
                    if (GameSettings.IsLeftClient)
                    {
                        Player.Instance.animator.SetTrigger("ReceiveRight");
                    }
                    else
                    {
                        Player.Instance.animator.SetTrigger("ReceiveLeft");
                    }
                }
                pickupCountdowns.Add(new PickupCountdown(pickup.identifier, 2.50f, true));
            }
        }
    }

    private void Update()
    {
        for (int i = pickupCountdowns.Count - 1; i >= 0; i--)
        {
            pickupCountdowns[i].currentTimer += Time.deltaTime;
            if (pickupCountdowns[i].currentTimer >= pickupCountdowns[i].maxTimer)
            {
                foreach (PickupData pickup in pickups)
                {
                    if (pickup.identifier != pickupCountdowns[i].identifier) { continue; }
                    if (!pickupCountdowns[i].hasReceivedFromServer)
                    {
                        if (pickup.shouldSendToServer) { pickedupPowerupEvent?.Invoke(pickup); }
                        else { pickup.onPickupEvent?.Invoke(pickup.parameters); }
                        break;
                    }
                    else
                    {
                        pickup.onPickupEvent?.Invoke(pickup.parameters);
                        break;
                    }
                }
                pickupCountdowns.RemoveAt(i);
            }
        }
    }
}

[Serializable]
public enum PickupIdentifier
{
    Slowdown,
    Speedup,
    DoublePoints,
    AddObstacle,
    AddHeart,
    RemoveHeart,
    SpeedupRework,
    SlowdownRework,
    Slippery,
    Invincible,
}