using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuChoose : MonoBehaviour
{
    public Button catBth;
    public Button mouseBth;
    // Start is called before the first frame update
    void Start()
    {
        catBth.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("PlayerType", "Cat");
            SceneManager.LoadScene(3);
        });
        mouseBth.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("PlayerType", "Mouse");
            SceneManager.LoadScene(3);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
