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
        //���λ�ÿ��Ƹ��������λ��
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);//���������������������������
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdataState(gridPosition);
            lastDetectedPosition = gridPosition;
        }        
    }

    public void StartPlacement(int ID)//��Ϊ��ť�Ĵ����¼�����
    {
        StopPlacement();
        girdVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;//��ť�������
        inputManager.OnExit += StopPlacement;//��ť���ֹͣ
        inputManager.OnRotate += RotatePlacement;
    }

    private void RotatePlacement()
    {
        // ��ȡ��ǰ��BuildingState
        PlacementState placementState = (PlacementState)buildingState;
        // ��תԤ��
        previewSystem.RotatePreview();
        // ���³ߴ�
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

    //�������岢����ռ�ݵĸ�����Ϣ��GridData��
    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        //ʹ�Ҿ�������������
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    //����ܷ����
    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;//�ж��Ƿ��ǵ�̺��selectedObjectIndexΪ0��
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
