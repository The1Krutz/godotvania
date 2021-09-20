using System;
using Godot;

namespace Godotvania {
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
    private bool isGoingUpStairs = false;
    private bool IsOnStairs => currentStairs != null;

    [Export]
    private bool isWhipping = false; // controlled by the animation player
    private WhipLevel currentWhip = WhipLevel.Basic;

    private Vector2 velocity = new Vector2();

    private AnimationPlayer animationPlayer;
    private Sprite sprite;

    public override void _Ready() {
      // get references to other useful things
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
      sprite = GetNode<Sprite>("Sprite");

      // set defaults
      isWhipping = false;
      animationPlayer.Play("stand");
      FaceRight();
    }

    public override void _PhysicsProcess(float delta) {
      bool isWalking = false;
      bool isDucking = false;
      bool isJumping = !IsOnFloor();

      if (isWhipping) {
        // continue existing trajectory, for jumping/falling
        if (IsOnStairs) {
          velocity = Vector2.Zero;
        } else {
          velocity.y += gravity * delta;
        }
        if (IsOnFloor()) {
          velocity = Vector2.Zero;
        }
        velocity = MoveAndSlide(velocity, Vector2.Up);

        return;
      }

      // movement handling
      if (IsOnStairs) {
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
          isGoingUpStairs = false;
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
        // stuck in place when whipping
        velocity.x = 0;
        jumpX = 0;
        if (Input.IsActionPressed("move_right")) {
          velocity.x += groundSpeed;
          FaceRight();
          isWalking = true;
        } else if (Input.IsActionPressed("move_left")) {
          velocity.x -= groundSpeed;
          FaceLeft();
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

      // whipping stuff
      if (Input.IsActionJustPressed("primary_attack")) {
        PlayWhipAnimation(isDucking, isGoingUpStairs, currentWhip);
        return;
      }

      // physics stuff for movement
      if (!IsOnStairs) {
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

    private void FaceRight() {
      sprite.Scale = new Vector2(-1, 1);
    }

    private void FaceLeft() {
      sprite.Scale = new Vector2(1, 1);
    }

    private void PlayWhipAnimation(bool isDucking, bool isGoingUpStairs, WhipLevel currentWhip) {
      /**
       * Most of these animations don't exist yet, but the Print statements accurately reflect which animation *should* be played once they get added
       */
      switch (currentWhip) {
        case WhipLevel.Basic:
          if (IsOnStairs) {
            if (isGoingUpStairs) {
              animationPlayer.Play("whip-upstairs-basic");
            } else {
              animationPlayer.Play("whip-downstairs-basic");
            }
          } else if (isDucking) {
            animationPlayer.Play("whip-ducking-basic");
          } else {
            animationPlayer.Play("whip-standing-basic");
          }
          break;
        case WhipLevel.Short:
          if (IsOnStairs) {
            if (isGoingUpStairs) {
              GD.Print("upstairs short");
            } else {
              GD.Print("downstairs short");
            }
          } else if (isDucking) {
            GD.Print("ducking short");
          } else {
            GD.Print("standing short");
          }
          break;
        case WhipLevel.Long:
          if (IsOnStairs) {
            if (isGoingUpStairs) {
              GD.Print("upstairs long");
            } else {
              GD.Print("downstairs long");
            }
          } else if (isDucking) {
            GD.Print("ducking long");
          } else {
            GD.Print("standing long");
          }
          break;
      }
    }
  }

  public enum WhipLevel {
    Basic,
    Short,
    Long
  }
}