using System.Collections.Generic;
using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Enemy.Boss;

[GlobalClass]
public partial class Boss : CharacterBody2D
{
    [Export] public Area2D ActivatorArea;
    [Export] public BossResource Stats;

    private Timer attackCooldown;
    private AnimationPlayer animationPlayer;
    private ShapeCast2D playerSearchAbove;
    private AnimationPlayer attackAnimationPlayer;
    private Node2D pivot;
    private CpuParticles2D underGroundParticles;

    private Util.BossState state = Util.BossState.OnGround;
    private bool belowPlayer;
    private float damage;
    private bool canChangeState = true;
    private bool movingVertically;

    private bool active;
    private Area2D playerArea;
    private Vector2 playerDirection;
    private float facingDir; // 1 right, -1 left
    private Vector2 targetPosition;

    public override void _Ready()
    {
        attackCooldown = GetNode<Timer>("%AttackCooldown");
        animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        playerSearchAbove = GetNode<ShapeCast2D>("%PlayerSearchAbove");
        attackAnimationPlayer = GetNode<AnimationPlayer>("%AttackAnimationPlayer");
        pivot = GetNode<Node2D>("%Pivot");
        underGroundParticles = GetNode<CpuParticles2D>("%UnderGroundParticles");

        attackAnimationPlayer.AnimationFinished += OnAttackAnimationPlayerAnimationFinished;

        if (ActivatorArea != null)
            ActivatorArea.AreaEntered += OnActivatorAreaEntered;
        else active = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!active) return;
        if (attackAnimationPlayer.IsPlaying()) return;

        HandleVisuals();

        Move((float)delta);

        if (attackCooldown.TimeLeft <= 0 && !movingVertically)
            HandleAttack();
    }

    private void HandleVisuals()
    {
        bool isUnderground = state == Util.BossState.Underground;
        underGroundParticles.Emitting = isUnderground && !movingVertically;
    }

    private void Move(float delta)
    {
        playerDirection = (playerArea.GlobalPosition - GlobalPosition).Normalized();
        facingDir = Mathf.Sign(playerDirection.X);
        if (facingDir != 0)
            pivot.Scale = new Vector2(-facingDir, pivot.Scale.Y);

        targetPosition.X = playerArea.GlobalPosition.X;

        Vector2 moveDir = (targetPosition - GlobalPosition).Normalized();

        var pos = GlobalPosition;
        pos.Y = Mathf.MoveToward(pos.Y, targetPosition.Y, delta * Stats.Speed);
        GlobalPosition = pos;

        movingVertically = !Mathf.IsEqualApprox(targetPosition.Y, GlobalPosition.Y);

        if (Stats.MoveableStates.Contains(state) && !movingVertically)
        {
            pos = GlobalPosition;
            pos.X = Mathf.MoveToward(pos.X, targetPosition.X, delta * Stats.Speed);
            GlobalPosition = pos;
        }
    }

    private void HandleAttack()
    {
        bool playerAbove = playerSearchAbove.IsColliding();
        var usableAttacks = new List<Attack>();

        foreach (Attack attack in Stats.Attacks)
        {
            if (attack == null) continue;
            if (attack.IsAvailable(state, playerAbove) && (canChangeState || !attack.IsStateChange()))
                usableAttacks.Add(attack);
        }

        if (usableAttacks.Count == 0) return;

        Attack chosenAttack = usableAttacks[GD.RandRange(0, usableAttacks.Count - 1)];
        StringName animName = chosenAttack.AnimationName;
        damage = chosenAttack.Damage;
        GD.Print(chosenAttack.ResourcePath);

        if (animName != "")
            attackAnimationPlayer.Play(animName);
        else
            StartCooldown();

        if (chosenAttack.IsStateChange())
        {
            TransitionState(chosenAttack.StateTransition);
            canChangeState = false;
        }
        else
        {
            canChangeState = true;
        }
    }

    private void TransitionState(Util.BossState nextState)
    {
        switch (state)
        {
            case Util.BossState.OnGround:
                if (nextState == Util.BossState.Underground)
                    targetPosition.Y += 100;
                break;

            case Util.BossState.Underground:
                if (nextState == Util.BossState.OnGround)
                    targetPosition.Y -= 100;
                break;
        }

        state = nextState;
    }

    private void OnActivatorAreaEntered(Area2D area)
    {
        active = true;
        playerArea = area;
        targetPosition = GlobalPosition;
        StartCooldown();
        GD.Print("activated");
        ActivatorArea.AreaEntered -= OnActivatorAreaEntered;
    }

    private void OnAttackAnimationPlayerAnimationFinished(StringName animName)
    {
        if (animName != "RESET")
            attackAnimationPlayer.Play("RESET");
        else
            StartCooldown();
    }

    private void StartCooldown()
    {
        float cd = (float)GD.RandRange(
            Stats.Cooldown - Stats.CooldownDeviation,
            Stats.Cooldown + Stats.CooldownDeviation
        );
        attackCooldown.Start(cd);
    }
}