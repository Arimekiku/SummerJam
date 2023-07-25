using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    [Space, SerializeField] private CanvasGroup moveGroup;
    [Space, SerializeField] private CanvasGroup attackGroup;
    [Space, SerializeField] private CanvasGroup dashGroup;
    
    private const int TotalTutorials = 3;

    private Coroutine movementTutorial;
    private bool movementTutorialStarted;

    private Coroutine attackTutorial;
    private bool attackTutorialStarted;

    private Coroutine dashTutorial;
    private bool dashTutorialStarted;

    private int currentTutorialsHandled;

    private void Update()
    {
        if (movementTutorialStarted && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            StopTutorial(moveGroup, movementTutorial);

            movementTutorialStarted = false;
        }

        if (attackTutorialStarted && Input.GetMouseButtonDown(0))
        {
            StopTutorial(attackGroup, attackTutorial);

            attackTutorialStarted = false;
        }

        if (dashTutorialStarted && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StopTutorial(dashGroup, dashTutorial);
            

            dashTutorialStarted = false;
        }
        
        if (currentTutorialsHandled == TotalTutorials)
            Destroy(gameObject, 5f);
    }

    private void StopTutorial(CanvasGroup groupToFade, Coroutine coroutineToStop)
    {
        groupToFade.DOFade(0, 3f).OnKill(() => Destroy(groupToFade.gameObject));
        
        if (coroutineToStop != null)
            StopCoroutine(coroutineToStop);
        currentTutorialsHandled++;
    }

    public void StartMoveTutorial()
    {
        movementTutorial = StartCoroutine(FadeGroupTimer(moveGroup));
        movementTutorialStarted = true;
    }

    public void StartAttackTutorial()
    {
        attackTutorial = StartCoroutine(FadeGroupTimer(attackGroup));
        attackTutorialStarted = true;
    }

    public void StartDashTutorial()
    {
        dashTutorial = StartCoroutine(FadeGroupTimer(dashGroup));
        dashTutorialStarted = true;
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