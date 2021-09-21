using Godot;

namespace Godotvania {
  public class LittleHeart : Area2D {
    [Signal]
    public delegate void CollectHeart(int count);

    public override void _Ready() {
      GD.Print("LittleHeart._Ready");
    }

    public void OnAreaEntered(Area2D area) {
      // do the logic later
      GD.Print("LittleHeart.OnAreaEntered");
      EmitSignal(nameof(CollectHeart), 1);
      GetParent().QueueFree();
    }
  }
}