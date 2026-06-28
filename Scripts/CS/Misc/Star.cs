using DPlatformer.NET.Scripts.CS.Entities.Enemy;
using Godot;

namespace DPlatformer.NET.Scripts.CS.Misc;

public partial class Star : Node2D
{
	[Export] public Enemy DropSource { get; set; }

	private AnimatedSprite2D animatedSprite2D;
	private Area2D area2D;

	public InventoryItem ItemResource =
		ResourceLoader.Load<InventoryItem>("res://Resources/Inventory/star.tres");

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		area2D = GetNode<Area2D>("Area2D");

		if (DropSource != null)
			DropSource.Dead += Spawn;

		animatedSprite2D.AnimationFinished += OnAnimatedSprite2DAnimationFinished;
	}

	private void Spawn()
	{
		GlobalPosition = DropSource.GlobalPosition;
		Visible = true;
		area2D.Monitorable = true;
	}

	public void Pickup()
	{
		area2D.Monitorable = false;
		animatedSprite2D.Play("pickup");
	}

	private void OnAnimatedSprite2DAnimationFinished()
	{
		QueueFree();
	}
}