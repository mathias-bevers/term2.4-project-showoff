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
            if (randomNum == 0) helper.SetIdentifier(PickupIdentifier.AddHeart);
            else if (randomNum == 1) helper.SetIdentifier(PickupIdentifier.RemoveHeart);
            else if (randomNum == 2) helper.SetIdentifier(PickupIdentifier.Slippery);
            else if (randomNum == 3) helper.SetIdentifier(PickupIdentifier.SlowdownRework);
            else if (randomNum == 4) helper.SetIdentifier(PickupIdentifier.Speedup);
            else if (randomNum == 5) helper.SetIdentifier(PickupIdentifier.SpeedupRework);
        }

        return spawnedElements;
    }
}
