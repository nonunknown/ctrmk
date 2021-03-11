using System;
using Godot;
using System.Diagnostics;
using data;
class ExecuteProcess
{
    public enum PROGRAM {BIGFILE,EMULATOR_BIOS,EMULATOR,CUSTOM_MODEL,BUILD};
    private enum ERROR {INVALID_PARAM}
    public static string workingDir = "";
    public async static void Main(Node target, PROGRAM pr = PROGRAM.BIGFILE, object data = null) {
        target.GetTree().Paused = true;
        string path = "";
        string arg = "";
        switch(pr) 
        {
            case PROGRAM.BIGFILE:
                    path = Software.pathWorkbench+"/tools_r10/bigtool.exe";
                    arg = Software.pathWorkbench+"/ctr_source/BIGFILE.BIG";
                    GD.Print("Program path: ", path);
                break;
            
            case PROGRAM.EMULATOR:
                    DataEmulator d = (DataEmulator)data;
                    int id = d.selectedID;
                    path = d.pathExe[id];
                    arg = d.pathCue[id];
                    GD.Print("Executing emulator: ", path);
                break;
            case PROGRAM.EMULATOR_BIOS:
                    DataEmulator de = (DataEmulator)data;
                    int _id = de.selectedID;
                    path = de.pathExe[_id];
                    arg = "-load_bios "+de.pathBios[_id] + " " + "-loadbin " + de.pathCue[_id];
                break;
            case PROGRAM.CUSTOM_MODEL:
                    path = Software.pathWorkbench+"/custom_model/model_reader.exe";
                    string _d = (data as string);
                    arg = Software.pathWorkbench+_d;
                    
                break;
            case PROGRAM.BUILD:
                    path = data as string;
                    arg = "";
                break;
            default:
                target.EmitSignal("output", ERROR.INVALID_PARAM.ToString());
                break;
        }
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @path,
                Arguments = @arg,
                UseShellExecute = true,
                RedirectStandardOutput = false,
                CreateNoWindow = false,
                WorkingDirectory = Software.pathWorkbench
            }
        
        };
        proc.Start();
        while(!proc.WaitForExit(500)) {
            
            await target.ToSignal(target.GetTree(), "idle_frame");
        }
        if (proc.ExitCode != 0) {
            target.EmitSignal("output","Error #"+proc.ExitCode.ToString());

        }
        else {
            target.EmitSignal("output","OK");
        }
        target.GetTree().Paused = false;
    }


}
