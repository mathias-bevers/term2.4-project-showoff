using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private PowerupSounds ps;
    [SerializeField] private PickupIdentifier id;
    [SerializeField] private GUIStyle style;

    [Button]
    private void PlaySound()
    {
        ps.ForcePlaySound(id);
    }
}