using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Enemy.Boss;

[GlobalClass]
public partial class Attack : Resource
{
	[Export] public StringName AnimationName;
	[Export] public float Damage;
	[Export] public Util.BossState RequiredState;
	[Export] public bool RequiredUnderPlayer;
	[Export] public Util.BossState StateTransition;

	public bool IsAvailable(Util.BossState bossState, bool underPlayer)
	{
		bool under = !RequiredUnderPlayer || underPlayer;
		return bossState == RequiredState && under;
	}

	public bool IsStateChange()
	{
		return StateTransition != RequiredState;
	}
}