using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Components;

[GlobalClass]
public partial class HitBoxComponent : Area2D
{
	[Export] public HealthComponent HealthComponent;
	[Export] public float InvincibilityDuration = 0.5f;
	
	private Timer invincibilityTimer;
	public override void _Ready() {
		invincibilityTimer = GetNode<Timer>("%InvincibilityTimer");
	}
	
	[Signal]
	public delegate void OnKnockbackEventHandler(Vector2 force);
	[Signal]
	public delegate void OnHitEventHandler(float invincibilityDuration);

	public bool Damage(float damage, Vector2 direction)
	{
		if (HealthComponent == null || !invincibilityTimer.IsStopped())
			return false;
		HealthComponent.Damage(damage);
		float force = Mathf.Max(200f, damage * 10f);
		EmitSignalOnKnockback(direction * force);
		invincibilityTimer.Start(InvincibilityDuration);
		EmitSignalOnHit(InvincibilityDuration);
		return true;
	}
}
