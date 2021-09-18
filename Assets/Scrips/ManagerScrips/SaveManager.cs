using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName="defaul";
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); }   }

    // PlayerPrefs保存数据
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SenceControler.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }
    public void Save(Object data ,string key) {
        var jsonData = JsonUtility.ToJson(data,true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName,SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    public void Load(Object data, string key) {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
       
    }
   public void SavePlayerData() { Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name); }
   public void LoadPlayerData() { Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name); }

    
}
