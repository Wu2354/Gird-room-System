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
        //���λ�ÿ��Ƹ��������λ��
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);//���������������������������

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);//ʹ��ʶ��������������������ʾ
    }

    public void StartPlacement(int ID)//��Ϊ��ť�Ĵ����¼�����
    {        
        selectedObjectIndex = database.objectsDate.FindIndex(data => data.ID == ID);//����ID��ͬ������objectsDate��λ�ã�δ�ҵ�����-1
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"û��{ID}");
        }
        girdVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    //�������岢����ռ�ݵĸ�����Ϣ��
    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        //ʹ�Ҿ�������������
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
        //�������б����
        selectedData.AddObjectAt(gridPosition, database.objectsDate[selectedObjectIndex].Size,
            database.objectsDate[selectedObjectIndex].ID,
            placeedObjects.Count - 1);
    }

    //����ܷ����
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsDate[selectedObjectIndex].ID == 0 ? floorData : furnitureData;//�ж��Ƿ��ǵ�̺��selectedObjectIndexΪ0��
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
