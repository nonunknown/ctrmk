using Godot;
using System;
using System.IO;
public class SourceCheck : Control
{
    Label msg;
    public override void _Ready()
    {
		GD.Print("Started Source Check Scene");

        msg = GetNode<Label>("Panel/VBoxContainer/HBoxContainer/lb_msg");
        CheckFiles();
    }

    async void CheckFiles() 
    {
        await ToSignal(GetTree(),"idle_frame");
        string path = Software.pathWorkbench+"/ctr_source/BIGFILE.BIG";
        while(true)
        {
            if (System.IO.File.Exists(@path)) break;
            
            await ToSignal(GetTree().CreateTimer(.5f,false),"timeout");
        }

        Transition.instance.ChangeScene(Transition.SCENE.bigfile_extractor);
    }

    void OnMetaClicked(string arg)
    {
        if (arg.Contains("www"))
        {
            OS.ShellOpen(arg);
        }
        else
        {
            arg = arg.Replace("\"","");
            OS.ShellOpen(Software.pathWorkbench+arg);
        }

        GD.Print("META: ", arg, "full: ", Software.pathWorkbench+arg);
    }
}
