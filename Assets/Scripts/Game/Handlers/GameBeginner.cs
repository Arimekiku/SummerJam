using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(UIHandler), typeof(TutorialHandler))]
public class GameBeginner : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CameraObject cameraHandler;
    [SerializeField] private CinemachineVirtualCamera defaultCamera;

    private UIHandler uiHandler;
    private TutorialHandler tutorialHandler;

    private void Awake()
    {
        uiHandler = GetComponent<UIHandler>();
        tutorialHandler = GetComponent<TutorialHandler>();
    }

    private void Start()
    {
        uiHandler.SetPlayer(player);
        
        player.OnReloadRequired += uiHandler.PlayerToLastCheckPoint;
        player.OnReloadRequired += () => defaultCamera.enabled = true;
        player.OnActivate += cameraHandler.FindTarget;
        player.OnDeactivate += cameraHandler.LoseTarget;
        player.OnWeaponPickup += PickupHandle;
    }

    public void BeginGame()
    {
        Light2D playerLight = player.GetComponent<Light2D>();

        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, 0.02f, 2f);
        
        player.Rend.DOFade(1, 2f).OnComplete(() =>
        {
            player.Activate();
            tutorialHandler.StartMoveTutorial();
        });
    }

    private void PickupHandle()
    {
        tutorialHandler.StartAttackTutorial();
        player.OnWeaponPickup -= PickupHandle;
    }
}
