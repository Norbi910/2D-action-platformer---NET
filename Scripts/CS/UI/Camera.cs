using Godot;

namespace DPlatformer.NET.Scripts.CS.UI;

[GlobalClass]
public partial class Camera : Camera2D
{

	[Export] public int MaxOffset = 50;
	private float velocity = 0;
	private CharacterBody2D parent;

	public override void _Ready()
	{
		parent = GetParent<CharacterBody2D>();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
			
		float direction = Input.GetAxis("camera_up", "camera_down");

		if (direction != 0 && parent?.IsOnFloor() == true) {
			velocity = Mathf.MoveToward(velocity, MaxOffset * direction, 4);
		}
		else {
			velocity = Mathf.MoveToward(velocity, 0, 6);
		}

		Offset = Offset with { Y = Mathf.MoveToward(Offset.Y, velocity, 4) };
	}
}