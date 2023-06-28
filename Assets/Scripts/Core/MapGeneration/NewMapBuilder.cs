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


    ElementRefs? _activeElement;
    public MapGroupElement activeElement => _activeElement?.spawnedElement ?? null;

    bool isEnd = false;

    Era lastEra = Era.Era20;

    [SerializeField] List<ElementRefs> allSpawnedElements = new List<ElementRefs>();
    [SerializeField] List<ElementRefs> spawnedLevelElements = new List<ElementRefs>();
    [SerializeField] List<ElementRefs> newSpawnedElements = new List<ElementRefs>();

    private void Awake()
    {
        SetMapGroup(startGroup);
    }


    public void BuildLevelElement()
    {
        if (activeMapGroup == null) return;
        _elementAmount++;
        if (totalCount == 0)
        {
            if (HandleOld() == null) { Debug.Log("Returned!"); return; }
        }
        else
        {
            foreach (ElementRefs spawnedElement in spawnedLevelElements)
            {
                int totalspawncount = 0;
                foreach (LevelPoint point in spawnedElement.spawnedElement.baseElement.EndPoints)
                {
                    if (point == null) continue;

                    ElementRefs? refs;
                    if (totalCount <= 3) refs = HandleOld();
                    else refs = HandleNew(spawnedElement.OGElement);
                    if (refs == null) continue;
                    ElementRefs el = refs.Value;

                    el.spawnedElement.transform.position = point.position - el.spawnedElement.baseElement.transform.position;
                    el.spawnedElement.transform.rotation = point.transform.rotation;
                    el.spawnedElement.transform.parent = point.transform;
                    el.spawnedElement.gameObject.SetActive(totalspawncount == 0);
                    totalspawncount++;
                }
            }
        }

        allSpawnedElements.AddRange(spawnedLevelElements);
        spawnedLevelElements.Clear();
        spawnedLevelElements.AddRange(newSpawnedElements);
        newSpawnedElements.Clear();

        totalCount++;
        currentMapgroupCounter++;

        if (isEnd)
        {
            isEnd = false;

            if (activeMapGroup.nextMapGroups.Count > 0)
                SetMapGroup(activeMapGroup.nextMapGroups.GetRandomElement());
            else
                SetMapGroup(activeMapGroup);
        }
    }

    ElementRefs? HandleNew(MapGroupElement lastElement) 
    {
        ElementData elData = activeMapGroup.GetElement(currentMapgroupCounter, lastElement);
        isEnd = elData.isEnd;
        return CreateElement(elData.mapGroupElement); 
    }


    public void MoveOverElement(PlayerRig optionalRig = null)
    {
        if (activeMapGroup == null) return;

        if (_activeElement == null) { Debug.Log("Active element null!"); return; }
        LevelElement levelElement = _activeElement.Value.spawnedElement.baseElement;
        if (levelElement.TakenLevelPoint == null)
        {
            Debug.Log("Chose wrong side, probably should die!"); _activeElement.Value.spawnedElement.baseElement.ChoseSide(MapSides.Right); return;
        }

        _elementAmount--;

        Transform newActiveElementTrans = levelElement.TakenLevelPoint.transform.GetChild(0);
        if (newActiveElementTrans == null) { Debug.Log("Null!"); return; }
        newActiveElementTrans.parent = null;
        _activeElement.Value.spawnedElement.transform.parent = newActiveElementTrans;
        DestroyRecursive(_activeElement.Value.spawnedElement);

        foreach (ElementRefs elle in allSpawnedElements)
            if (elle.spawnedElement == newActiveElementTrans.GetComponent<MapGroupElement>())
                _activeElement = elle;

        foreach (ElementRefs elle in spawnedLevelElements)
            if (elle.spawnedElement == newActiveElementTrans.GetComponent<MapGroupElement>())
                _activeElement = elle;

        foreach (ElementRefs elle in newSpawnedElements)
            if (elle.spawnedElement == newActiveElementTrans.GetComponent<MapGroupElement>())
                _activeElement = elle;

        if (_activeElement == null) return;

        _activeElement.Value.spawnedElement.transform.position = transform.position;
        _activeElement.Value.spawnedElement.transform.parent = transform;
        _activeElement.Value.spawnedElement.gameObject.SetActive(true);

        foreach (LevelPoint pp in _activeElement.Value.spawnedElement.baseElement.EndPoints)
        {
            Transform t = pp.transform.GetChild(0);
            MapGroupElement mapGroupElement = t.GetComponent<MapGroupElement>();
            if (mapGroupElement == null) continue;
            mapGroupElement.gameObject.SetActive(true);
            /*
            foreach(LevelPoint ppp in mapGroupElement.baseElement.EndPoints)
            {
                Transform tt = ppp.transform.GetChild(0);
                MapGroupElement mapGroupElement2 = tt.GetComponent<MapGroupElement>();
                if (mapGroupElement2 == null) continue;
                mapGroupElement2.gameObject.SetActive(false);
            }*/
        }

        if(_activeElement.Value.spawnedElement.isEra != lastEra)
        {
            lastEra = _activeElement.Value.spawnedElement.isEra;
            SkyboxSetter.Instance?.SetSkybox(lastEra, false);
        }

        if (optionalRig == null) return;


       // optionalRig.transform.rotation = _activeElement.Value.spawnedElement.transform.rotation;
        optionalRig.transform.position = _activeElement.Value.spawnedElement.baseElement.StartPoint.position;

        optionalRig.cameraRig.GetComponent<CameraFollowPoint>()?.Teleport();
      
    }

    void DestroyRecursive(MapGroupElement element)
    {
        if (element == null) return;
        LevelElement levelElement = element.baseElement;
        if(levelElement == null) return;
        foreach (LevelPoint point in levelElement.EndPoints)
        {
            if (levelElement.TakenLevelPoint == point) continue;
            if (point.transform.childCount <= 0) continue;
            Transform pTrans = point.transform.GetChild(0);
            if (pTrans == null) continue;
            MapGroupElement levelEl = pTrans.GetComponent<MapGroupElement>();
            DestroyRecursive(levelEl);
        }
        for (int i = allSpawnedElements.Count - 1; i >= 0; i--)
        {
            if(allSpawnedElements[i].spawnedElement == element || allSpawnedElements[i].spawnedElement == null)
                allSpawnedElements.RemoveAt(i);
        }
        for(int i = spawnedLevelElements.Count - 1; i >= 0; i--)
        {
            if (spawnedLevelElements[i].spawnedElement == null || spawnedLevelElements[i].spawnedElement == element)
                spawnedLevelElements.RemoveAt(i);
        }
        Destroy(element.gameObject, 0.5f);
        Destroy(element);
    }


    ElementRefs? CreateElement(MapGroupElement overrideElement)
    {
        if (overrideElement == null) return null;
        if (!overrideElement.ValidForEra(buildForEra)) return null;
        ElementRefs reffie = new ElementRefs(ref overrideElement, Instantiate(overrideElement));
        newSpawnedElements.Add(reffie);
        if (_activeElement == null) _activeElement = reffie;
        reffie.spawnedElement.Display(buildForEra, _elementAmount > 2);
        return reffie;
    }

    public void SetMapGroup(MapGroup mapGroup)
    {
        if (mapGroup == null) return;
        //if (activeMapGroup == mapGroup) return;
        activeMapGroup = mapGroup;
        mapGroup.DeclareRange();
        currentMapgroupCounter = 0;
    }

    public void SetEra(int era)
    {
        buildForEra = (Era)era;
    }

    ElementRefs? HandleOld() => CreateElement(startGroup.startElements.GetRandomElement());
}

[System.Serializable]
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
