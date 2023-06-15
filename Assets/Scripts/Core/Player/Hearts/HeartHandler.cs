using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHandler : MonoBehaviour
{
    [SerializeField] List<Image> heartImages = new List<Image>();

    [SerializeField] Animator animator;

    int lastHearts = 0;

    int amount = 0;

    private void Update()
    {
        if(lastHearts != Player.Instance.hearts)
        {
            lastHearts = Player.Instance.hearts;
            SetHearts(lastHearts);
            if(amount == 0)
            ActualHeartUpdate();
            amount++;
        }
    }

    int heartAmount = 0;

    public void SetHearts(int amount)
    {
        heartAmount = amount;
        animator?.SetTrigger("Heart");
    }

    public void ActualHeartUpdate()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].enabled = i < heartAmount;
        }
        animator?.ResetTrigger("Heart");
    }
}
