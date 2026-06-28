using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Components;

[GlobalClass]
public partial class HealthComponent : Node2D
{
	[Export] public float MaxHealth = 20;

	private float health;

	public override void _Ready()
	{
		health = MaxHealth;
	}
	
	[Signal]
	public delegate void HealthChangedEventHandler(float hp);
	[Signal]
	public delegate void HealthFractionRemainingEventHandler(float fraction);

	public void Damage(float damage) {
		if (damage <= 0) return;
		health = Mathf.Clamp(health - damage, 0, MaxHealth);
		EmitSignalHealthChanged(health);
		EmitSignalHealthFractionRemaining(health/MaxHealth);
		GD.Print(damage);
	}

	public void Heal(float heal) {
		if  (heal <= 0) return;
		health = Mathf.Clamp(health + heal, 0, MaxHealth);
		EmitSignalHealthChanged(health);
		EmitSignalHealthFractionRemaining(health/MaxHealth);
	}
	public void SetMaxHealth(float newMaxHealth) {
		MaxHealth = newMaxHealth;
		health = MaxHealth;
		
	}

}
