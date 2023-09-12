using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�򻯱�Ľű���������
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                         Grid grid,
                         PreviewSystem previewSystem,
                         ObjectsDatabaseSO database,
                         GridData floorData,
                         GridData furnitureData,
                         ObjectPlacer objectPlacer
                         )
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);//����ID��ͬ������objectsDate��λ�ã�δ�ҵ�����-1
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
               database.objectsData[selectedObjectIndex].Prefab,
               database.objectsData[selectedObjectIndex].Size);
        }
        else
            throw new System.Exception($"û����ƥ�䣺{iD}");
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition));

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;//�ж��Ƿ��ǵ�̺��selectedObjectIndexΪ0��
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
    public void UpdataState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}