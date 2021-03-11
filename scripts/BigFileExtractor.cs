using Godot;
using System;

public class BigFileExtractor : Control
{
	[Signal]
	public delegate void output(string data);
	public override void _Ready()
	{
		Start();
	}

	async void Start()
	{
		await ToSignal(GetTree().CreateTimer(.5f,false),"timeout");
		GD.Print("Started BigFile Extractor Scene");
		Connect("output",this,"OnOutput");
		string path = Software.pathWorkbench+"/bigfile";

		if (System.IO.Directory.Exists(@path)) {
			Transition.instance.ChangeScene(Transition.SCENE.emulator_settings);
			return;
		}

		path = Software.pathWorkbench+"/ctr_source/BIGFILE.BIG";
		ExecuteProcess.Main(this, ExecuteProcess.PROGRAM.BIGFILE);
	}

	public void OnOutput(string data) {
		GD.Print(data);
		if (data == "OK")
		{
			Transition.instance.ChangeScene(Transition.SCENE.emulator_settings);
		}
	}


}
