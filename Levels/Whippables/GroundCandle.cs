using Godot;

namespace Godotvania {
  public class GroundCandle : Node2D {
    private PackedScene DropScene;
    private AnimationPlayer animationPlayer;

    [Export]
    public PickupType drops;

    public override void _Ready() {
      DropScene = GetDropScene();
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

      animationPlayer.Play("idle");
    }

    public void OnAreaEntered(Area2D area) {
      CallDeferred("DropLoot");
      animationPlayer.Play("hit");
    }

    public void DropLoot() {
      RigidBody2D drop = (RigidBody2D)DropScene.Instance();
      drop.Position = Position + new Vector2(0, -16);
      GetParent().AddChild(drop, true);
    }

    private PackedScene GetDropScene() {
      return drops switch
      {
        PickupType.LittleHeart => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/LittleHeart.tscn"),
        PickupType.WhipUpgrade => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/WhipUpgrade.tscn"),
        PickupType.BigHeart => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/BigHeart.tscn"),
        PickupType.MoneyBag => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/MoneyBag.tscn"),
        PickupType.DoubleShot => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/DoubleShot.tscn"),
        PickupType.TripleShot => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/TripleShot.tscn"),
        PickupType.Rosary => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/Rosary.tscn"),
        _ => ResourceLoader.Load<PackedScene>("res://Levels/Pickups/LittleHeart.tscn"),
      };
    }
  }
}