using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPosition);
    void UpdataState(Vector3Int gridPosition);
}