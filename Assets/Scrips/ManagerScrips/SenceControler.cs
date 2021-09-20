using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SenceControler : Singleton<SenceControler>,IEndGameObserver
{
    GameObject player;
    public GameObject playerPrefs;
    NavMeshAgent playerAgent;
    bool isdDead=true;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        //注册观察者
        GameManager.Instance.AddObserver(this);
    }

    public void TransitionToDestination(TransitionPoint transitionPoint) {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
            default:
                break;
        }
    }
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag) {
        SaveManager.Instance.SavePlayerData();
        if (SceneManager.GetActiveScene().name!=sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefs, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            playerAgent.enabled = true;
            yield return null;
        }
       
    }
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag) {
        var entrances = FindObjectsOfType<TransitionDestination>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag== destinationTag)
            {
                return entrances[i];
            }
        }
        return null;
    }
    public void TransitionToMain() {
        StartCoroutine(LoadMain());
    }
    public void TransitionToLoadGame() {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    public void TransitionToFirstLevel() {
        StartCoroutine(LoadLevel("SampleScene"));
    
    }
    IEnumerator LoadLevel(string scene) {
        if (scene!="")
        {
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefs, GameManager.Instance.GetEnterance().position, GameManager.Instance.GetEnterance().rotation);
            SaveManager.Instance.SavePlayerData();
            yield break;
           
         }
    }
    IEnumerator LoadMain() {
        yield return SceneManager.LoadSceneAsync("Start");
        yield break;
    }

    public void EndNotify()
    {
        if (isdDead)
        {
            isdDead = false;
            StartCoroutine(LoadMain());
        }
        //Destroy(gameObject);
        
    }
}
