using Godot;

public class GroundCandle : Node2D {
  public void OnAreaEntered(Area2D area) {
      // drop loot later
      QueueFree();
  }
}
