using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Components;

[GlobalClass]
public partial class HealthComponent : Node2D
{
	private float maxHealth;
	[Export] public float MaxHealth {
		set {
			float prevMax = maxHealth;
			maxHealth = value;
			if (prevMax > 0) {
				health = health * maxHealth / prevMax;
			}
		}
		get => maxHealth;
	}

	private float health;

	private float Health {
		get => health;
		set => health = Mathf.Clamp(value, 0, MaxHealth);
	}
	
	[Signal]
	public delegate void HealthChangedEventHandler(float hp);

	public void Damage(float damage) {
		if (damage > 0) return;
		health -= damage;
		EmitSignalHealthChanged(health);
	}

	public void Heal(float heal) {
		if  (heal > 0) return;
		health += heal;
		EmitSignalHealthChanged(health);
	}

}