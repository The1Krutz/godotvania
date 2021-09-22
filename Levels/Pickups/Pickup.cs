using Godot;

namespace Godotvania {
  public class Pickup : Area2D {
    [Signal]
    public delegate void CollectPickup(PickupType type);

    [Export(hint: PropertyHint.Enum)]
    public PickupType pickupType;

    public override void _Ready() {
      GD.Print("Pickup._Ready");
    }

    public void OnAreaEntered(Area2D area) {
      // do the logic later
      GD.Print($"Pickup.OnAreaEntered: {pickupType}");
      EmitSignal(nameof(CollectPickup), pickupType);
      GetParent().QueueFree();
    }
  }

  public enum PickupType {
    LittleHeart,
    BigHeart,
    WhipUpgrade,
    MoneyBag,
    DoubleShot,
    TripleShot,
    Rosary,
  }
}