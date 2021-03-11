using Godot;
using System;

public class MainMenu : Control
{
    VBoxContainer boxContainer;
    public override void _Ready()
    {
        boxContainer = GetNode<VBoxContainer>("Panel/VBoxContainer");
        foreach(Button bt in boxContainer.GetChildren())
        {
            bt.Connect("mouse_entered",this,nameof(OnMouseHover),new Godot.Collections.Array() {bt.HintTooltip});
        }
    }

    void OnMouseHover(string text)
    {
        GetNode<RichTextLabel>("info/VBoxContainer/RichTextLabel").BbcodeText = text;
    }

    void OnCustomPressed()
    {
        Transition.instance.ChangeScene(Transition.SCENE.custom_racer);
    }



}
