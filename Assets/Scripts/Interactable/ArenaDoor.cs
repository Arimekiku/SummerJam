using DG.Tweening;
using UnityEngine;

public class ArenaDoor : MonoBehaviour
{
    [SerializeField] private Vector3 target;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    public void Close()
    {
        transform.DOLocalMove(target, 1f).SetEase(Ease.InCubic);
    }

    public void Open()
    {
        transform.DOLocalMove(startPosition, 1f).SetEase(Ease.InCubic);
    }
}
