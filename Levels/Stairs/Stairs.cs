using Godot;

public class Stairs : Node2D {
  private Line2D path;
  private StairsInfo Info;

  public override void _Ready() {
    path = GetNode<Line2D>("path");

    var bottom = path.Points[0] + Position;
    var top = path.Points[1] + Position;

    Info = new StairsInfo(bottom, top);
  }

  public void OnBottomBodyEntered(Node body) {
    if (body is Simon player) {
      player.NearStairsBottom = Info;
    }
  }

  public void OnBottomBodyExited(Node body) {
    if (body is Simon player) {
      player.NearStairsBottom = null;
    }
  }

  public void OnTopBodyEntered(Node body) {
    if (body is Simon player) {
      player.NearStairsTop = Info;
    }
  }

  public void OnTopBodyExited(Node body) {
    if (body is Simon player) {
      player.NearStairsTop = null;
    }
  }
}
