using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Map Generation/Map Group Element", fileName = "MapGroupElement", order = 12)]
public class MapGroupElement : ScriptableObject
{
    public List<EraElement> eraElementList = new List<EraElement>();

    public LevelElement GetElement(Era forEra)
    {
        foreach(EraElement el in eraElementList)
        {
            if (el.era == forEra)
                return el.levelElement.levelElement;
        }
        return null;
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

