using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Image loader;
    
    private Player player;

    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }

    public void PlayerToLastCheckPoint()
    {
        player.Deactivate();

        loader.rectTransform.DOScale(new Vector3(1, 1, 1), 1f).OnComplete(() =>
        {
            player.Activate();
            player.transform.position = player.currentCheckPointPosition;
            loader.rectTransform.DOScale(new Vector3(0, 0, 0), 1f);
        });
    }
}