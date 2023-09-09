using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData 
{
    //�洢����Щ������Щ����ռ��
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    /*
     * ��ָ��������λ�ðڷ�һ�����������ȼ������ռ�õ���������λ�ã�Ȼ�󴴽�һ��PlacementData�������洢��Щ��Ϣ��
     * ���ţ����Ὣÿ��ռ�õ�λ�����ӵ�placedObjects�ֵ��С����һ��λ���Ѿ�����һ������ռ�ã��÷������׳�һ���쳣��
     */
    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        //�����Ѿ�ռ�ݵ�λ������
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy , ID , placedObjectIndex);
        
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"�ֵ����Ѿ�������������λ���ڣ�{pos}");
            }
            placedObjects[pos] = data;
        }
    }

    //���㲢����һ�����󽫻�ռ�õ���������λ�ã����ڸ�������ʼλ�úͶ����С
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