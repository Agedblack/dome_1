using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }

    public void AddObserver(IEndGameObserver observer)//加入
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)//移除
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}
