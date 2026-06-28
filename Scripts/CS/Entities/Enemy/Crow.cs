using DPlatformer.NET.Scripts.CS.Entities.Components;
using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Enemy;

[GlobalClass]
public partial class Crow : Enemy
{
	[Export] public float Speed = 250f;

	private Area2D target;
	private Vector2 direction = Vector2.Zero;
	private bool isAlive = true;
	
	private AnimationPlayer animationPlayer;
	private Sprite2D sprite;
	private CollisionShape2D collision;
	private Timer directionRecalculateTimer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("$AnimationPlayer");
		sprite = GetNode<Sprite2D>("$Sprite2D");
		collision = GetNode<CollisionShape2D>("$Collision");
		directionRecalculateTimer = GetNode<Timer>("$DirectionSearchTimer");
		
		Area2D playerFinderArea = GetNode<Area2D>("$PlayerFinderArea");
		playerFinderArea.AreaEntered += OnPlayerFinderAreaEntered;
		playerFinderArea.AreaExited += OnPlayerFinderAreaExited;
		GetNode<HealthComponent>($"HealthComponent").HealthChanged += OnHealthComponentHpChanged;

	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;

		AnimationHandler();

		if (target == null) {
			Velocity += GetGravity() * (float)delta;
		}
		else {
			if (directionRecalculateTimer.IsStopped())
			{
				direction = (target.GlobalPosition - GlobalPosition).Normalized();
				directionRecalculateTimer.Start();
			}

			Velocity = Velocity.MoveToward(direction * Speed, 10);
			GetNode<RayCast2D>("DebugDirection").TargetPosition = direction * Speed;
		}

		MoveAndSlide();
	}

	private void OnPlayerFinderAreaEntered(Area2D area)
	{
		target = area;
		direction = (target.GlobalPosition - GlobalPosition).Normalized();
		((CircleShape2D)collision.Shape).Radius = 16;
	}

	private void OnPlayerFinderAreaExited(Area2D area)
	{
		if (area == target)
		{
			target = null;
			((CircleShape2D)collision.Shape).Radius = 10;
		}
	}

	private void AnimationHandler()
	{
		sprite.FlipH = Velocity.X >= 0;

		if (target == null && IsOnFloor())
			animationPlayer.Play("idle");
		else
			animationPlayer.Play("attack");
	}

	private void OnHealthComponentHpChanged(float hp)
	{
		if (hp <= 0)
			Die();
	}

	private void Die() {
		isAlive = false;
		EmitSignal(nameof(DeadEventHandler));
		GetNode("HealthComponent").QueueFree();
		GetNode("HitBoxComponent").QueueFree();
		GetNode("AttackComponent").QueueFree();

		animationPlayer.Play("death");
	}
}