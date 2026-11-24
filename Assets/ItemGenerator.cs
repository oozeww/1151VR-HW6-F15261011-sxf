using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [Header("掉落物 Prefab")]
    public GameObject applePrefab;
    public GameObject bombPrefab;

    [Header("关卡难度参数（由 GameDirector 自动控制）")]
    public float spawnInterval = 1.0f;   // 产生间隔（秒）
    public float fallSpeed = 3.0f;       // 掉落速度
    [Range(0f, 1f)]
    public float bombRatio = 0.25f;      // 炸弹生成比例（0~1）

    [Header("3x3 九宫格位置设置")]
    public float gridOffset = 0.63f;     // 九宫格相邻格子的间距
    public float spawnHeight = 3.5f;     // 掉落物生成的 Y 轴高度

    private float timer = 0f;
    private bool isGenerating = true;

    void Update()
    {
        if (!isGenerating) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        if (applePrefab == null || bombPrefab == null) return;

        // 1. 按当前关卡的炸弹比例决定生成炸弹还是苹果
        GameObject prefabToSpawn = (Random.value < bombRatio) ? bombPrefab : applePrefab;

        // 2. 随机抽取 3x3 九宫格坐标 (-0.63f, 0f, +0.63f)
        int gridX = Random.Range(-1, 2); // 随机得到 -1, 0, 1
        int gridZ = Random.Range(-1, 2); // 随机得到 -1, 0, 1
        Vector3 spawnPos = new Vector3(gridX * gridOffset, spawnHeight, gridZ * gridOffset);

        // 3. 生成物体（保留 Prefab 原本的旋转角度），并把当前关卡速度传给它
        GameObject item = Instantiate(prefabToSpawn, spawnPos, prefabToSpawn.transform.rotation);
        item.SendMessage("SetSpeed", fallSpeed, SendMessageOptions.DontRequireReceiver);
    }

    public void StartGenerating()
    {
        isGenerating = true;
        timer = 0f;
    }

    public void StopGenerating()
    {
        isGenerating = false;
    }
}