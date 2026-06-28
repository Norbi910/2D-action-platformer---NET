using DPlatformer.NET.Scripts.CS.Entities.Components;
using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Enemy;

[GlobalClass]
public partial class Ant : Enemy
{
	[Export] public float Speed = 20f;
	[Export] public float MaxHp = 100f;
	[Export] public float Damage = 20f;

	private int dir = 1;  
	private bool isAlive = true;
	
	private RayCast2D rayCastAhead;
	private RayCast2D rayCastDown;
	private Sprite2D sprite;
	private Node2D pivot;
	private AnimationPlayer animationPlayer;
	private HealthComponent healthComponent;
	private AttackComponent attackComponent;
	private HitBoxComponent hitBoxComponent;

	public override void _Ready()
	{
		rayCastAhead = GetNode<RayCast2D>("%RayCastAhead");
		rayCastDown = GetNode<RayCast2D>("%RayCastDown");
		sprite = GetNode<Sprite2D>("%Sprite2D");
		pivot = GetNode<Node2D>("%Pivot");
		animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("%HealthComponent");
		attackComponent = GetNode<AttackComponent>("%AttackComponent");
		hitBoxComponent = GetNode<HitBoxComponent>("%HitBoxComponent");
		
		healthComponent.MaxHealth = MaxHp;
		attackComponent.AttackDamage =  Damage;
		healthComponent.HealthChanged += OnHealthChanged;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (rayCastAhead.IsColliding() || !rayCastDown.IsColliding()) {
			dir *= -1;
			pivot.SetScale(new Vector2(dir, pivot.Scale.Y));
		}
		velocity.X = dir * Speed;
		velocity += GetGravity() * (float)delta;

		float groundFollowingRotation = GetRealVelocity().AngleTo(new Vector2(dir, 0f)) * -dir;
		sprite.Rotation = Mathf.MoveToward(sprite.Rotation, groundFollowingRotation, 0.1f);

		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnHealthChanged(float hp)
	{
		if (hp <= 0) {
			EmitSignal(nameof(DeadEventHandler));
			isAlive = false;
			healthComponent.QueueFree();
			hitBoxComponent.QueueFree();
			attackComponent.QueueFree();
			animationPlayer.Play("death");
		}
	}
}