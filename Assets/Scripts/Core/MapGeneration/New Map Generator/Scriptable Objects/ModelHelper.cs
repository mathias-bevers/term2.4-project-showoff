using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelHelper : MonoBehaviour
{
    [SerializeField] bool isEditorVisual = false;
    
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    List<GapFiller> gapFiller = new List<GapFiller>();

    private void Awake()
    {
        meshFilters = GetComponentsInChildren<MeshFilter>(true).ToList();
        gapFiller = GetComponentsInChildren<GapFiller>(true).ToList();

        if (!isEditorVisual)
        gameObject.SetActive(false);
    }

    bool wasActive = false;

    private void OnDrawGizmos()
    {
        if (!isEditorVisual) return;
        Gizmos.color = Color.white;
        foreach (MeshFilter mesh in meshFilters)
        {
            Gizmos.DrawWireMesh(mesh.sharedMesh, mesh.transform.position, mesh.transform.rotation, mesh.transform.localScale);
        }
    }

    private void Update()
    {
        if (Player.Instance.EffectIsActive(PickupIdentifier.Speedup))
        {
            foreach(GapFiller filler in gapFiller)
                filler.Display(true);
            wasActive = true;
        }else
        {
            if (wasActive)
            {
                foreach (GapFiller filler in gapFiller)
                    filler.Display(false);
                wasActive = false;
            }
        }
    }

    public void Display(bool display)
    {
        if (isEditorVisual) return;

        gameObject.SetActive(display);
    }
}
