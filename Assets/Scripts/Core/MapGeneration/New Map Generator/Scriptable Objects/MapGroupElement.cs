using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGroupElement : MonoBehaviour
{
    public List<MapElement> eraElementList = new List<MapElement>();

    public LevelElement baseElement;

    public ModelHelper baseModelParent;

    Era _isEra;

    public Era isEra => _isEra;

    public void Display(Era era, bool spawnObstacles)
    {
        _isEra = era;
        foreach (MapElement element in eraElementList)
        {
            element.gameObject.SetActive(element.era == era);
            element.modelHelper?.Display(element.era == era);
        }
        if (!spawnObstacles) return;
        EraObject[] eraObjects = transform.GetComponentsInChildren<EraObject>(true);
        foreach(EraObject eraObject in eraObjects)
            eraObject.Display(era);
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

