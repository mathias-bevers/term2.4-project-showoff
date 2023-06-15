using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : EraObject
{
    public override List<GameObject> Display(Era era)
    {
        List<GameObject> spawnedElements = base.Display(era);

        foreach(GameObject element in spawnedElements)
        {
            PickupHelper helper = element.GetComponent<PickupHelper>();
            if (helper == null) continue;
            int randomNum = Random.Range(0, 6);
            if (randomNum == 0) helper.pickupType = PickupIdentifier.AddHeart;
            else if (randomNum == 1) helper.pickupType = PickupIdentifier.RemoveHeart;
            else if (randomNum == 2) helper.pickupType = PickupIdentifier.Slippery;
            else if (randomNum == 3) helper.pickupType = PickupIdentifier.SlowdownRework;
            else if (randomNum == 4) helper.pickupType = PickupIdentifier.SpeedupRework;
            else if (randomNum == 5) helper.pickupType = PickupIdentifier.Invincible;
        }

        return spawnedElements;
    }
}
