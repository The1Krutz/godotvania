using Godot;

public class Simon : KinematicBody2D {
  private int speed = 80;
  private int jumpForce = 250;
  private int gravity = 800;
  private float jumpX = 0;

  [Export]
  private bool isWhipping = false; // controlled by the animation player

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
    bool isWhipping = false;
    bool isOnStairs = false;
    bool isJumping = !IsOnFloor();

    // movement handling
    if (isJumping) {
      /*
       * override the normal physics calculations for X motion while in the air.
       * this lets you jump in corners, and also makes you fall straight down instead of in a gentle physics arc
       */
      velocity.x = jumpX;
    } else {
      velocity.x = 0;
      jumpX = 0;
      if (Input.IsActionPressed("move_left")) {
        velocity.x -= speed;
        sprite.Scale = new Vector2(1, 1);
        isWalking = true;
      }
      if (Input.IsActionPressed("move_right")) {
        velocity.x += speed;
        sprite.Scale = new Vector2(-1, 1);
        isWalking = true;
      }
      if (Input.IsActionJustPressed("jump")) {
        velocity.y -= jumpForce;
        jumpX = velocity.x;
      }
      isDucking = Input.IsActionPressed("move_down");
    }

    // physics stuff for movement
    velocity.y += gravity * delta;
    velocity = MoveAndSlide(velocity, Vector2.Up);

    // animation swapping
    if (isJumping) {
      animationPlayer.Play("jump");
    } else if (isWalking) {
      animationPlayer.Play("walk");
    } else if (isDucking) {
      animationPlayer.Play("jump");
    } else {
      animationPlayer.Play("stand");
    }
  }
}
