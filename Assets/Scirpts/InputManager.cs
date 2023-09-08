using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    [SerializeField]
    private LayerMask placementLayermask;        
    
    private Vector3 lastPosition;

    public event Action OnClicked, OnExit;
           
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();                
        if(Input.GetKeyDown(KeyCode.Space))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();//��鵱ǰ��ָ���Ƿ�������ͣ����UIԪ�ء�����ǣ��򷵻�true�����򷵻�false��

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

}
