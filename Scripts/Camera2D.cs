using Godot;

public class Camera2D : Godot.Camera2D {
  private Simon player;

  public override void _Ready() {
    player = GetNode<Simon>("Main/Simon");
  }

  public override void _Process(float delta) {
    // TODO: fix this
    // this.Position.x = player.Position.x;
  }
}
