using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameBeginner : MonoBehaviour
{
    [SerializeField] private Player player;
    [Space, SerializeField] private CanvasGroup moveGroup;
    [Space, SerializeField] private CanvasGroup attackGroup;
    [Space, SerializeField] private CanvasGroup dashGroup;

    private Vector3 playerStartPosition;
    private Coroutine movementTutorial;
    private bool movementHandled;

    private Coroutine attackTutorial;
    private bool attackHandled;

    private Coroutine dashTutorial;
    private bool dashHandled;

    private void Awake()
    {
        playerStartPosition = player.transform.position;
    }

    public void BeginGame()
    {
        Light2D playerLight = player.GetComponent<Light2D>();

        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, 0.02f, 5f);
        player.Rend.DOFade(1, 5f).OnComplete(() =>
        {
            player.Activate();
            movementTutorial = StartCoroutine(FadeGroupTimer(moveGroup));
        });
    }

    private void Update()
    {
        if (playerStartPosition != player.transform.position)
        {
            if (!movementHandled)
            {
                moveGroup.DOFade(0, 3f).OnKill(() => Destroy(moveGroup.gameObject));
            
                if (movementTutorial != null)
                    StopCoroutine(movementTutorial);

                movementHandled = true;
            }
        }

        /*if (!attackHandled)
        {
            attackGroup.DOFade(0, 3f).OnKill(() => Destroy(attackGroup.gameObject));
            
            if (attackTutorial != null)
                StopCoroutine(attackTutorial);

            attackHandled = true;
        }*/

        /*if (!dashHandled)
        {
            dashGroup.DOFade(0, 3f).OnKill(() =>
            {
                Destroy(dashGroup.gameObject);
                Destroy(gameObject);
            });
            
            if (dashTutorial != null)
                StopCoroutine(dashTutorial);

            dashHandled = true;
        }*/
    }

    public void StartAttackTutorial()
    {
        StartCoroutine(FadeGroupTimer(attackGroup));
    }

    public void StartDashTutorial()
    {
        StartCoroutine(FadeGroupTimer(dashGroup));
    }
    
    private static IEnumerator FadeGroupTimer(CanvasGroup group)
    {
        float currentTime = 0;

        while (currentTime < 0)
        {
            yield return new WaitForFixedUpdate();
            currentTime += Time.fixedDeltaTime;
        }

        group.DOFade(1, 3f);
    }
}
