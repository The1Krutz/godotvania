using System.Dynamic;
using Godot;

public class Simon : KinematicBody2D {
  private int groundSpeed = 80;
  private int stairsSpeed = 60;
  private int jumpForce = 250;
  private int gravity = 800;
  private float jumpX = 0;

  private bool isOnStairs = false;
  private bool needsLerpStairsBottom = false;
  private bool needsLerpStairsTop = false;

  private Vector2 upStairsDirection;
  public bool NearStairsBottom { get; set; } = false;
  public bool NearStairsTop { get; set; } = false;
  public Vector2 StairsBottom { get; set; } = new Vector2();
  public Vector2 StairsTop { get; set; } = new Vector2();

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
    bool isJumping = !IsOnFloor();
    bool isGoingUpStairs = false;

    // movement handling
    if (isOnStairs) {
      isJumping = false;
      upStairsDirection = StairsBottom.DirectionTo(StairsTop);
      GetAlternateStairsInputs(out string alternateDownInput, out string alternateUpInput, out Vector2 upScale, out Vector2 downScale);

      /*
       * Called these lerp because maybe I'll lerp them in the future to smooth out the movement
       * Right now it's pretty jerky, but it works
       */
      if (needsLerpStairsBottom) {
        Position = StairsBottom;
        needsLerpStairsBottom = false;
      } else if (needsLerpStairsTop) {
        Position = StairsTop;
        needsLerpStairsTop = false;
      } else {
        // bottom of the stairs
        if (Position.y >= StairsBottom.y) {
          Position = new Vector2(Position.x, StairsBottom.y);
          isOnStairs = false;
        }
        // top of the stairs, gets a little leeway since you're passing through the collider sometimes
        if (Position.y < StairsTop.y - 1) {
          Position = new Vector2(Position.x, StairsTop.y);
          isOnStairs = false;
        }
      }

      if (Input.IsActionPressed("move_up") || Input.IsActionPressed(alternateUpInput)) {
        velocity = upStairsDirection * stairsSpeed;
        sprite.Scale = upScale;
        isWalking = true;
        isGoingUpStairs = true;
      } else if (Input.IsActionPressed("move_down") || Input.IsActionPressed(alternateDownInput)) {
        velocity = upStairsDirection * stairsSpeed * -1;
        sprite.Scale = downScale;
        isWalking = true;
      } else {
        velocity = Vector2.Zero;
        isWalking = false;
      }
    } else if (isJumping) {
      /*
       * override the normal physics calculations for X motion while in the air.
       * this lets you jump in corners, and also makes you fall straight down instead of in a gentle physics arc
       */
      velocity.x = jumpX;
    } else {
      velocity.x = 0;
      jumpX = 0;
      if (Input.IsActionPressed("move_right")) {
        velocity.x += groundSpeed;
        sprite.Scale = new Vector2(-1, 1);
        isWalking = true;
      } else if (Input.IsActionPressed("move_left")) {
        velocity.x -= groundSpeed;
        sprite.Scale = new Vector2(1, 1);
        isWalking = true;
      }

      if (NearStairsBottom && Input.IsActionPressed("move_up")) {
        isOnStairs = true;
        needsLerpStairsBottom = true;
      } else if (NearStairsTop && Input.IsActionPressed("move_down")) {
        isOnStairs = true;
        needsLerpStairsTop = true;
      } else if (Input.IsActionPressed("move_down")) {
        isDucking = true;
      }

      if (!isDucking && Input.IsActionJustPressed("jump")) {
        velocity.y -= jumpForce;
        jumpX = velocity.x;
      }
    }

    // physics stuff for movement
    if (!isOnStairs) {
      velocity.y += gravity * delta;
    }
    velocity = MoveAndSlide(velocity, Vector2.Up);

    // animation swapping
    if (isJumping) {
      animationPlayer.Play("jump");
    } else if (isWalking && isOnStairs) {
      if (isGoingUpStairs) {
        animationPlayer.Play("upstairs");
      } else {
        animationPlayer.Play("downstairs");
      }
    } else if (isOnStairs) {
      animationPlayer.Stop(); // freeze the walking animation on its current frame
    } else if (isWalking) {
      animationPlayer.Play("walk");
    } else if (isDucking) {
      animationPlayer.Play("jump");
    } else {
      animationPlayer.Play("stand");
    }
  }

  private void GetAlternateStairsInputs(out string alternateDownInput, out string alternateUpInput, out Vector2 upScale, out Vector2 downScale) {
    if (upStairsDirection.x > 0) {
      alternateUpInput = "move_right";
      alternateDownInput = "move_left";
      upScale = new Vector2(-1, 1);
      downScale = new Vector2(1, 1);
    } else {
      alternateUpInput = "move_left";
      alternateDownInput = "move_right";
      upScale = new Vector2(1, 1);
      downScale = new Vector2(-1, 1);
    }
  }
}
