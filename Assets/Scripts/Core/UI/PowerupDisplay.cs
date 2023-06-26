using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PowerupDisplay : MonoBehaviour
{
    [SerializeField] PickupIdentifier identifier;

    [SerializeField] Image clockPannel;


    Image image;

    bool hasPickup = false;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetPickup(PickupIdentifier identifier)
    {
        this.identifier = identifier;
        if(image == null) image = GetComponent<Image>();
        image.overrideSprite = Player.Instance.GetSprite(identifier);
        clockPannel.overrideSprite = Player.Instance.GetSprite(identifier);
        hasPickup = true;
    }


    private void Update()
    {
        if (!hasPickup) return;

        if (!Player.Instance.EffectIsActive(identifier)) Destroy(gameObject);

        clockPannel.fillAmount = Player.Instance.EffectNearEnd(identifier);
    }
}
