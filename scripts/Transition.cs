using Godot;
using System;

public class Transition : Node
{
    [Export]
    NodePath npScene;
    Node scene = null;
    public static Transition instance = null;

    public enum SCENE {source_check,bigfile_extractor,emulator_settings,main_menu,custom_racer,track_viewer}
    private string[] scenes_path = new string[] {
        "res://scenes/SourceCheck.tscn",
        "res://scenes/BIGExtract.tscn",
        "res://scenes/EmulatorSettings.tscn",
        "res://scenes/MainMenu.tscn",
        "res://scenes/CustomRacer.tscn"
    };
    public override void _Ready()
    {
        scene = GetNode<Node>(npScene);
        instance = this;
    }

    public async void ChangeScene(SCENE sceneID) 
    {
        
        //Validate current scene
        if (scene.GetChildCount() == 0 || scene.GetChildCount() > 1)
        {
            if (scene.GetChildCount() > 1)
            {
                byte tries = 4;

                while(tries > 0)
                {
                    tries -= 1;
                    await ToSignal(GetTree().CreateTimer(.5f,false),"timeout");
                    if (scene.GetChildCount() > 1)
                        continue;
                    goto perform;
                }
                GD.Print("Error changing scene, more than one child!");
                return;
            }
            GD.Print("Error changing scene, no child in there");
            return;
        }

        perform:

        //Move away from screen
        Control outScene = scene.GetChild<Control>(0);
        DoAnimation(outScene,true,true);


        //Load Next scene
        PackedScene ps = ResourceLoader.Load(scenes_path[(int)sceneID]) as PackedScene;
        Control inScene = ps.Instance() as Control;
        scene.CallDeferred("add_child",inScene);
        inScene.RectGlobalPosition = new Vector2(-1300,0);

        //Move into screen
        DoAnimation(inScene,false,true);


    }

    private void DoAnimation(Control target,bool toOut=true, bool toRight=false)
    {
        Vector2 valueStart,valueFinal;

        if (toOut)
        {
            if (toRight)
            {
                valueStart = new Vector2(0,0);
                valueFinal = new Vector2(1300,0);
            }
            else
            {
                valueStart = new Vector2(0,0);
                valueFinal = new Vector2(-1300,0);

            }
        }
        else
        {
            if (toRight)
            {
                valueStart = new Vector2(-1300,0);
                valueFinal = new Vector2(0,0);
            }
            else
            {
                valueStart = new Vector2(1300,0);
                valueFinal = new Vector2(0,0);
            }
        }

        Tween t = new Tween();
        GetTree().Root.AddChild(t);
        if (toOut)
        {
            t.Connect("tween_all_completed",target,"queue_free");
        }
        t.Connect("tween_all_completed",t,"queue_free");

        t.InterpolateProperty(target,"rect_position",valueStart,valueFinal,.5f,Tween.TransitionType.Linear,Tween.EaseType.InOut);
        t.Start();

    }

}
