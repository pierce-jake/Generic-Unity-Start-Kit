using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public Camera MainCamera { get; private set; }

    public GameState state;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        MainCamera =  camera ? camera : Camera.main;
        DontDestroyOnLoad(gameObject);
    }

}

public enum GameState
{
    Title, Gameplay, Paused, GameOver
}