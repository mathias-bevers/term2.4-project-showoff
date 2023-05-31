using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GapFiller : MonoBehaviour
{
    public bool display = true;
    public bool invertDisplay = false;
    public bool invertCollision = false;

    List<Collider> colliders = new List<Collider>();
    List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    bool doDisplay = false;

    private void Awake()
    {
        colliders.AddRange(GetComponents<Collider>()); 
        colliders.AddRange(GetComponentsInChildren<Collider>(true));
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>(true));
        meshRenderers.AddRange(GetComponents<MeshRenderer>());
    }

    public void Display(bool asActive)
    {
        if (display)

                if (!invertCollision) doDisplay = asActive;
                else doDisplay = !asActive;

        foreach (Collider c in colliders)
            if (!invertCollision) c.enabled = asActive;
            else c.enabled = !asActive;
        
    }

    private void Update()
    {
        foreach (MeshRenderer renderer in meshRenderers)
            renderer.enabled = doDisplay;
    }
}
