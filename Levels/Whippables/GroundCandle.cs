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
      GD.Print("GroundCandle.OnAreaEntered:start");
      CallDeferred("DropLoot");
      animationPlayer.Play("hit");
      GD.Print("GroundCandle.OnAreaEntered:end");
    }

    public void DropLoot() {
      GD.Print("GroundCandle.DropLoot:start");
      Area2D newheart = (Area2D)LittleHeartScene.Instance();
      newheart.Position = Position + new Vector2(0, -32);
      GetParent().AddChild(newheart, true);
      GD.Print("GroundCandle.DropLoot:end");
    }
  }
}