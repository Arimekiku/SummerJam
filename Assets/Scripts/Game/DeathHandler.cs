using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] private Image loadImage;
    
    public void StartLevel(TweenCallback onAnimationEnd) => loadImage.rectTransform.DOScale(Vector3.zero, 1f).OnComplete(onAnimationEnd);
    public void EndLevel(TweenCallback onAnimationEnd) => loadImage.rectTransform.DOScale(Vector3.one, 1f).OnComplete(onAnimationEnd);
}