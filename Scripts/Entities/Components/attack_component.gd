class_name AttackComponent
extends Area2D

@export var ATTACK_DAMAGE: float = 20.0

signal damage_dealt

func _process(_delta: float) -> void:
	if not has_overlapping_areas():
		return
	for area: Area2D in get_overlapping_areas():
		if area is not HitboxComponent:
			continue
		var hitbox: HitboxComponent = area
		var knockback_direction: Vector2 = (area.global_position - global_position).normalized()
		if hitbox.damage(ATTACK_DAMAGE, knockback_direction):
			damage_dealt.emit()
