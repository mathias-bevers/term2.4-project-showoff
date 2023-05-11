using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] LevelElement[] levelElements;

    List<LevelElement> allSpawnedElements = new List<LevelElement>();
    List<LevelElement> spawnedLevelElements = new List<LevelElement>();
    List<LevelElement> newSpawnedElements = new List<LevelElement>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) BuildElement();
        if (Input.GetKeyDown(KeyCode.Tab)) MoveOverElement();
    }

    void BuildElement()
    {
        if(spawnedLevelElements.Count == 0)
        {
            CreateLevelElement();
        }
        else
        {
            foreach(LevelElement spawnedElement in spawnedLevelElements)
                foreach(LevelPoint point in spawnedElement.EndPoints)
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
        return el;
    }


    void MoveOverElement()
    {

    }
}
