using UnityEngine;
using UnityEngine.UI;

public class AmmunitionUI : MonoBehaviour
{
    [SerializeField] private Sprite fillSprite;
    [SerializeField] private Sprite emptySprite;

    [SerializeField] private Image currentImage;

    public void Reload() => currentImage.sprite = fillSprite;
    public void Deplete() => currentImage.sprite = emptySprite;
}
