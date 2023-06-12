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
    public int safetyRange = 10;

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
    [InfoBox("Blocked Supports. The complete opposite of the above.")]
    public List<ContinuousSupport> blockedSupports = new List<ContinuousSupport>();

    [Space(30)]
    [Header("Next Map Groups")]
    [InfoBox("This works a LOT like Continuous support. If an end element has been spawned the next mapgroup that will spawn will be one of the specified groups in this list.")]
    public List<MapGroup> nextMapGroups = new List<MapGroup>();

    public void DeclareRange()
    {
        currentRange = Random.Range(minRange, maxRange + 1);
        WriteDebug("Current Range: " + currentRange + " [" + minRange + "] [" + maxRange + "]");
    }

    public ElementData GetElement(int aliveCount, MapGroupElement lastElement, int internalCounter = 0)
    {
        if (internalCounter >= safetyRange) return new ElementData(GetRandomStart(), false);
        if (aliveCount == 0) 
        { 
            WriteDebug("Random Start!");
            ElementData elD3 = new ElementData(GetRandomStart(), false);
            if (Blocked(lastElement, elD3)) GetElement(aliveCount, lastElement, internalCounter+1);
            else return elD3;
        }

        MapGroupElement el = GetFromContinuedSupport(lastElement);

        if (aliveCount > currentRange)
            if (overrideContinuousSupport)
            {
                WriteDebug("Got Continued Support early!");
                ElementData elD2 =  new ElementData(GetFromContinuedSupport(lastElement, endElements), true);
                if (Blocked(lastElement, elD2)) return GetElement(aliveCount, lastElement, internalCounter + 1);
                else return elD2;
            }
            else
            {
                if (el == null) 
                { 
                    WriteDebug("Got End"); 
                    ElementData elD = new ElementData(GetRandomEnd(), true);
                    if (Blocked(lastElement, elD)) return GetElement(aliveCount, lastElement, internalCounter + 1);
                    else return elD;
                }
            }

        if (el != null) 
        { 
            WriteDebug("Got continued support!");
            ElementData inD = new ElementData(el, false);
            if (Blocked(lastElement, inD)) return GetElement(aliveCount, lastElement, internalCounter + 1);
            else return inD;
        }

        WriteDebug("Get random mid!");
        ElementData endD=  new ElementData(GetRandomMiddle(), false);
        if (Blocked(lastElement, endD)) return GetElement(aliveCount, lastElement, internalCounter + 1);
         return endD;
    }

    bool Blocked(MapGroupElement lastElement, ElementData currentElement)
    {
        for (int i = 0; i < blockedSupports.Count; i++)
        {
            if (blockedSupports[i].lastElement.baseElement == lastElement.baseElement)
            {
                for (int f = 0; f < blockedSupports[i].canTransformInto.Count; f++)
                {
                    if (blockedSupports[i].canTransformInto[f].baseElement == currentElement.mapGroupElement.baseElement)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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

#pragma warning disable CS0162
    void WriteDebug(string line)
    {
        return;
        Debug.Log(line);
    }
#pragma warning restore CS0162
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