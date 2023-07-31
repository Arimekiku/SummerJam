using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    private Player playerPrefab;
    
    public Player InitializePlayer()
    {
        playerPrefab = Resources.Load("Prefabs/Player/Player", typeof(Player)) as Player;

        Player instantiatedPlayer = Instantiate(playerPrefab, transform);
        instantiatedPlayer.Activate();

        return instantiatedPlayer;
    }
}
