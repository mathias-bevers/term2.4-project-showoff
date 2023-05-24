using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Map Generation/Map Group", fileName = "MapGroup", order = 11)]
public class MapGroup : ScriptableObject
{
    [Header("Range Specifications")]
    [InfoBox("Range Specifications work as follows:\n\n" +
        "If overrideContinuousSupport is false then this gets completely ignored, unless the spawned element is NOT present in the continousSupports list.\n\n" +
        "If overrideContinuousSupport is true it will still try to get the elements from the list UNTIL it hits the specified random range. Then it will automatically get an end element")]
    public bool overrideContinuousSupport = false;
    public int minRange = 4;
    public int maxRange = 10;

    int currentRange = 0;

    [Space(30)]
    [Header("Element Declaration")]
    [InfoBox("Start elements is a list of elements which it will then randomly pick from, this will be the first element that spawns.")]
    [InfoBox("Random Selectable Elements is a list of elements which it will then randomly pick from, for the specified range. It will spawn those elements.")]
    [InfoBox("Start elements is a list of elements which it will then randomly pick from, this will be the last element that spawns.")]
    public List<MapGroupElement> startElements = new List<MapGroupElement>();
    public List<MapGroupElement> randomSelectableElements = new List<MapGroupElement>();
    public List<MapGroupElement> endElements = new List<MapGroupElement>();

    [Space(30)]
    [Header("Continuous Support")]
    [InfoBox("Continuous Support is a manual override for what element comes next.\n\n" +
        "By setting Last Element to any element in the lists you can detect when that element gets spawned.\n" +
        "Once it detects that element has spawned, it will then forcefully overwrite the next element to spawn with one of the specified elements.")]
    public List<ContinuousSupport> continuousSupports = new List<ContinuousSupport>();

    [Space(30)]
    [Header("Next Map Groups")]
    [InfoBox("This works a LOT like Continuous support. If an end element has been spawned the next mapgroup that will spawn will be one of the specified groups in this list.")]
    public List<MapGroup> nextMapGroups = new List<MapGroup>();

    public void DeclareRange()
    {
        currentRange = Random.Range(minRange, maxRange + 1);
    }

    public MapGroupElement GetElement(int aliveCount, MapGroupElement lastElement)
    {
        if (aliveCount == 0) return GetRandomStart();

        if (aliveCount > currentRange)
            if (overrideContinuousSupport)
                return GetFromContinuedSupport(lastElement, endElements);
            else
            {
                MapGroupElement elem = GetFromContinuedSupport(lastElement);
                if (elem == null) return GetRandomEnd();
            }

        MapGroupElement el = GetFromContinuedSupport(lastElement);
        if (el != null) return el;

        return GetRandomMiddle();
    }

    MapGroupElement GetFromContinuedSupport(MapGroupElement lastElement, List<MapGroupElement> mustBeInThis = null)
    {
        if (lastElement == null) return null;
        List<MapGroupElement> continuousSupportList = new List<MapGroupElement>();
        foreach (ContinuousSupport supportEl in continuousSupports)
            if (supportEl.lastElement == lastElement)
            {
                if (mustBeInThis == null) continuousSupportList.AddRange(supportEl.canTransformInto);
                else
                {
                    foreach (MapGroupElement el in supportEl.canTransformInto)
                        if (mustBeInThis.Contains(el))
                            continuousSupportList.Add(el);
                }
            }
        if (continuousSupportList.Count > 0)
            return continuousSupportList.GetRandomElement();

        if (mustBeInThis != null)
            return mustBeInThis.GetRandomElement();

        return null;
    }

    MapGroupElement GetRandomStart() => startElements.GetRandomElement();
    MapGroupElement GetRandomMiddle() => randomSelectableElements.GetRandomElement();
    MapGroupElement GetRandomEnd() => endElements.GetRandomElement();
}

[System.Serializable]
public struct ContinuousSupport
{
    public MapGroupElement lastElement;
    public List<MapGroupElement> canTransformInto;
}