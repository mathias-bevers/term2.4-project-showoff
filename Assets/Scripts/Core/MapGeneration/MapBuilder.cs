using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    int _elementAmount = 0;
    public int elementAmount => _elementAmount;

    [SerializeField] LevelElement startElement;
    [SerializeField] LevelElement[] levelElements;

    List<LevelElement> allSpawnedElements = new List<LevelElement>();
    List<LevelElement> spawnedLevelElements = new List<LevelElement>();
    List<LevelElement> newSpawnedElements = new List<LevelElement>();

    LevelElement _activeElement = null;
    public LevelElement activeElement => _activeElement;

    int totalCount = 0;

    public virtual void BuildElement()
    {
        _elementAmount++;

        if (spawnedLevelElements.Count == 0)
        {
            CreateLevelElement();
        }
        else
        {
            foreach (LevelElement spawnedElement in spawnedLevelElements)
                foreach (LevelPoint point in spawnedElement.EndPoints)
                {
                    if (point == null) continue;
                    LevelElement el = CreateLevelElement();
                    el.transform.position = point.position;
                    el.transform.rotation = point.transform.rotation;
                    el.transform.parent = point.transform;
                }

        }

        allSpawnedElements.AddRange(spawnedLevelElements);
        spawnedLevelElements.Clear();
        spawnedLevelElements.AddRange(newSpawnedElements);
        newSpawnedElements.Clear();
        totalCount++;
    }

    LevelElement CreateLevelElement()
    {
        LevelElement el;
        if(totalCount <= 3)
        {
            newSpawnedElements.Add(el = Instantiate(startElement));
        }
        else
        {
            newSpawnedElements.Add(el = Instantiate(levelElements[Random.Range(0, levelElements.Length)]));
        }
       
        if (_activeElement == null) _activeElement = el;
        return el;
    }


    public void MoveOverElement(PlayerRig optionalRig = null)
    {
        if (_activeElement == null) { Debug.Log("Active element null!"); return; }
        if (_activeElement.TakenLevelPoint == null) { Debug.Log("Chose wrong side, probably should die!"); return; }

        _elementAmount--;
        Transform newActiveElementTrans = _activeElement.TakenLevelPoint.transform.GetChild(0);
        if(newActiveElementTrans == null) { Debug.Log("Null!"); return; }
        newActiveElementTrans.parent = null;
        _activeElement.transform.parent = newActiveElementTrans;
        DestroyRecursive(_activeElement);

        _activeElement = newActiveElementTrans.GetComponent<LevelElement>();
        _activeElement.transform.position = transform.position;
        _activeElement.transform.parent = transform;

        if (optionalRig == null) return;

        optionalRig.transform.position = _activeElement.StartPoint.position;

    }

    void DestroyRecursive(LevelElement element)
    {
        if (element == null) return;
        foreach (LevelPoint point in element.EndPoints)
        {
            if (element.TakenLevelPoint == point) continue;
            if (point.transform.childCount <= 0) continue;
            Transform pTrans = point.transform.GetChild(0);
            if (pTrans == null) continue;
            LevelElement levelEl = pTrans.GetComponent<LevelElement>();
            DestroyRecursive(levelEl);
        }
        allSpawnedElements.Remove(element);
        Destroy(element.gameObject, 0.5f);
    }
}
