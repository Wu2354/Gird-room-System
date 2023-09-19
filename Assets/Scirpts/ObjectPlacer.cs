using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placeedObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position,Quaternion quaternion)
    {
        GameObject newObject = Instantiate(prefab, position, quaternion);
        //newObject.transform.position = position;
        placeedObjects.Add(newObject);
        return placeedObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placeedObjects.Count <= gameObjectIndex || placeedObjects[gameObjectIndex] == null)
            return;
        Destroy(placeedObjects[gameObjectIndex]);
        placeedObjects[gameObjectIndex] = null;
    }    
}
