using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewMapBuilder))]
public class DebugNewMapBuilder : MonoBehaviour
{
    NewMapBuilder builder;
    private void Start()
    {
        builder =GetComponent<NewMapBuilder>(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            builder.BuildLevelElement();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            builder.MoveOverElement();
        }
    }
}
