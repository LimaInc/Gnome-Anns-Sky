using Godot;
using System;

public class Pig : KinematicBody
{
    //TODO: terminal velocity

    private Vector3 velocity;
    private Vector2 direction;
    private float gravity = 20.0f;
    private float speed = 200.0f;
    private float jumpStrength = 5.0f;

    private float collisionEpsilon = 0.5f;


    private float timerThreshold = 2;
    private float timer = 0;

    Random random;

    private void SetRandomDirection()
    {
        direction = new Vector2((float)(random.NextDouble() * 2.0 - 1.0), (float)(random.NextDouble() * 2.0 - 1.0));
        direction = direction.Normalized();
    }

    private void Jump()
    {
        velocity.y = jumpStrength;
    }

    public override void _Ready()
    {
        random = new Random();
        SetRandomDirection();
        
    }

    public override void _Process(float delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.

        timer += delta;
        if(timer > timerThreshold)
        {
            timer = 0;
            SetRandomDirection();
        }
        velocity.x = direction.x * speed * delta;
        velocity.z = direction.y * speed * delta;
        velocity.y -= gravity * delta;

        //GD.Print("Before: ", velocity.x, " ", velocity.y, " ", velocity.z);

        if (this.IsOnFloor())
        {
            GD.Print("On floor!");
            if(this.TestMove(this.GetTransform(), velocity))
            {
                Jump();
                GD.Print("Jumping!");
            }
        }

        Vector3 newVelocity = this.MoveAndSlide(velocity, new Vector3(0.0f, 1.0f, 0.0f));
        float xDif = Math.Abs(velocity.x - newVelocity.x);
        float zDif = Math.Abs(velocity.z - newVelocity.z);

        velocity.y = newVelocity.y;

        if (this.IsOnFloor() && (xDif > collisionEpsilon || zDif > collisionEpsilon))
        {
            Jump();
            GD.Print("Jumping!");
        }

        

       // GD.Print("After: ", velocity.x, " ", velocity.y, " ", velocity.z);

    }
}
