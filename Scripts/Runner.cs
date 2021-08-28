using System;
using Godot;

public class Runner : KinematicBody2D {
  private AnimatedSprite _animatedSprite;

  public override void _Ready() {
    _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
  }

  public override void _Process(float delta) {
    if (Input.IsActionPressed("ui_right")) {
      _animatedSprite.Play("run");
    } else {
      _animatedSprite.Stop();
    }
  }
}
