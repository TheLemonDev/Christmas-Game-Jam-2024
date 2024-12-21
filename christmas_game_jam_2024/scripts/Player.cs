using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{
	[Export]
	public float speed;

	[Export]
	public float speedMultiplyer;

	[Export]
	public float speedMultiplyerOffset;

	[Export]
	public float turnSpeed;

	[Export]
	public float dragThreshold;

	[Export]
	public float gravity;
	public float gatheredGravity;

	[Export]
	public Label text;

	[Export]
	public Sprite2D sprite;

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// keyboard rotation
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction.X != 0)
		{
			GlobalRotationDegrees -= direction.X * turnSpeed * (float)delta;
		}
		else if (Math.Abs(GlobalRotationDegrees) >= 90)
		{
			GlobalRotationDegrees -= turnSpeed / 2 * (float)delta;
		}
		else
		{
			GlobalRotationDegrees += turnSpeed / 2 * (float)delta;
		}

		// we now add a force to the player's velocity in their facing angle equal to the speed.
		velocity.X = speed * (float)Math.Cos(GlobalRotation);
		velocity.Y = speed * (float)Math.Sin(GlobalRotation) + gatheredGravity;

		float speedMod;

		// now we change the player's speed based off of the angle they are facing, we do this so the player can build and lose momentum.
		// We modify the value of the rotation (so that it is the strongest at the medium point) and then normalise it to 0 - 1 then multiply on an offset value- 
		// -This is done so that dives and ascends affect the speed of the player the most.
		// the game is biased towards you losing speed to entice the players to be efficent with their speed as well as to incentivise collecting power-ups.
		if (GlobalRotationDegrees > 0)
		{
			float speedChange = (1 - (Math.Abs(Math.Abs(GlobalRotationDegrees) - 90) / 90)) * speedMultiplyerOffset;
			speed += speedChange * speedMultiplyer * (float)delta;
			speedMod = speedChange;
		}
		else
		{
			float speedChange = (1 - (Math.Abs(Math.Abs(GlobalRotationDegrees) - 90) / 90)) * speedMultiplyerOffset;
			speed -= speedChange * 1.1f * speedMultiplyer * (float)delta;
			if (speed < 0) speed = 0;
			speedMod = speedChange;
		}

		// flip the player based off of what direction they are facing so that the player is always upright.
		if (Math.Abs(GlobalRotationDegrees) >= 90)
		{
			sprite.FlipV = true;
		}
		else
		{
			sprite.FlipV = false;
		}

		// when the player moves too slow and drops below the threshold whey start to be affected by gravity. This means you must go fast to stay airborn.
		if (speed < dragThreshold)
		{
			gatheredGravity += gravity * (float)delta;
		}
		else
		{
			gatheredGravity *= .98f;
		}

		// apply velocity of course.
		Velocity = velocity;
		MoveAndSlide();

		// boring debugging stuff.
		text.Text = "ANGLE: " + GlobalRotationDegrees.ToString() + " SPEED MODIFIER: " + speedMod.ToString() + " SPEED: " + speed.ToString();
	}
}
