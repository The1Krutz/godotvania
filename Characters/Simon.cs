using Godot;

public class Simon : KinematicBody2D {
  private int groundSpeed = 80;
  private int stairsSpeed = 60;
  private int jumpForce = 250;
  private int gravity = 800;
  private float jumpX = 0;

  private StairsInfo currentStairs;
  public StairsInfo NearStairsBottom { get; set; }
  public StairsInfo NearStairsTop { get; set; }
  private bool needsLerpStairsBottom = false;
  private bool needsLerpStairsTop = false;

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
    if (currentStairs != null) {
      isJumping = false;

      if (Input.IsActionPressed("move_up") || Input.IsActionPressed(currentStairs.AlternateUpInput)) {
        velocity = currentStairs.UpStairsDirection * stairsSpeed;
        sprite.Scale = currentStairs.UpScale;
        isWalking = true;
        isGoingUpStairs = true;
      } else if (Input.IsActionPressed("move_down") || Input.IsActionPressed(currentStairs.AlternateDownInput)) {
        velocity = currentStairs.UpStairsDirection * stairsSpeed * -1;
        sprite.Scale = currentStairs.DownScale;
        isWalking = true;
      } else {
        velocity = Vector2.Zero;
        isWalking = false;
      }

      /*
       * Called these lerp because maybe I'll lerp them in the future to smooth out the movement
       * Right now it's pretty jerky, but it works
       */
      if (needsLerpStairsBottom) {
        Position = currentStairs.BottomPoint;
        needsLerpStairsBottom = false;
      } else if (needsLerpStairsTop) {
        Position = currentStairs.TopPoint;
        needsLerpStairsTop = false;
      } else {
        if (Position.y >= currentStairs.BottomPoint.y) {
          // bottom of the stairs
          Position = new Vector2(Position.x, currentStairs.BottomPoint.y);
          currentStairs = null;
        } else if (Position.y < currentStairs.TopPoint.y - 1) {
          // top of the stairs, gets a little leeway since you're passing through the floor collider sometimes
          Position = new Vector2(Position.x, currentStairs.TopPoint.y);
          currentStairs = null;
        }
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

      if (NearStairsBottom != null && Input.IsActionPressed("move_up")) {
        currentStairs = NearStairsBottom;
        needsLerpStairsBottom = true;
      } else if (NearStairsTop != null && Input.IsActionPressed("move_down")) {
        currentStairs = NearStairsTop;
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
    if (currentStairs == null) {
      velocity.y += gravity * delta;
    }
    velocity = MoveAndSlide(velocity, Vector2.Up);

    // animation swapping
    if (isJumping) {
      animationPlayer.Play("jump");
    } else if (isWalking && currentStairs != null) {
      if (isGoingUpStairs) {
        animationPlayer.Play("upstairs");
      } else {
        animationPlayer.Play("downstairs");
      }
    } else if (currentStairs != null) {
      animationPlayer.Stop(); // freeze the walking animation on its current frame
    } else if (isWalking) {
      animationPlayer.Play("walk");
    } else if (isDucking) {
      animationPlayer.Play("jump");
    } else {
      animationPlayer.Play("stand");
    }
  }
}
