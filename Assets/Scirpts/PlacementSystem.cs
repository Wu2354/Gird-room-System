using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject mouseIndicator, cellIndicator;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO database;
    [SerializeField] private GameObject girdVisualization;
    [SerializeField] private AudioSource audioSource;
    private int selectedObjectIndex = -1;
    private GridData floorData, furnitureData;
    private Renderer previewRenderer;
    private List<GameObject> placeedObjects = new();

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        //鼠标位置控制格子坐标的位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);//世界坐标中鼠标坐标变成网格坐标

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);//使标识格子物体在世界坐标显示
    }

    public void StartPlacement(int ID)//作为按钮的触发事件函数
    {        
        selectedObjectIndex = database.objectsDate.FindIndex(data => data.ID == ID);//返回ID相同物体在objectsDate的位置，未找到返回-1
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"没有{ID}");
        }
        girdVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    //放置物体并保存占据的格子信息在
    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        //使家具在网格上生成
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition,selectedObjectIndex);
        if (placementValidity == false)
            return;
        audioSource.Play();
        
        GameObject newObject = Instantiate(database.objectsDate[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        placeedObjects.Add( newObject );

        GridData selectedData = database.objectsDate[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        //保存在列表最后
        selectedData.AddObjectAt(gridPosition, database.objectsDate[selectedObjectIndex].Size,
            database.objectsDate[selectedObjectIndex].ID,
            placeedObjects.Count - 1);
    }

    //检查能否放置
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsDate[selectedObjectIndex].ID == 0 ? floorData : furnitureData;//判断是否是地毯（selectedObjectIndex为0）
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsDate[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        girdVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }
    
}
