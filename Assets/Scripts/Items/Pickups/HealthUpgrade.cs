using UnityEngine;

public class HealthUpgrade : ItemBehaviour
{
    [SerializeField] private int amount;
    [SerializeField] private Dialogue dialogue;

    public override void Initialize(Player player)
    {
        //player.IncreaseMaxHp(amount);
        //dialogue.Interact();
        print("Max hp increased");
    }
}
