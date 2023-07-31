using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    private Player playerPrefab;
    
    private void Awake()
    {
        playerPrefab = Resources.Load("Prefabs/Player/Player", typeof(Player)) as Player;

        Player instantiatedPlayer = Instantiate(playerPrefab, transform);
        instantiatedPlayer.Activate();
    }
}
