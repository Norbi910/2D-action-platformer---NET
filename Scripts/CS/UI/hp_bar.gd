extends Control
class_name HpBar

@onready var hp: ColorRect = $hp

func update(percentage: float):
	hp.scale.x = percentage
