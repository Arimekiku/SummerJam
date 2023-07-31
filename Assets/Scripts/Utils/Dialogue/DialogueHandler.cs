using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private Text messageBox;
    [SerializeField] private Player player;
    [SerializeField] private int charPerSecond;

    private DialogueActor[] actorsInDialogue;
    private DialoguePhrase[] phrasesInDialogue;
    private UnityEvent toPerform;
    private int currentMessageIndex;

    private Vector3 initialScale;
    private CanvasGroup alphaGroup;
    private IEnumerator dialogueCoroutine;

    private void Awake()
    {
        initialScale = transform.localScale;
        alphaGroup = GetComponent<CanvasGroup>();
        alphaGroup.alpha = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && phrasesInDialogue != null)
        {
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
                messageBox.text = phrasesInDialogue[currentMessageIndex].Message;
                dialogueCoroutine = null;
                return;    
            }
                
            NextMessage();
        }
    }
    
    public void StartDialogue(DialoguePhrase[] phrases, DialogueActor[] actors, UnityEvent @event)
    {
        player.Deactivate();
        
        actorsInDialogue = actors;
        phrasesInDialogue = phrases;
        toPerform = @event;
        currentMessageIndex = 0;

        List<Tween> possibleTweens = DOTween.TweensByTarget(transform);
        if (possibleTweens is not null)
            foreach (Tween tween in possibleTweens)
                tween?.Kill();
        
        gameObject.SetActive(true);
        DisplayCurrentMessage();
        alphaGroup.DOFade(1, 0.25f);
        transform.DOScale(Vector3.one, 0.25f);
    }

    private void DisplayCurrentMessage()
    {
        messageBox.text = string.Empty;
        
        DialoguePhrase currentPhrase = phrasesInDialogue[currentMessageIndex];
        dialogueCoroutine = AnimateMessage();
        StartCoroutine(dialogueCoroutine);

        DialogueActor currentActor = actorsInDialogue[currentPhrase.ActorID];
        currentActor.ApplyActorSprite();

        IEnumerator AnimateMessage()
        {
            int charIndex = 0;
            
            while (messageBox.text != currentPhrase.Message)
            {
                messageBox.text += currentPhrase.Message[charIndex];
                charIndex++;
                
                yield return new WaitForSeconds(1f / charPerSecond);
            }

            dialogueCoroutine = null;
        }
    }

    private void NextMessage()
    {
        currentMessageIndex++;

        if (phrasesInDialogue.Length <= currentMessageIndex)
        {
            phrasesInDialogue = null;
            actorsInDialogue = null;

            alphaGroup.DOFade(0, 0.25f);
            transform.DOScale(initialScale, 0.25f).OnComplete(() => gameObject.SetActive(false));
            toPerform?.Invoke();
            return;
        }
        
        actorsInDialogue[phrasesInDialogue[currentMessageIndex - 1].ActorID].RevertActorSprite();
        DisplayCurrentMessage();
    }
}
