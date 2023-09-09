using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData 
{
    //存储了哪些网格被哪些对象占用
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    /*
     * 在指定的网格位置摆放一个对象。它首先计算对象将占用的所有网格位置，然后创建一个PlacementData对象来存储这些信息。
     * 接着，它会将每个占用的位置添加到placedObjects字典中。如果一个位置已经被另一个对象占用，该方法会抛出一个异常。
     */
    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        //保存已经占据的位置数据
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy , ID , placedObjectIndex);
        
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"字典里已经包含这个物体的位置在：{pos}");
            }
            placedObjects[pos] = data;
        }
    }

    //计算并返回一个对象将会占用的所有网格位置，基于给定的起始位置和对象大小
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions( gridPosition,  objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }            
        }
        return true;
    }

}
    
public class PlacementData 
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}