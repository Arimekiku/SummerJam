using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Player Player { get; private set; }

    private CameraFollow cameraFollow;
    private List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        Player = FindAnyObjectByType<Player>();
        cameraFollow = FindAnyObjectByType<CameraFollow>();
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
    }

    public void InitializeAll() 
    {
        Player.Initialize();
        cameraFollow.Initialize();
        enemies.ForEach(e => e.Initialize());
    }

    public void DeinitializeAll() 
    {
        Player.Deinitialize();
        cameraFollow.Deinitialize();
        enemies.ForEach(e => e.Deinitialize());
    }

    public void TeleportPlayerToLastCheckPoint() 
    {
        Player.transform.position = Player.currentCheckPointPosition;
        Camera.main.transform.position = Player.transform.position;
    }
}
