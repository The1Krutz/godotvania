using Godot;

namespace Godotvania {
  public class LittleHeart : Area2D {
    public void OnAreaEntered(Area2D area) {
      // do the logic later
      GD.Print("LittleHeart.OnAreaEntered");
    }
  }
}