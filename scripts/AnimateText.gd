extends Control

export var speed:float = .5
var t = 0

func _process(delta: float) -> void:
	t += delta * speed
	if t > 1: t = 0
	set("percent_visible", t)
#	print(percent_visible)
