public interface IPickupable
{
    public bool Pickupable { get; }
    
    public void PickUp(Player player);
}