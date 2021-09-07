using Godot;

public class Stairs : Node2D {
  private Line2D path;
  private Vector2 bottom;
  private Vector2 top;

  public override void _Ready() {
    path = GetNode<Line2D>("path");

    bottom = path.Points[0] + Position;
    top = path.Points[1] + Position;
  }

  public void OnBottomBodyEntered(Node body) {
    if (body is Simon player) {
      player.NearStairsBottom = true;
      player.StairsBottom = bottom;
      player.StairsTop = top;
    }
  }

  public void OnBottomBodyExited(Node body) {
    if (body is Simon player) {
      player.NearStairsBottom = false;
    }
  }

  public void OnTopBodyEntered(Node body) {
    if (body is Simon player) {
      player.NearStairsTop = true;
      player.StairsBottom = bottom;
      player.StairsTop = top;
    }
  }

  public void OnTopBodyExited(Node body) {
    if (body is Simon player) {
      player.NearStairsTop = false;
    }
  }
}
