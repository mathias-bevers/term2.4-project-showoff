using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGroupElement : MonoBehaviour
{
    public List<MapElement> eraElementList = new List<MapElement>();

    public LevelElement baseElement;

    public ModelHelper baseModelParent;

    public void Display(Era era)
    {
        foreach (MapElement element in eraElementList)
        {
            element.gameObject.SetActive(true);
            element.modelHelper?.Display(element.era == era);
        }
    }

    public MapElement GetEraModel(Era era)
    {
        foreach (MapElement element in eraElementList)
            if (element.era == era)
                return element;
        return null;
    }

    public bool ValidForEra(Era era)
    {
        foreach (MapElement element in eraElementList)
            if (element.era == era)
                return true;
        return false;
    }

}

[System.Serializable]
public struct EraElement
{
    [SerializeField] MapElement _levelElement;
    [SerializeField] Era _era;

    public MapElement levelElement { get => _levelElement; }
    public Era era { get => _era; }
}

