using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : Utility.UniqueInstance<GameManager>
{
    public int maxAchivement = 3;
    public int nbAchievement = 0;

    public void Start()
    {
    }

    public void Achieve()
    {
        nbAchievement++;
        if (nbAchievement == maxAchivement) MenuManager.Instance.end.OpenDialog();

    }
}
