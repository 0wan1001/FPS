using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public bool IsGameRunning { get; private set; }
    public static GameManager Instance;

    private void Awake() => SetSingleton();
    private void Start() => Run();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) Instance.Pause();
        else if (Input.GetKeyDown(KeyCode.O)) Instance.Run();
    }
    
    public void Run()
    {
        LockCursor();
        Time.timeScale = 1;
        IsGameRunning = true;
    }

    public void Pause()
    {
        UnlockCursor();
        Time.timeScale = 0;
        IsGameRunning = false;
    }
    
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}