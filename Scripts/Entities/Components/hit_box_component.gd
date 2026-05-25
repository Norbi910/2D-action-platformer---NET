class_name HitboxComponent
extends Area2D

@onready var invincibilty_timer: Timer = $InvincibiltyTimer

@export var health_component: HealthComponent
@export var invincibility_time: float = 0.5
@export var hit_flash: HitFlashComponent

signal on_knockback(force: Vector2)



func damage(dmg: float, direction: Vector2) -> bool:
	if not health_component:
		return false
	if invincibilty_timer.time_left >0: 
		return false
	health_component.damage(dmg)
	var force := maxf(400, dmg * 10)
	on_knockback.emit(direction * force)
	invincibilty_timer.start(invincibility_time)
	if hit_flash:
		hit_flash.toggle_flashing()
	return true
	
	
func _on_invincibilty_timer_timeout() -> void:
	if not hit_flash: return
	hit_flash.toggle_flashing()
