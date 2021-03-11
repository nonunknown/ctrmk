using Godot;
using System;
using data;
public class Software : Control
{
    public static string pathWorkbench = "";
    bool release_debug = true;
    public override void _EnterTree()
    {
        if (!release_debug) 
        {
            pathWorkbench = @ProjectSettings.GlobalizePath("user://workbench");
        }
        else 
        {
            pathWorkbench = @OS.GetExecutablePath().GetBaseDir()+"/workbench";
        }
        
    }
}
