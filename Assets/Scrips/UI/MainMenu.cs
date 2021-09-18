using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button Startbtn;
    Button Continuebtn;
    Button Quitbtn;

    private void Awake()
    {
        Startbtn = transform.GetChild(1).GetComponent<Button>();
        Continuebtn = transform.GetChild(2).GetComponent<Button>();
        Quitbtn = transform.GetChild(3).GetComponent<Button>();
        Startbtn.onClick.AddListener(NewGame);
        Continuebtn.onClick.AddListener(ContinueGame);
        Quitbtn.onClick.AddListener(QuitGame);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void NewGame() {
        PlayerPrefs.DeleteAll();

        SenceManager.Instance.TransitionToFirstLevel();
    }
    void ContinueGame() {
        SenceManager.Instance.TransitionToLoadGame();
    }
    void QuitGame() {
        Application.Quit();

    }
}
