using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private List <IGameLoop> GameLoops = new List<IGameLoop>();
    private bool inErrorState;

public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        try
        {
            if (!inErrorState)
                foreach (var gameLoop in GameLoops)
                    gameLoop.Update();
        }
        catch (System.Exception e)
        {
            HandleGameLoopException(e);
            throw;
        }
    }

    void HandleGameLoopException(System.Exception e)
    {
        Debug.Log("EXCEPTION: " + e.Message + "\n" + e.StackTrace);
        // Time.timeScale = 0; // If certain game objects continue some behaviour, uncomment this line.
        inErrorState = true;
    }

    public void AddGameLoop(IGameLoop gameLoop) { GameLoops.Add(gameLoop); }
    public interface IGameLoop
    {
        void Update();
        // void FixedUpdate();
        // void LateUpdate();
    }
}
