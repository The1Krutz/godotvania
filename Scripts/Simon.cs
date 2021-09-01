using Godot;

public class Simon : KinematicBody2D {
  private int speed = 50;
  private int jumpForce = 200;
  private int gravity = 800;

  private Vector2 velocity = new Vector2();
  private bool grounded = false;

  private AnimatedSprite sprite;

  public override void _Ready() {
    sprite = GetNode<AnimatedSprite>("AnimatedSprite");
  }

  public override void _PhysicsProcess(float delta) {
    if (IsOnFloor()) {
      velocity.x = 0;
      if (Input.IsActionPressed("move_left")) {
        velocity.x -= speed;
      }
      if (Input.IsActionPressed("move_right")) {
        velocity.x += speed;
      }
    }

    velocity = MoveAndSlide(velocity, Vector2.Up);

    velocity.y += gravity * delta;

    if (Input.IsActionPressed("jump") && IsOnFloor()) {
      velocity.y -= jumpForce;
    }

    if (velocity.x > 0) {
      sprite.FlipH = true;
    } else if (velocity.x < 0) {
      sprite.FlipH = false;
    }
  }
}
