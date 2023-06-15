using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupDisplayHandler : Singleton<PowerupDisplayHandler>
{
    [SerializeField] PowerupDisplay powerupDisplayPrefab;
    [SerializeField] RectTransform holderPannel;


    public void AddPowerup(PickupIdentifier identifier)
    {
        PowerupDisplay rectTransform = Instantiate(powerupDisplayPrefab, holderPannel);

        rectTransform.SetPickup(identifier);
    }

}
