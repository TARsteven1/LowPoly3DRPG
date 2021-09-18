using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button Startbtn;
    Button Continuebtn;
    Button Quitbtn;
PlayableDirector directer;

    private void Awake()
    {
        Startbtn = transform.GetChild(1).GetComponent<Button>();
        Continuebtn = transform.GetChild(2).GetComponent<Button>();
        Quitbtn = transform.GetChild(3).GetComponent<Button>();
        Startbtn.onClick.AddListener(PlayTImeline);
        Continuebtn.onClick.AddListener(ContinueGame);
        Quitbtn.onClick.AddListener(QuitGame);

directer=FindObjectOfType<PlayableDirector>();
directer.stopped+=NewGame;
    }
    // Start is called before the first frame update

void PlayTImeline(){
        directer.Play();
}
    void NewGame(PlayableDirector obj) {
        PlayerPrefs.DeleteAll();
        Debug.Log(111);
        SenceControler.Instance.TransitionToFirstLevel();
    }
    void ContinueGame() {
        SenceControler.Instance.TransitionToLoadGame();
    }
    void QuitGame() {
        Application.Quit();

    }
}
