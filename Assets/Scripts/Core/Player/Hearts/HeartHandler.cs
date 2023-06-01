using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHandler : MonoBehaviour
{
    [SerializeField] List<Image> heartImages = new List<Image>();

    int lastHearts = 0;

    private void Update()
    {
        if(lastHearts != Player.Instance.hearts)
        {
            lastHearts = Player.Instance.hearts;
            SetHearts(lastHearts);
        }
    }

    public void SetHearts(int amount)
    {
        for(int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].enabled = i < amount;
        }
    }
}
