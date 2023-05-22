using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Map Generation/Map Group", fileName = "MapGroup", order = 11)]
public class MapGroup : ScriptableObject
{
    public List<MapGroupElement> interchangableElements = new List<MapGroupElement>();
    public List<MapGroupElement> startElements = new List<MapGroupElement>();
    public List<MapGroupElement> endElements = new List<MapGroupElement>();

    public List<ContinuousSupport> continuousSupports = new List<ContinuousSupport>();


    public List<MapGroup> nextMapGroups = new List<MapGroup>();


    public MapGroupElement GetElement(int aliveCount, MapGroupElement lastElement)
    {
        if (aliveCount == 0) return startElements.GetRandomElement();
        if (lastElement != null)
            foreach (ContinuousSupport supportEl in continuousSupports)
                if (supportEl.lastElement == lastElement)
                    return supportEl.canTransformInto.GetRandomElement();
            
        return interchangableElements.GetRandomElement();

    }
}

[System.Serializable]
public struct ContinuousSupport
{
    public MapGroupElement lastElement;
    public List<MapGroupElement> canTransformInto;
}

public static class MapGroupElementHelper
{
    public static MapGroupElement RandomElement(this List<MapGroupElement> mapGroupElements) => mapGroupElements[Random.Range(0, mapGroupElements.Count)];
}