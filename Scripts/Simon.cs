using Godot;

public class Simon : KinematicBody2D {
  private int speed = 100;
  private int jumpForce = 300;
  private int gravity = 1000;

  private Vector2 velocity = new Vector2();

  private AnimationPlayer animationPlayer;
  private Sprite sprite;

  public override void _Ready() {
    animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    sprite = GetNode<Sprite>("Sprite");
  }

  public override void _PhysicsProcess(float delta) {
    bool isWalking = false;
    bool isDucking = false;

    // movement handling
    if (IsOnFloor()) {
      velocity.x = 0;
      if (Input.IsActionPressed("move_left")) {
        velocity.x -= speed;
        isWalking = true;
      }
      if (Input.IsActionPressed("move_right")) {
        velocity.x += speed;
        isWalking = true;
      }
      if (Input.IsActionJustPressed("jump")) {
        velocity.y -= jumpForce;
      }
      isDucking = Input.IsActionPressed("move_down");
    }

    // physics stuff for movement
    velocity.y += gravity * delta;
    velocity = MoveAndSlide(velocity, Vector2.Up);

    // animation swapping
    if (IsOnFloor() && !isWalking && !isDucking) {
      animationPlayer.Play("stand");
    } else if (!IsOnFloor()) {
      animationPlayer.Play("jump");
    } else if (isWalking) {
      animationPlayer.Play("walk");
    } else if (isDucking) {
      animationPlayer.Play("jump");
    }

    // sprite faces the right way
    if (velocity.x > 0) {
      sprite.Scale = new Vector2(-1, 1);
    } else if (velocity.x < 0) {
      sprite.Scale = new Vector2(1, 1);
    }
  }
}
