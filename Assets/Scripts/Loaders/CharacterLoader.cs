using Cinemachine;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera defaultCamera;
    
    private Player playerPrefab;
    
    public Player InitializePlayer()
    {
        playerPrefab = Resources.Load("Prefabs/Player/Player", typeof(Player)) as Player;

        Player instantiatedPlayer = Instantiate(playerPrefab, transform);
        instantiatedPlayer.Activate();

        defaultCamera.Follow = instantiatedPlayer.transform;

        return instantiatedPlayer;
    }
}
