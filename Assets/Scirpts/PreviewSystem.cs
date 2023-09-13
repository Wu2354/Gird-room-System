using UnityEngine;
public class PreviewSystem : MonoBehaviour
{
    //��y���ϵ�ƫ����
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;
    
    private void Start()
    {
        //����������һ���µĲ���ʵ�������������(��ֹ��ͬһ����������������仯����������)
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    //�Բ�ͬԤ����Ͳ�ͬ�ߴ��������չʾ
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        //֮ǰ��previewObject������
        if (previewObject != null)
        {
            StopShowingPreview();
        }
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size); 
        cellIndicator.SetActive(true);
    }
    public void StopShowingPreview()
    {
        cellIndicator.SetActive (false);
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        
    }
    //Ԥ�����彫Ҫ�ڷŵ�λ��
    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            //��������������������Ĵ���������2,2���ͻ�����x��z���Ϸֱ�2���������൱�ڡ�Tilling��
            cellIndicator.GetComponentInChildren<Renderer>().material.mainTextureScale = size;

        }
    }

    //Ԥ���������״�����ʣ�͸��shader�����滻��
    private void PreparePreview(GameObject previewObject)
    {
        //���������в�ͬ�Ĳ������
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            //һ����λ�ж������
            Material[] materials = renderer.materials;
            for (int i=0; i<materials.Length ; i++)
            {
                materials[i] = previewMaterialInstance;//ȫ���滻��͸������
            }
            renderer.materials = materials;
        }
    }

    

    //Ԥ�������������ƶ����жϿɷ�λ��
    public void UpdatePosition( Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;

    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x, 
            position.y+ previewYOffset,
            position.z);
    }

    internal void StartShowingRemovePreview()
    {
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
