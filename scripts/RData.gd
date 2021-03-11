extends Resource
class_name RData

export var emulator_epsxe:Dictionary = {
	exe_path = "",
	bios_path = "",
	cue_path = ""
}

export var emulator_bizhawk:Dictionary = {
	exe_path = "",
	cue_path = ""
}

export var emulator_nocash:Dictionary = {
	exe_path = "",
	cue_path = ""
}

export var emulator_duck:Dictionary = {
	exe_path = "",
	bios_path = "",
	cue_path = ""
}

func save(path:String) -> void:
	var err = ResourceSaver.save(path, self)
	if err != OK:
		push_error("Error saving data")
		return
	pass

func retrieve(path:String) -> RData:
	return ResourceLoader.load(path) as RData
