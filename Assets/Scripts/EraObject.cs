using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraObject : MonoBehaviour
{
    [SerializeField] bool chanceToSpawnNothing = false;
    [Range(1, 100)]
    [SerializeField] int noSpawnChance = 25;
    [SerializeField] List<EraList> spawnableObjects = new List<EraList>();

    public void Display(Era era)
    {
        if (chanceToSpawnNothing && Random.Range(0, 100) <= noSpawnChance) return;
        List<SpawnableEraElement> spawnableEraElements = new List<SpawnableEraElement>();

        foreach(EraList eraList in spawnableObjects)
            if (eraList.forEra == era) 
                spawnableEraElements.AddRange(eraList.spawnableObjects);

        if (spawnableEraElements.Count == 0) return;

        SpawnableEraElement spawnableElement = spawnableEraElements.GetRandomElementStruct();

        GameObject spanwnedElement = Instantiate(spawnableElement.gameObject, transform);
        spanwnedElement.transform.localPosition = spawnableElement.transformData.position;
        spanwnedElement.transform.localScale = spawnableElement.transformData.scale;
        spanwnedElement.transform.localRotation = Quaternion.Euler(spawnableElement.transformData.eulerAngles);
    }
}

[System.Serializable]
public struct EraList
{
    public Era forEra;
    public List<SpawnableEraElement> spawnableObjects;
}

[System.Serializable]
public struct SpawnableEraElement
{
    public GameObject gameObject;
    public TransformData transformData;
}

[System.Serializable]
public struct TransformData
{
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 scale;
}
