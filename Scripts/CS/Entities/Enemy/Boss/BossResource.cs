using Godot;
using Godot.Collections;

namespace DPlatformer.NET.Scripts.CS.Entities.Enemy.Boss;

[GlobalClass]
public partial class BossResource : Resource
{
	[Export] public Array<Attack> Attacks;

	[Export(PropertyHint.Range, "0.5,10,0.25")]
	public float Cooldown = 5;

	[Export] public float CooldownDeviation = 1;

	[Export] public Array<Util.BossState> MoveableStates;

	[Export] public float Speed = 50;
}