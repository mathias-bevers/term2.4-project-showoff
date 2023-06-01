using UnityEngine;
using System;

public class PickupManager : Singleton<PickupManager>
{
    [SerializeField] private PickupData[] pickups;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PickupHelper helper = hit.transform.GetComponent<PickupHelper>();
        if (helper == null) return;

        foreach (PickupData data in pickups)
            if (data.identifier == helper.pickupType)
                data.onPickupEvent?.Invoke(data.parameters);

        Destroy(hit.transform.gameObject);
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
    Invincible
}