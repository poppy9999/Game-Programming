using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "Game/Map Config")]
public class MapConfig : ScriptableObject
{
    public int mapType = 3; // 1 = Park, 2 = ParkingLot, 3 = Custom

    // 只有 mapType = 3 时使用：
    public int cityWidth = 3;
    public int cityHeight = 3;

    public GameObject[] sidewalkProps;
    public int propsPerSide = 8;
    public int propsPerBlock = 32;
}