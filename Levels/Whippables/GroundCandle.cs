using Godot;

namespace Godotvania {
  public class GroundCandle : Node2D {
    private PackedScene LittleHeartScene;
    private AnimationPlayer animationPlayer;

    public override void _Ready() {
      LittleHeartScene = ResourceLoader.Load<PackedScene>("res://Levels/Pickups/LittleHeart.tscn");
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

      animationPlayer.Play("idle");
    }

    public void OnAreaEntered(Area2D area) {
      CallDeferred("DropLoot");
      animationPlayer.Play("hit");
    }

    public void DropLoot() {
      RigidBody2D newheart = (RigidBody2D)LittleHeartScene.Instance();
      newheart.Position = Position + new Vector2(0, -16);
      GetParent().AddChild(newheart, true);
    }
  }
}