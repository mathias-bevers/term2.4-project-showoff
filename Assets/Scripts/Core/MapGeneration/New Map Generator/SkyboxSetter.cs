using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSetter : Singleton<SkyboxSetter>
{
    [SerializeField] float changeTime = 2f;
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] Material skybox1;
    [SerializeField] Material skybox2;
    [SerializeField] Material skybox3;

    Material newSetSkybox;

    float timer = 0;
    bool change = false;
    bool instant = false;

    public void SetSkybox(Era era, bool instant = false)
    {
        timer = 0;
        Material setTo = skybox1;
        if (era == Era.Era2) setTo = skybox2;
        else if (era == Era.Era3) setTo = skybox3;
        newSetSkybox = setTo;
        change = true;
        this.instant = instant;
    }


    private void Update()
    {
        if (meshRenderer != null) HandleFade();
        if (!change) return;
        timer += Time.deltaTime;
        if (timer > changeTime * 0.5f)
        {
          
            RenderSettings.skybox = newSetSkybox;
        }
        if(timer > changeTime)
        {
            timer = 0;
            change = false;
        }
    }

    void HandleFade()
    {
        Color matColor = meshRenderer.sharedMaterial.color;
        if (timer <= 0 || change == false || instant) matColor.a = 0;
        else if(change == true)
        {
            if (timer >= 0 && timer <= changeTime * 0.5f)
                matColor.a = Utils.Map(timer, 0, changeTime * 0.5f, 0, 1);
            else if(timer > changeTime * 0.5f) matColor.a = 1 - Utils.Map(timer, changeTime * 0.5f, changeTime, 0, 1);
        }

        meshRenderer.sharedMaterial.color = matColor;
    }
}
