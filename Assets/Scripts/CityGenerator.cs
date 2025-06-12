using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] buildingPrefabs;
    public GameObject roadStraightPrefab;    // 普通直路
    public GameObject roadCrossPrefab;       // 十字路口
    public GameObject blockFloorPrefab;      // 街区地板

    [Header("City Size")]
    public int cityWidth = 3;
    public int cityHeight = 3;

    [Header("Block Settings")]
    public float buildingSize = 1f;          // 每个建筑格的单位大小
    public float blockSpacing = 40f;         // 街区中心到中心距离

    [Header("Advanced Settings")]
    public float blockMargin = 4.5f;         // 街区四周预留空地（留出人行道）

    [Header("Decoration Prefabs")]
    public GameObject[] sidewalkProps;   // 道路物品
    public int propsPerBlock = 32;        // 每个街区放几个

    [Header("Lighting")]
    public Material skyboxMaterial;
    public void GenerateWithSeed(int seed)
    {
        Random.InitState(seed);
        ClearOldCity();
        GenerateCity();
        SetupLighting();
        CreateStartPositionAtCrossroad();
        AssignStartPosToPlayerInit();
        AddCollidersToChildren();
        FixAllBoxColliders();
    }

    void ClearOldCity()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void GenerateCity()
    {
        // 1. 生成街区 + 地板
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityHeight; z++)
            {
                Vector3 blockCenter = new Vector3(x * (blockSpacing + blockMargin), 0, z * (blockSpacing + blockMargin));

                // 地板：完整铺满一个 4x4 街区大小
                if (blockFloorPrefab != null)
                {
                    Vector3 floorPos = blockCenter;
                    GameObject floor = Instantiate(blockFloorPrefab, floorPos, Quaternion.identity, transform);
                    if (floor.GetComponent<Collider>() == null)
                    {
                        floor.AddComponent<BoxCollider>();
                    }

                    float blockWorldSize = buildingSize - blockMargin / 3f;
                    floor.transform.localScale = new Vector3(blockWorldSize, 1, blockWorldSize);
                }

                // 建筑群
                GenerateBlock(blockCenter);

                // 横向道路（右侧）
                if (x < cityWidth - 1 && roadStraightPrefab != null)
                {
                    Vector3 roadPos = blockCenter + new Vector3(blockSpacing / 2f + blockMargin / 2f, 0, 0);
                    GameObject road1 = Instantiate(roadStraightPrefab, roadPos, Quaternion.identity, transform);
                    if (road1.GetComponent<Collider>() == null)
                    {
                        road1.AddComponent<BoxCollider>();
                    }
                    road1.transform.localScale = new Vector3(1, 1, 1.25f);
                }

                // 纵向道路（上方）
                if (z < cityHeight - 1 && roadStraightPrefab != null)
                {
                    Vector3 roadPos = blockCenter + new Vector3(0, 0, (blockSpacing + blockMargin) / 2f);
                    Quaternion rot = Quaternion.Euler(0, 90, 0);
                    GameObject road2 = Instantiate(roadStraightPrefab, roadPos, rot, transform);
                    if (road2.GetComponent<Collider>() == null)
                    {
                        road2.AddComponent<BoxCollider>();
                    }
                    road2.transform.localScale = new Vector3(1, 1, 1.25f);
                }

                // 摆放物品
                PlaceSidewalkProps(blockCenter);

            }
        }

        // 2. 十字路口（街区之间）
        for (int x = 0; x < cityWidth - 1; x++)
        {
            for (int z = 0; z < cityHeight - 1; z++)
            {
                Vector3 crossPos = new Vector3(x * (blockSpacing + blockMargin) + (blockSpacing + blockMargin) / 2f, 0, z * (blockSpacing + blockMargin) + (blockSpacing + blockMargin) / 2f);
                if (roadCrossPrefab != null)
                {
                    GameObject road3 = Instantiate(roadCrossPrefab, crossPos, Quaternion.identity, transform);
                    if (road3.GetComponent<Collider>() == null)
                    {
                        road3.AddComponent<BoxCollider>();
                    }
                }
            }
        }
    }

    void GenerateBlock(Vector3 center)
    {
        float startX = center.x - buildingSize * 1.5f;
        float startZ = center.z - buildingSize * 1.5f;

        for (int i = 0; i < 4; i++) // 横向格子
        {
            for (int j = 0; j < 4; j++) // 纵向格子
            {
                if (i == 0 || i == 3 || j == 0 || j == 3) // 只放边缘
                {
                    Vector3 pos = new Vector3(startX + i * buildingSize, 0, startZ + j * buildingSize);
                    GameObject building = GetRandomBuilding();
                    if (building.GetComponent<Collider>() == null)
                    {
                        building.AddComponent<BoxCollider>();
                    }
                    if (building != null)
                    {
                        Quaternion rot = GetFacingRotation(i, j);
                        Instantiate(building, pos, rot, transform);
                    }
                }
            }
        }
    }

    void PlaceSidewalkProps(Vector3 center)
    {
        if (sidewalkProps == null || sidewalkProps.Length == 0) return;

        float halfSize = buildingSize * 2f + blockMargin / 2f;
        float edgeOffset = blockMargin * 0.3f;  // 靠近边但不贴太紧
        int propsPerEdge = 8; // 每边放几个

        float minX = center.x - halfSize;
        float maxX = center.x + halfSize;
        float minZ = center.z - halfSize;
        float maxZ = center.z + halfSize;

        // 上边（正Z）
        for (int i = 0; i < propsPerEdge; i++)
        {
            float baseT = (i + 1f) / (propsPerEdge + 1f);
            float offsetT = (0.35f / (propsPerEdge + 1f)) * Random.Range(-1f, 1f); // 随机偏移比例
            float t = Mathf.Clamp01(baseT + offsetT);
            float x = Mathf.Lerp(minX, maxX, t);
            float z = maxZ - edgeOffset;
            PlacePropAt(new Vector3(x, 0, z));
        }

        // 下边（负Z）
        for (int i = 0; i < propsPerEdge; i++)
        {
            float baseT = (i + 1f) / (propsPerEdge + 1f);
            float offsetT = (0.5f / (propsPerEdge + 1f)) * Random.Range(-1f, 1f); // 随机偏移比例
            float t = Mathf.Clamp01(baseT + offsetT);
            float x = Mathf.Lerp(minX, maxX, t);
            float z = minZ + edgeOffset;
            PlacePropAt(new Vector3(x, 0, z));
        }

        // 左边（负X）
        for (int i = 0; i < propsPerEdge; i++)
        {
            float baseT = (i + 1f) / (propsPerEdge + 1f);
            float offsetT = (0.5f / (propsPerEdge + 1f)) * Random.Range(-1f, 1f); // 随机偏移比例
            float t = Mathf.Clamp01(baseT + offsetT);
            float z = Mathf.Lerp(minZ, maxZ, t);
            float x = minX + edgeOffset;
            PlacePropAt(new Vector3(x, 0, z));
        }

        // 右边（正X）
        for (int i = 0; i < propsPerEdge; i++)
        {
            float baseT = (i + 1f) / (propsPerEdge + 1f);
            float offsetT = (0.5f / (propsPerEdge + 1f)) * Random.Range(-1f, 1f); // 随机偏移比例
            float t = Mathf.Clamp01(baseT + offsetT);
            float z = Mathf.Lerp(minZ, maxZ, t);
            float x = maxX - edgeOffset;
            PlacePropAt(new Vector3(x, 0, z));
        }
    }

    void PlacePropAt(Vector3 position)
    {
        GameObject prefab = sidewalkProps[Random.Range(0, sidewalkProps.Length)];
        Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0f); // 可随机旋转
        GameObject prop = Instantiate(prefab, position, rot, transform);
        if (prop.GetComponent<Collider>() == null)
        {
            prop.AddComponent<BoxCollider>();
        }
    }


    bool IsNearEdge(float x, float z, float minX, float maxX, float minZ, float maxZ, float threshold)
    {
        return Mathf.Abs(x - minX) < threshold || Mathf.Abs(x - maxX) < threshold ||
               Mathf.Abs(z - minZ) < threshold || Mathf.Abs(z - maxZ) < threshold;
    }

    Quaternion GetFacingRotation(int i, int j)
    {
        // 拐角优先判断
        if (i == 0 && j == 0) return Quaternion.Euler(0, -90, 0);   // 左下
        if (i == 0 && j == 3) return Quaternion.Euler(0, 0, 0);     // 左上
        if (i == 3 && j == 0) return Quaternion.Euler(0, -180, 0);  // 右下
        if (i == 3 && j == 3) return Quaternion.Euler(0, 90, 0);    // 右上

        if (j == 0) return Quaternion.Euler(0, -90, 0);             // 下边
        if (j == 3) return Quaternion.Euler(0, 90, 0);              // 上边
        if (i == 0) return Quaternion.Euler(0, 0, 0);               // 左边
        if (i == 3) return Quaternion.Euler(0, -180, 0);            // 右边

        return Quaternion.identity;
    }

    GameObject GetRandomBuilding()
    {
        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
        {
            Debug.LogWarning("No building prefabs assigned!");
            return null;
        }
        return buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
    }

    void SetupLighting()
    {
        ApplySkybox();
    }

    void ApplySkybox()
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment(); // 更新全局环境光照
        }
        else
        {
            Debug.LogWarning("Skybox 材质未设置！");
        }
    }

    void CreateStartPositionAtCrossroad()
    {
        // 找中间的十字路口
        int midX = cityWidth / 2;
        int midZ = cityHeight / 2;

        float spacing = blockSpacing + blockMargin;

        Vector3 centerPos = new Vector3(midX * spacing - spacing / 2f, 0f, midZ * spacing - spacing / 2f);

        GameObject startPos = new GameObject("StartPos");
        startPos.transform.position = centerPos + Vector3.up * 0.5f; // 稍微抬高一点点
    }

    void AssignStartPosToPlayerInit()
    {
        GameObject startObj = GameObject.Find("StartPos");
        GameInitPlayer playerInit = GameObject.Find("Game")?.GetComponent<GameInitPlayer>();

        if (startObj != null && playerInit != null)
        {
            playerInit.startPos = startObj.transform;
            Debug.Log("✅ StartPos 已成功绑定到 GameInitPlayer");
        }
        else
        {
            Debug.LogWarning("❗无法绑定 StartPos，请检查 StartPos 或 GameInitPlayer 是否存在");
        }
    }

    void AddCollidersToChildren()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Collider>() == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }
        }
    }

    void FixAllBoxColliders()
{
    foreach (var box in GetComponentsInChildren<BoxCollider>())
    {
        Renderer r = box.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            box.center = box.transform.InverseTransformPoint(r.bounds.center);
            box.size = r.bounds.size;
        }
    }
}

}
