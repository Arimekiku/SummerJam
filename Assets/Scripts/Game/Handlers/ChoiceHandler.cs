using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoiceHandler : MonoBehaviour
{
    [SerializeField] private Text mainText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    
    private void Awake()
    {
        canvasGroup.alpha = 0;
        
        leftButton.onClick.AddListener(Disappear);
        rightButton.onClick.AddListener(Disappear);
    }

    public void Appear()
    {
        canvasGroup.DOFade(1f, 2f);
        leftButton.transform.DOScale(Vector3.one, 1f).SetEase(Ease.InSine);
        rightButton.transform.DOScale(Vector3.one, 1f).SetEase(Ease.InSine);
    }

    private void Disappear()
    {
        canvasGroup.DOFade(0f, 2f);
        leftButton.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InSine);
        rightButton.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InSine);
        
        leftButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(Disappear);
        
        rightButton.onClick.RemoveAllListeners();
        rightButton.onClick.AddListener(Disappear);
    }

    public void AddActionToLeftButton(UnityAction callback)
    {
        leftButton.onClick.AddListener(callback);
    }

    public void SetLeftButtonText(string text)
    {
        Text textComponent = leftButton.GetComponentInChildren<Text>();
        textComponent.text = text;
    }

    public void RemoveActionFromLeftButton(UnityAction callback)
    {
        leftButton.onClick.RemoveListener(callback);
    }

    public void AddActionToRightButton(UnityAction callback)
    {
        rightButton.onClick.AddListener(callback);
    }
    
    public void SetRightButtonText(string text)
    {
        Text textComponent = rightButton.GetComponentInChildren<Text>();
        textComponent.text = text;
    }

    public void RemoveActionFromRightButton(UnityAction callback)
    {
        rightButton.onClick.RemoveListener(callback);
    }

    public void SetMainText(string text)
    {
        mainText.text = text;
    }
}
