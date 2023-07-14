using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Image loadImage;
    [SerializeField] private AnimationCurve loadCurve;

    private Game game;
    private bool started;

    private void Start()
    {
        game = FindObjectOfType<Game>();
        game.Player.OnDeathEvent += RestartLevel;

        loadImage.gameObject.SetActive(true);

        if (SceneManager.GetActiveScene().buildIndex != 0)
            StartLevel();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 && !started)
        {
            if (Input.anyKeyDown)
            {
                StartCoroutine(Tweener.Scale(loadImage.gameObject, Vector3.zero, loadCurve, () =>
                {
                    game.InitializeAll();
                    loadImage.GetComponentInChildren<SpriteRenderer>().enabled = false;
                }));

                started = true;
            }
        }          
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player)) 
        {
        
        }    
    }

    public void RestartLevel() 
    {
        game.DeinitializeAll();

        EndLevel(() =>
        {
            game.TeleportPlayerToLastCheckPoint();

            StartLevel();
        });
    }

    public void StartNextLevel()
    {
        game.DeinitializeAll();

        EndLevel(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

    public void StartLevel()
    {
        StartCoroutine(Tweener.Scale(loadImage.gameObject, Vector3.zero, loadCurve, game.InitializeAll));
    }

    public void EndLevel(Action onAnimationEnd) 
    {
        StartCoroutine(Tweener.Scale(loadImage.gameObject, Vector3.one, loadCurve, onAnimationEnd));
    }
}