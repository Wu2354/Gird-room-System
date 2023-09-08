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

    

    private void Start()
    {
        StopPlacement();
    }
    public void StartPlacement(int ID)//��Ϊ��ť�Ĵ����¼�����
    {
        //StopPlacement();
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

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        //ʹ�Ҿ�������������
        audioSource.Play();
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        GameObject newObject = Instantiate(database.objectsDate[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        girdVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }
    void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        //���λ�ÿ��Ƹ��������λ��
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);//���������������������������
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);//ʹ��ʶ��������������������ʾ
    }
}
