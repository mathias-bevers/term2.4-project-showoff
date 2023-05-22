using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathEffect : Singleton<DeathEffect>
{
    [SerializeField] Image backgroundPanel;

    [SerializeField] Image deathPanel;

    bool dead = false;
    float timer = 0;

    public override void Awake()
    {
        base.Awake();
        backgroundPanel.gameObject.SetActive(false);
        deathPanel.gameObject.SetActive(false);
    }


    public void Death()
    {
        dead = true;
    }

    void Update()
    {
        if (!dead) return;
        backgroundPanel.gameObject.SetActive(true);
        timer += Time.deltaTime;

        float filler = Mathf.Clamp01(Utils.Map(timer, 0, 2, 0, 1));

        backgroundPanel.fillAmount = filler;

        if(timer >= 2)
            deathPanel.gameObject.SetActive(true);
    }

    public void RunAgain()
    {
        SceneManager.LoadScene("Amber");
    }
}
