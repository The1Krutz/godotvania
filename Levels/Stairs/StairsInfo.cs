using Godot;

namespace Godotvania {
  public class StairsInfo {
    // set
    public Vector2 TopPoint { get; }
    public Vector2 BottomPoint { get; }

    // calculated
    public Vector2 UpStairsDirection { get; }
    public string AlternateUpInput { get; }
    public string AlternateDownInput { get; }
    public Vector2 UpScale { get; }
    public Vector2 DownScale { get; }

    public StairsInfo(Vector2 bottomPoint, Vector2 topPoint) {
      TopPoint = topPoint;
      BottomPoint = bottomPoint;

      UpStairsDirection = BottomPoint.DirectionTo(TopPoint);
      if (UpStairsDirection.x > 0) {
        AlternateUpInput = "move_right";
        AlternateDownInput = "move_left";
        UpScale = new Vector2(-1, 1);
        DownScale = new Vector2(1, 1);
      } else {
        AlternateUpInput = "move_left";
        AlternateDownInput = "move_right";
        UpScale = new Vector2(1, 1);
        DownScale = new Vector2(-1, 1);
      }
    }
  }
}