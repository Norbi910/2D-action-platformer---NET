using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Enemy;

[GlobalClass]
public partial class Enemy : CharacterBody2D
{
	[Signal]
	public delegate void DeadEventHandler();
}