using UnityEngine;
public class PreviewSystem : MonoBehaviour
{
    //在y轴上的偏移量
    [SerializeField]
    private float previewYOffset = 0.06f;
    //旋转的角度
    private float rotateAngle = 90f;
    private Vector2Int rotateSize;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;
   

    private void Start()
    {
        //创建并复制一个新的材质实例，定制其外观(防止用同一材质引起所有物体变化和消耗性能)
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    //对不同预制体和不同尺寸物体进行展示
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        
        //之前有previewObject，销毁
        if (previewObject != null)
        {
            StopShowingPreview();
        }
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size); 
        cellIndicator.SetActive(true);
        rotateSize = size;//方便后面旋转改变Size
    }

    public void RotatePreview()
    {
        if(previewObject != null)
        {
            previewObject.transform.Rotate(0, rotateAngle, 0);
            // 旋转尺寸
            rotateSize = new Vector2Int(rotateSize.y, rotateSize.x);
            //格子进行旋转匹配
            cellIndicator.transform.Rotate(0, rotateAngle, 0);
            // 预览格子占据位置
            PrepareCursor(rotateSize);
        }
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true );
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive (false);
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        
    }

    public Quaternion GetCurrentRotation()
    {
        return previewObject.transform.rotation;
    }

    //预览物体将要摆放的位置
    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            //控制纹理在物体上铺设的次数（例（2,2）就会铺设x和z轴上分别2个纹理），相当于“Tilling”
            cellIndicator.GetComponentInChildren<Renderer>().material.mainTextureScale = size;

        }
    }

    //预览物体的形状，材质（透明shader材质替换）
    private void PreparePreview(GameObject previewObject)
    {
        //物体下面有不同的部分组成
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            //一个部位有多个材质
            Material[] materials = renderer.materials;
            for (int i=0; i<materials.Length ; i++)
            {
                materials[i] = previewMaterialInstance;//全部替换成透明材质
            }
            renderer.materials = materials;
        }
    }
    
    //预览物体跟随鼠标移动并判断可放位置
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

    //根据状态改变对应格子的颜色
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    //根据状态改变物体的颜色
    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;

    }
    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }
    
    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x, 
            position.y+ previewYOffset,
            position.z);
    }

    //判断是否旋转
    public bool IsPreviewObjectRotated()
    {
        return Mathf.Abs(previewObject.transform.eulerAngles.y - rotateAngle) < 0.1f ||
               Mathf.Abs(previewObject.transform.eulerAngles.y - 3 * rotateAngle) < 0.1f; // 对于90度旋转
    }



}
