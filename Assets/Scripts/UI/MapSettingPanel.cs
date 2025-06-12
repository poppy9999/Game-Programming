using UnityEngine;
using UnityEngine.UI;

public class MapSettingPanel : MonoBehaviour
{
    public GameObject map1Mark;
    public GameObject map2Mark;
    public GameObject map3Mark;

    public Button map1Btn;
    public Button map2Btn;
    public Button map3Btn;
    public Button backBtn;

    void Start()
    {
        map1Btn.onClick.AddListener(() => SelectMap(1));
        map2Btn.onClick.AddListener(() => SelectMap(2));
        map3Btn.onClick.AddListener(() => SelectMap(3));
        backBtn.onClick.AddListener(ClosePanel);

        // 初始化状态（比如用保存的 mapType）
        SelectMap(PlayerPrefs.GetInt("MapType", 1));
    }

    void SelectMap(int type)
    {
        map1Mark.SetActive(type == 1);
        map2Mark.SetActive(type == 2);
        map3Mark.SetActive(type == 3);

        PlayerPrefs.SetInt("MapType", type);
        PlayerPrefs.Save();
    }

    void ClosePanel()
    {
        Destroy(gameObject);
    }
}
