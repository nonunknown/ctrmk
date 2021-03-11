using Godot;
using System;
using building;

public class CustomRacer : Control
{
    [Signal]
    public delegate void output(string arg);
    const string _out = "/bigfile/models/racers/hi/{0}";
    const string _in = "/custom_model/kart/{0}";
    string character = "";
    RichTextLabel rtl;
    OptionButton ob_char,ob_format;
    LineEdit le_fileName;
    string initial_text = "";
    string outMsg = "";

    public override void _Ready()
    {
        rtl = GetNode<RichTextLabel>("Panel/VBoxContainer/Panel/VBoxContainer/RichTextLabel");
        ob_char = GetNode<OptionButton>("Panel/VBoxContainer/character/ob_char");
        ob_format = GetNode<OptionButton>("Panel/VBoxContainer/format/ob_format");
        le_fileName = GetNode<LineEdit>("Panel/VBoxContainer/output/le_filename");

        initial_text = rtl.BbcodeText;

        Connect("output",this,"OnOutput");
    }
    async void OnCompile()
    {
        character = ob_char.GetItemText(ob_char.GetSelectedId()) + ob_format.GetItemText(ob_format.GetSelectedId());
        string ss = string.Format(_in,character);
        GD.Print("Printing the input file: ", ss);
        ExecuteProcess.Main(this,ExecuteProcess.PROGRAM.CUSTOM_MODEL,ss);

        await ToSignal(this,"output");

        if (outMsg != "OK")
        {
            GD.Print("ERROR EXECUTING CUSTOM MODEL");
            return;
        }

        //TODO: throw the file from user to the custom model reader
        
        //Get the generated file and throw in model reader
        try
        {
            string input = "{0}{1}";
            string output = "{0}";
            input = string.Format(input,Software.pathWorkbench,string.Format(_in,character));
            GD.Print("INPUT: ", input);
            output = Software.pathWorkbench+ string.Format(_out,ob_char.GetItemText(ob_char.GetSelectedId()))+".ctr";
            GD.Print("OUTPUT: ",output);
            System.IO.File.Copy(@input,@output,overwrite: true);
            
        } catch (Exception e )
        {
            GD.Print("Error copying character to bigfile dir: ", e.Message);
            return;
        }

        //TODO: Show the generated model inside godot

        //TODO: if user agree continue

        //TODO: call rebuild script
        if (le_fileName.Text.Length < 3 || le_fileName.Text.Contains(" ") || le_fileName.Text.Contains("."))
        {
            GD.Print("Invalid filename");
            return;
        }

        Rebuild.Build(this, le_fileName.Text);
    
        
        
    }

    void OnOutput(string arg)
    {
        outMsg = arg;
    }

    void OnMetaClicked(string arg)
    {
        arg = arg.Replace("\"","");
        OS.ShellOpen(Software.pathWorkbench+arg);
        GD.Print("META: ", arg, "full: ", Software.pathWorkbench+arg);

    }
}
