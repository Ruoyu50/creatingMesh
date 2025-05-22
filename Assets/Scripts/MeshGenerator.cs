using UnityEngine;
using Uduino;

public class MeshGenerator : MonoBehaviour
{
    public GameObject circlePrefab, spherePrefab, squarePrefab, blockPrefab; // 预制体
    public Transform spawnPoint;
    public float spawnInterval = 1.0f; // 初始生成间隔
    public Vector3 circleRotation, squareRotation, blockRotation; // 各类型旋转角度

    private float nextSpawnTime;
    private int lastDistance = 0; // 初始化为0，防止一开始为-1不能生成
    private int directionMultiplier = 1; // 反转方向用
    private GameObject currentPrefab; // 当前预制体
    private Vector3 lastSpawnPosition; // 上一次生成的位置

    void Start()
    {
        UduinoManager.Instance.OnDataReceived += OnDataReceived;
        currentPrefab = circlePrefab;
        lastSpawnPosition = spawnPoint.position;
    }

    void Update()
    {
        // 切换形状
        if (Input.GetKeyDown(KeyCode.C)) currentPrefab = circlePrefab;
        if (Input.GetKeyDown(KeyCode.G)) currentPrefab = spherePrefab;
        if (Input.GetKeyDown(KeyCode.R)) currentPrefab = squarePrefab;
        if (Input.GetKeyDown(KeyCode.B)) currentPrefab = blockPrefab;

        // 调整生成频率
        if (Input.GetKeyDown(KeyCode.Equals)) // 兼容 = 和 +
            spawnInterval = Mathf.Max(0.1f, spawnInterval - 0.1f);
        if (Input.GetKeyDown(KeyCode.Minus))
            spawnInterval += 0.1f;

        // 反转方向
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            directionMultiplier *= -1;

        // 判断是否生成
        if (Time.time >= nextSpawnTime)
        {
            GenerateMesh();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void OnDataReceived(string data, UduinoDevice device)
    {
        if (int.TryParse(data, out int distance))
        {
            lastDistance = distance;
        }
    }

    void GenerateMesh()
    {
        // 选择对应旋转角度
        Quaternion rotation = Quaternion.identity;
        if (currentPrefab == squarePrefab) rotation = Quaternion.Euler(squareRotation);
        if (currentPrefab == blockPrefab) rotation = Quaternion.Euler(blockRotation);
        if (currentPrefab == circlePrefab) rotation = Quaternion.Euler(circleRotation);

        // 根据距离决定生成方向
        if (lastDistance < 10)
            lastSpawnPosition += directionMultiplier * Vector3.right; // X
        else if (lastDistance < 20)
            lastSpawnPosition += directionMultiplier * Vector3.up; // Y
        else if (lastDistance < 30)
            lastSpawnPosition += directionMultiplier * Vector3.forward; // Z
        else if (lastDistance < 40)
            lastSpawnPosition += directionMultiplier * new Vector3(1, 1, 0).normalized; // XY
        else if (lastDistance < 50)
            lastSpawnPosition += directionMultiplier * new Vector3(1, 0, 1).normalized; // XZ
        else if (lastDistance < 60)
            lastSpawnPosition += directionMultiplier * new Vector3(0, 1, 1).normalized; // YZ
        else
            lastSpawnPosition += directionMultiplier * Vector3.one.normalized; // XYZ 默认方向

        // 实例化物体
        Instantiate(currentPrefab, lastSpawnPosition, rotation);
    }
}
