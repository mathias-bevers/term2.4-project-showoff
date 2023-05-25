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
    public int minRange;
    public int maxRange;

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
        WriteDebug("Current Range: " + currentRange + " [" + minRange + "] [" + maxRange + "]");
    }

    public ElementData GetElement(int aliveCount, MapGroupElement lastElement)
    {
        if (aliveCount == 0) { WriteDebug("Random Start!"); return new ElementData(GetRandomStart(), false); }

        MapGroupElement el = GetFromContinuedSupport(lastElement);

        if (aliveCount > currentRange)
            if (overrideContinuousSupport)
            {
                WriteDebug("Got Continued Support early!");
                return new ElementData(GetFromContinuedSupport(lastElement, endElements), true);
            }
            else
            {
                if (el == null) { WriteDebug("Got End"); return new ElementData(GetRandomEnd(), true); }
            }

        if (el != null) { WriteDebug("Got continued support!"); return new ElementData(el, false); }

        WriteDebug("Get random mid!");
        return new ElementData(GetRandomMiddle(), false);
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

    void WriteDebug(string line)
    {
        return;
        Debug.Log(line);
    }
}

public struct ElementData
{
    public MapGroupElement mapGroupElement;
    public bool isEnd;

    public ElementData(MapGroupElement mapGroupElement, bool isEnd)
    {
        this.mapGroupElement = mapGroupElement;
        this.isEnd = isEnd;
    }
}

[System.Serializable]
public struct ContinuousSupport
{
    public MapGroupElement lastElement;
    public List<MapGroupElement> canTransformInto;
}