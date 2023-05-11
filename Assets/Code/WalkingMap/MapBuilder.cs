using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] LevelElement[] levelElements;

    List<LevelElement> allSpawnedElements = new List<LevelElement>();
    List<LevelElement> spawnedLevelElements = new List<LevelElement>();
    List<LevelElement> newSpawnedElements = new List<LevelElement>();

    LevelElement activeElement = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) BuildElement();
        if (Input.GetKeyDown(KeyCode.Tab)) MoveOverElement();
    }

    void BuildElement()
    {
        if (spawnedLevelElements.Count == 0)
        {
            CreateLevelElement();
        }
        else
        {
            foreach (LevelElement spawnedElement in spawnedLevelElements)
                foreach (LevelPoint point in spawnedElement.EndPoints)
                {
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
    }

    LevelElement CreateLevelElement()
    {
        LevelElement el;
        newSpawnedElements.Add(el = Instantiate(levelElements[Random.Range(0, levelElements.Length)]));
        if (activeElement == null) activeElement = el;
        return el;
    }


    void MoveOverElement()
    {
        if (activeElement == null) return;
        //TODO: Remove this and change it with proper side chosing later
        if (activeElement.TakenLevelPoint == null) activeElement.ChoseSide(Random.Range(0, 2) == 1 ? MapSides.Left : MapSides.Right);

        if (activeElement.TakenLevelPoint == null) { Debug.Log("Chose wrong side, probably should die!"); return; }

        //TODO: When destroying an object with another path, uhh, walk down the other path and manually remove all the old stuffs as well. I think this causes the null errors and stuff
        Transform newActiveElementTrans = activeElement.TakenLevelPoint.transform.GetChild(0).transform;
        newActiveElementTrans.parent = null;
        Destroy(activeElement.gameObject);
        allSpawnedElements.Remove(activeElement);
        activeElement = newActiveElementTrans.GetComponent<LevelElement>();
        activeElement.transform.position = transform.position;
        activeElement.transform.rotation = transform.rotation;
        activeElement.transform.parent = transform;
    }
}
