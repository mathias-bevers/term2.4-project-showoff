using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHelper : MonoBehaviour
{
    public PickupIdentifier pickupType;
    public bool isPendingDestroy { get; set; }

    [SerializeField] List<PickupIdentifier> goodOnes = new List<PickupIdentifier>();

    [SerializeField] List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    [SerializeField] GameObject goodBox;
    [SerializeField] GameObject badBox;

    public void SetIdentifier(PickupIdentifier identifier)
    {
        pickupType = identifier;
        Sprite gottenSprite = Player.Instance.GetSprite(identifier);
        foreach(SpriteRenderer renderer in renderers)
        {
            renderer.sprite = gottenSprite;
        }

        if(goodOnes.Contains(identifier) )
        {
            goodBox.SetActive(true);
            badBox.SetActive(false);
        }
        else
        {
            goodBox.SetActive(false);
            badBox.SetActive(true); 
        }
    }

}
