using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMapBuilder : MonoBehaviour
{
    public Era buildForEra;
    [SerializeField] MapGroup startGroup;
    [SerializeField] MapGroupElement starterElement;

    MapGroup activeMapGroup;

    int currentMapgroupCounter;

    int totalCount = 0;

    int _elementAmount = 0;
    public int elementAmount => _elementAmount;


    ElementRefs? activeElement;



    List<ElementRefs> allSpawnedElements = new List<ElementRefs>();
    List<ElementRefs> spawnedLevelElements = new List<ElementRefs>();
    List<ElementRefs> newSpawnedElements = new List<ElementRefs>();

    private void Awake()
    {
        activeMapGroup = startGroup;
    }

    public void BuildLevelElement()
    {
        _elementAmount++;
        if(totalCount == 0)
        {
            if (HandleOld() == null) return;
        }
        else
        {
            foreach (ElementRefs spawnedElement in spawnedLevelElements)
                foreach (LevelPoint point in spawnedElement.spawnedElement.baseElement.EndPoints)
                {
                    if (point == null) continue;

                    ElementRefs? refs;
                    if (totalCount <= 3) refs = HandleOld();
                    else refs = HandleNew(spawnedElement.OGElement);
                    if(refs == null) continue;
                    ElementRefs el = refs.Value;

                    el.spawnedElement.transform.position = point.position;
                    el.spawnedElement.transform.rotation = point.transform.rotation;
                    el.spawnedElement.transform.parent = point.transform;

                }
        }

        allSpawnedElements.AddRange(spawnedLevelElements);
        spawnedLevelElements.Clear();
        spawnedLevelElements.AddRange(newSpawnedElements);
        newSpawnedElements.Clear();

        totalCount++;
        currentMapgroupCounter++;
    }

    ElementRefs? HandleNew(MapGroupElement lastElement) => CreateElement(startGroup.GetElement(currentMapgroupCounter, lastElement));
    

    public void MoveOverElement(PlayerRig optionalRig = null)
    {
        if (activeElement == null) { Debug.Log("Active element null!"); return; }
        LevelElement levelElement = activeElement.Value.spawnedElement.baseElement;
        if (levelElement.TakenLevelPoint == null) { Debug.Log("Chose wrong side, probably should die!"); return; }

        _elementAmount--;

        //TODO: Finish move over element!
    }

    ElementRefs? CreateElement(MapGroupElement overrideElement)
    {
        if (overrideElement == null) return null;
        if (!overrideElement.ValidForEra(buildForEra)) return null;
        ElementRefs reffie = new ElementRefs(ref overrideElement, Instantiate(overrideElement));
        newSpawnedElements.Add(reffie);
        if (activeElement == null) activeElement = reffie;
        reffie.spawnedElement.Display(buildForEra);
        return reffie;
    }

    public void SetMapGroup(MapGroup mapGroup)
    {
        if (activeMapGroup == mapGroup) return;
        activeMapGroup = mapGroup;
        mapGroup.DeclareRange();
        currentMapgroupCounter = 0;
    }


    ElementRefs? HandleOld() => CreateElement(starterElement);
}

public struct ElementRefs
{
    //🚫🚫🚫🚫🚫ABSOLUTELY THE FOCK, DONT EVER!!!!!!!! CHANGE ANYTHING TO THIS. ITS THE DIRECT PREFAB!🚫🚫🚫🚫🚫
    public MapGroupElement OGElement;
    //👐👐👐👐👐Feel free to do whatever you want with this one, its the spawned in version.👐👐👐👐👐
    public MapGroupElement spawnedElement;

    public ElementRefs(ref MapGroupElement ogElement, MapGroupElement spawnedElement)
    {
        this.OGElement = ogElement;
        this.spawnedElement = spawnedElement;
    }
}
