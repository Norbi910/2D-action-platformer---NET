class_name HealthComponent
extends Node2D

@export var MAX_HEALTH: float = 10.0:
	set(value):
		var previous_max = MAX_HEALTH
		MAX_HEALTH = value
		health = health * MAX_HEALTH / previous_max
		update_bar()
var health: float:
	set(value):
		health = clamp(value, 0, MAX_HEALTH)
		
@export var hp_bar: HpBar

signal hp_changed(hp: float)

func _ready() -> void:
	health = MAX_HEALTH

func damage(dmg: float):
	health -= dmg
	hp_changed.emit(health)
	update_bar()

func heal(hp: float):
	health += hp
	hp_changed.emit(health)
	update_bar()

func update_bar():
	if hp_bar:
		hp_bar.update(health/ MAX_HEALTH)

	
