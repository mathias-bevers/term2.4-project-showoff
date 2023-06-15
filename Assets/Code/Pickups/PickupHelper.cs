using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHelper : MonoBehaviour
{
    public PickupIdentifier pickupType;
    public bool isPendingDestroy { get; set; }

    [SerializeField] List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    public void SetIdentifier(PickupIdentifier identifier)
    {
        pickupType = identifier;
        Sprite gottenSprite = Player.Instance.GetSprite(identifier);
        foreach(SpriteRenderer renderer in renderers)
        {
            renderer.sprite = gottenSprite;
        }
    }

}
