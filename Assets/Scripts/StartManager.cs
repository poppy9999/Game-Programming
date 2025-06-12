using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public Button LoginBth;
    public Button helpBth;
    public Button mapBth;
    public Button exitBth;
    public Button cuiderUI;

    public Transform uiRoot;
    public GameObject mapPanelPrefab;
    private GameObject currentMapPanel;


    // Start is called before the first frame update
    void Start()
    {
        cuiderUI.gameObject.SetActive(false);

        LoginBth.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });

        helpBth.onClick.AddListener(() =>
        {
            cuiderUI.gameObject.SetActive(true);
        });

        cuiderUI.onClick.AddListener(() =>
        {
            cuiderUI.gameObject.SetActive(false);
        });

        exitBth.onClick.AddListener(() =>
        {
            Application.Quit();
            Debug.Log("Quit Clicked"); 
        });

        mapBth.onClick.AddListener(() =>
        {            
            if (currentMapPanel != null) return;
            currentMapPanel = Instantiate(mapPanelPrefab, uiRoot);
        });
    }

    void Update()
    {
        
    }
}
