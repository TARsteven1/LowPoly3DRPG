using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private CinemachineFreeLook followCamera;

    List<IEndGameObserver> endGameObserver = new List<IEndGameObserver>();

    //观察者模式，反向注册的方法
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera!=null)
        {
            followCamera.Follow = playerStats.transform.GetChild(0);
            followCamera.LookAt = playerStats.transform.GetChild(0);
        }

    }
   public void AddObserver(IEndGameObserver observer)
    {
        endGameObserver.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObserver.Remove(observer);

    }
    //保证每个调用IEnd接口所有的函数最后都会执行Notify方法
    public void NotifyObserver()
    {
        foreach (var observer in endGameObserver)
        {
            observer.EndNotify();
        }
    }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    //找到出生点
    public Transform GetEnterance() {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag== TransitionDestination.DestinationTag.ENTER)
            {
                return item.transform;
            }
        }
        return null;
    }
}
