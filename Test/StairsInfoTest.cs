using System;
using Godot;

namespace Godotvania.Test {
  [Title("Stairs Info tests")]

  public class StairsInfoTest : WAT.Test {
    [Test]
    public void UpLeftTest() {
      GD.Print("tomato 1");
      Vector2 top = new Vector2(-5, -5);
      Vector2 bottom = new Vector2(0, 0);
      StairsInfo result = new StairsInfo(bottom, top);

      Assert.IsEqual(result.AlternateUpInput, "move_left");
    }

    [Test]
    public void UpLeftTest2() {
      GD.Print("tomato 2");
      Vector2 top = new Vector2(-5, -5);
      Vector2 bottom = new Vector2(0, 0);
      StairsInfo result = new StairsInfo(bottom, top);

      Assert.IsEqual(result.AlternateUpInput, "move_left");
    }

    [Test]
    public void UpRightTest() {
      GD.Print("tomato 3");
      Vector2 top = new Vector2(5, -5);
      Vector2 bottom = new Vector2(0, 0);
      StairsInfo result = new StairsInfo(bottom, top);

      Assert.IsEqual(result.AlternateUpInput, "move_right");
    }
  }
}
