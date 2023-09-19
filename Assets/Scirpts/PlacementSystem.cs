using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;    
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO database;
    [SerializeField] private GameObject girdVisualization;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;
    
    private GridData floorData, furnitureData;    
    
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    bool isRemoving;

    IBuildingState buildingState;

    private void Start()
    {
        StopPlacement();
        floorData = new(previewSystem);
        furnitureData = new(previewSystem);      
    }

    void Update()
    {
        if (buildingState == null)
            return;
        //鼠标位置控制格子坐标的位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);//世界坐标中鼠标坐标变成网格坐标
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdataState(gridPosition);
            lastDetectedPosition = gridPosition;
        }        
    }

    public void StartPlacement(int ID)//作为按钮的触发事件函数
    {
        StopPlacement();
        girdVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;//按钮点击放置
        inputManager.OnExit += StopPlacement;//按钮点击停止
        inputManager.OnRotate += RotatePlacement;
    }

    private void RotatePlacement()
    {
        // 获取当前的BuildingState
        PlacementState placementState = (PlacementState)buildingState;
        // 旋转预览
        previewSystem.RotatePreview();
        // 更新尺寸
        placementState.RotateCurrentObjectSize();   
    }

    public void StartRemoving()
    {
        StopPlacement();
        girdVisualization.SetActive(true);        
        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;   
    }

    //放置物体并保存占据的格子信息在GridData中
    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        //使家具在网格上生成
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    //检查能否放置
    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;//判断是否是地毯（selectedObjectIndex为0）
    //    return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    private void StopPlacement()
    {
        if (buildingState == null) return;
        girdVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }
    
}
