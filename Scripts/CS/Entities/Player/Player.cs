using System;
using DPlatformer.NET.Scripts.CS.Entities.Components;
using DPlatformer.NET.Scripts.CS.Entities.Misc;
using DPlatformer.NET.Scripts.CS.Misc;
using DPlatformer.NET.Scripts.CS.UI;
using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Player;

[GlobalClass]
public partial class Player : CharacterBody2D {
	// Variables 
	[ExportCategory("Vertical Movement")]
	[Export] public float Speed = 180f;
	[Export] public float Acceleration = 20f;
	[Export] public float Deacceleration = 10f;
	private float direction;
	
	[ExportCategory("Horizontal Movement")]
	[Export] public float JumpHeight = 128f;
	[Export] public float JumpTimeToPeak = 0.6f;
	[Export] public float JumpFallTime = 0.3f;
	private float jumpVelocity;
	private float jumpGravity;
	private float fallGravity;

	[Export] public float VariableJumpHeightStrength = 0.4f;
	[Export] public float FloatStrength = 10f;
	[Export] public float TerminalVelocity = 100f;
	
	// Imports
	private Sprite2D sprite;
	private AnimationPlayer animationPlayer;
	private Timer attackCooldownTimer;
	private Timer hitSlowTimer;
	private Timer floatCooldownTimer;
	private Timer coyoteTimer;
	private Timer jumpBufferTimer;
	private Timer respawnTimer;
	private static readonly Inventory PlayerInventory =
		ResourceLoader.Load<Inventory>("uid://bvyijoa5sha6v");
	private HealthComponent healthComponent;
	private HitBoxComponent hitboxComponent;
	
	// Signal
	[Signal]
	public delegate void HealthChangedEventHandler(float hp);	

	// State Machine
	private enum MovementState {
		Idle,
		Run,
		Jump,
		Fall,
		Float
	}
	private MovementState state = MovementState.Idle;
	private bool isAttacking = false;
	private bool isFloating = false;

	private bool isAlive = true;

	private Vector2 velocity;

	public override void _EnterTree()
	{
		base._EnterTree();
		Hud.Singleton.RegisterPlayer(this);
	}

	public override void _Ready() {
		jumpVelocity = -2f * JumpHeight / JumpTimeToPeak;
		jumpGravity = 2 * JumpHeight / JumpTimeToPeak / JumpTimeToPeak;
		fallGravity = 2 * JumpHeight / JumpFallTime / JumpFallTime;
		
		sprite = GetNode<Sprite2D>("%PlayerSprite");
		animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		attackCooldownTimer = GetNode<Timer>("%AttackCooldownTimer");
		hitSlowTimer = GetNode<Timer>("%HitSlowTimer");
		floatCooldownTimer = GetNode<Timer>("%FloatCooldownTimer");
		coyoteTimer = GetNode<Timer>("%CoyoteTimer");
		jumpBufferTimer = GetNode<Timer>("%JumpBufferTimer");
		respawnTimer = GetNode<Timer>("%RespawnTimer");
		hitboxComponent = GetNode<HitBoxComponent>("%HitBoxComponent");
		healthComponent = GetNode<HealthComponent>("%HealthComponent");
		
		animationPlayer.AnimationFinished += OnAnimationFinished;
		respawnTimer.Timeout += OnRespawnTimerTimeout;
		hitboxComponent.OnKnockback += OnHitboxKnockback;
		healthComponent.HealthChanged += OnHPChanged;
		hitSlowTimer.Timeout += OnHitSlowTimerTimeout;

	}

	public override void _PhysicsProcess(double delta) {
		if (!isAlive) return;
		velocity = Velocity;
		
		HandleInput();
		UpdateMovement((float)delta);
		UpdateState();
		UpdateAnimation();
		
		Velocity = velocity;
		MoveAndSlide();
	}

	private void HandleInput() {
		if(Input.IsActionJustPressed("attack") && attackCooldownTimer.IsStopped() ){
			isAttacking = true;
			attackCooldownTimer.Start();
		}
		if (Input.IsActionJustPressed("jump")) {
			jumpBufferTimer.Start();
		}
		if (Input.IsActionJustPressed("jump") && state == MovementState.Jump && !IsOnFloor()) {
			velocity.Y *= VariableJumpHeightStrength;
		}
		if (Input.IsActionJustPressed("interact")) {
			Interact();
		}

		isFloating = Input.IsActionPressed("jump") && !IsOnFloor();
		direction = Input.GetAxis("move_left", "move_right");
	}

	private void UpdateMovement(float delta) {
		// Kiting
		if (Mathf.Sign(direction) != Mathf.Sign(velocity.X)) {
			velocity.X = 0;
		}
		
		// Movement
		if (direction != 0f) {
			velocity.X = Mathf.MoveToward(velocity.X, direction * Speed, Acceleration * delta);  //delta??
		} else {
			velocity.X = Mathf.MoveToward(velocity.X, 0, Deacceleration * delta);
		}
		
		// Able to jump if player is on the ground (coyote timer included) or jump buffered
		if ((IsOnFloor() || !coyoteTimer.IsStopped()) && !jumpBufferTimer.IsStopped()) {
			state = MovementState.Jump;
			jumpBufferTimer.Stop();
			coyoteTimer.Stop();
			velocity.Y = jumpVelocity;
		}

		// Gravity
		if (state == MovementState.Jump) {
			velocity.Y += jumpGravity * delta;
		} else if (state == MovementState.Float) {
			velocity.Y += (fallGravity / FloatStrength) * delta;
		} else {
			velocity.Y += fallGravity * delta;
		}

		velocity.Y = Mathf.Min(velocity.Y, TerminalVelocity);
	}

	private void UpdateState() {
		switch (state) {
			case MovementState.Idle: 
				if(Mathf.Abs(velocity.X) >= Speed/10) 
					state = MovementState.Run;
				break;
			
			case MovementState.Run:
				if (Mathf.Abs(velocity.X) <= 0)
					state = MovementState.Idle;

				if (!IsOnFloor() && velocity.Y > 0) {
					state = MovementState.Fall;
					coyoteTimer.Start();
				}
				break;
			
			case MovementState.Jump:
				state = (isFloating)?  MovementState.Float : MovementState.Fall;
				break;
			
			case MovementState.Fall:
				if (IsOnFloor()) {
					state = velocity.X == 0 ? MovementState.Idle : MovementState.Run;
				}
				if (isFloating && floatCooldownTimer.IsStopped()) {
					state = MovementState.Float;
					velocity.Y *= 0.1f;
					floatCooldownTimer.Start();
				}
				break;
			
			case MovementState.Float:
				if (!isFloating && isAttacking) {
					state = MovementState.Fall;
				}
				if (IsOnFloor()) {
					state = (velocity.X == 0)?  MovementState.Idle : MovementState.Run;
				}
				break;
			
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void UpdateAnimation() {
		if (isAttacking) {
			animationPlayer.Play("attack");
		} else {
			if (velocity.X != 0) {
				var pivot = GetNode<Node2D>("%Pivot");
				pivot.Scale = pivot.Scale with { X = Mathf.Sign(velocity.X) };
			}
			switch (state) {
				case MovementState.Idle: animationPlayer.Play("idle"); break;
				case MovementState.Run: animationPlayer.Play("run"); break;
				case MovementState.Jump: animationPlayer.Play("jump"); break;
				case MovementState.Fall: animationPlayer.Play("fall"); break;
				case MovementState.Float: animationPlayer.Play("float"); break;
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void Interact() {
		Area2D interactionArea = GetNode<Area2D>("%Pivot/InteractionArea");
		if (!interactionArea.HasOverlappingBodies()) return;
		Npc npc = (Npc)interactionArea.GetOverlappingBodies()[0];
		npc.Interact();
	}

	public float GetMaxHealth()
	{
		return healthComponent.MaxHealth;
	}

	private void OnAnimationFinished(StringName animName) {
		if  (animName == "attack")
			isAttacking = false;
	}

	private void OnRespawnTimerTimeout() {
		Engine.TimeScale = 1;
		isAlive = true;
		sprite.Visible = true;
		GetTree().ReloadCurrentScene();
	}
	
	private void OnHitboxKnockback(Vector2 force) {
		velocity = force;
		velocity.Y /= 1.6f;
		state = MovementState.Jump;
	}

	private void OnHPChanged(float hp) {
		EmitSignalHealthChanged(hp);
		if (hp <= 0) {
			Die();
		}
	}
	private void Die() {
		isAlive = false;
		PlayerInventory.Reset();
		animationPlayer.Play("death");
		sprite.Visible = false;
		GD.Print("YOU DIED!");
		Engine.TimeScale = 0.5f;
		respawnTimer.Start();
	}


	private void OnDamageDealtToEnemy() {
		Engine.TimeScale = 0.5f;
		hitSlowTimer.Start();
	}
	private void OnHitSlowTimerTimeout() {
		Engine.TimeScale = 1;
	}
}
