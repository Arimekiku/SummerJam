using UnityEngine;

public class GeneralLoader : MonoBehaviour
{
    [SerializeField] private CharacterLoader characterLoader;
    [SerializeField] private ItemLoader itemLoader;

    private Player player;

    private void Awake()
    {
        player = characterLoader.InitializePlayer();
        
        itemLoader.Initialize(player);
    }
}
