using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMapBuilder : MonoBehaviour
{
    [SerializeField] MapGroup startGroup;

    MapGroup activeMapGroup;

    int currentMapgroupCounter;

    private void Awake()
    {
        activeMapGroup = startGroup;
    }

    public void BuildLevelElement()
    {

    }
}
