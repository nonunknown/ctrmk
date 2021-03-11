using Godot;
using System;
using data;
public class EmulatorSettings : Control
{

    enum EMULATOR {None,NoCash,BizHawk,DuckStation,Epsxe}
    EMULATOR selected_emulator;
    DataEmulator dataEmulator = new DataEmulator();

    LineEdit le_exe,le_bios,le_cue;
    Button bt_open_exe,bt_open_bios,bt_open_cue,bt_cancel,bt_test,bt_done;
    OptionButton ob_option;
    CheckBox cb_skip;

    FileDialog fileDialog;
    public override void _Ready()
    {
        
        cb_skip = GetNode<CheckBox>("Panel/VBoxContainer/hb_cue3/CheckBox");
		GD.Print("Started Emulator settings Scene");
        selected_emulator = EMULATOR.None;
        fileDialog = GetNode<FileDialog>("FileDialog");

        le_bios = GetNode<LineEdit>("Panel/VBoxContainer/hb_bios/le_bios");
        le_exe = GetNode<LineEdit>("Panel/VBoxContainer/hb_exe/le_exe");
        le_cue = GetNode<LineEdit>("Panel/VBoxContainer/hb_cue/le_cue");

        bt_open_bios = GetNode<Button>("Panel/VBoxContainer/hb_bios/bt_open_bios");
        bt_open_cue = GetNode<Button>("Panel/VBoxContainer/hb_cue/bt_open_cue");
        bt_open_exe = GetNode<Button>("Panel/VBoxContainer/hb_exe/bt_open_exe");

        bt_cancel = GetNode<Button>("Panel/VBoxContainer/hb_buttons/bt_cancel");
        bt_test = GetNode<Button>("Panel/VBoxContainer/hb_buttons/bt_test");
        bt_done = GetNode<Button>("Panel/VBoxContainer/hb_buttons/bt_done");

        ob_option = GetNode<OptionButton>("Panel/VBoxContainer/hb_emulator/ob_emulator");

        bt_cancel.Visible = false;
        
        dataEmulator = DataManager.LoadEmulator();

        if (dataEmulator != null)
        {
            int id = dataEmulator.selectedID;
            ob_option.Select(id+1);
            ob_option.EmitSignal("item_selected",id+1);
            cb_skip.Pressed = dataEmulator.skipWindow;
            GD.Print("loaded data from emulator");
        }
        else
        {
            dataEmulator = new DataEmulator();
            GD.Print("create a new data for emulator");
        }

        Connect("output",this,"OnOutput");

    }

    void OnOBSelected(int item)
    {
        dataEmulator.selectedID = item-1;
        if ( (int)EMULATOR.None == item )
        {
            foreach(Button bt in GetNode("Panel/VBoxContainer/hb_buttons").GetChildren())
            {
                bt.Disabled = true;
            }
            bt_cancel.Disabled = bt_test.Disabled = true;
        }
        else
        {
            bt_cancel.Disabled = bt_test.Disabled = false;
            dataEmulator.selectedID = item-1;
            le_exe.Text = dataEmulator.pathExe[item-1];
            le_bios.Text = dataEmulator.pathBios[item-1];
            le_cue.Text = dataEmulator.pathCue[item-1];
            GD.Print("Filled");


        }
        switch(item)
        {
            case (int)EMULATOR.None:
                bt_open_bios.Disabled = true;
                bt_open_cue.Disabled = true;
                bt_open_exe.Disabled = true;
                break;
            
            case (int)EMULATOR.NoCash:
                bt_open_cue.Disabled = false;
                bt_open_exe.Disabled = false;
                bt_open_bios.Disabled = true;
                break;
            
            case (int)EMULATOR.BizHawk:
                bt_open_cue.Disabled = false;
                bt_open_exe.Disabled = false;
                bt_open_bios.Disabled = true;
                break;
            
            case (int)EMULATOR.DuckStation:
                bt_open_cue.Disabled = false;
                bt_open_exe.Disabled = false;
                bt_open_bios.Disabled = true;
                break;
            
            case (int)EMULATOR.Epsxe:
                bt_open_cue.Disabled = false;
                bt_open_exe.Disabled = false;
                bt_open_bios.Disabled = false;
                dataEmulator.useBios[item-1] = true;
                break;
        }

        le_bios.Visible = !bt_open_bios.Disabled;
    }

    void OnBtExePressed()
    {
        fileDialog.Filters = new string[] {"*.exe"};
        fileDialog.Show();
    }

    void OnBtBiosPressed()
    {
        fileDialog.Filters = new string[] {"*.bin"};
        fileDialog.Show();
       
    }

    void OnBtCuePressed()
    {
        fileDialog.Filters = new string[] {"*.cue"};
        fileDialog.Show();
        
    }

    void OnFileSelected(string path)
    {
        switch(fileDialog.Filters[0])
        {
            case "*.exe":
                le_exe.Text = path;
                dataEmulator.pathExe[dataEmulator.selectedID] = le_exe.Text;
                break;
            
            case "*.cue":
                le_cue.Text = path;
                dataEmulator.pathCue[dataEmulator.selectedID] = le_cue.Text;
                break;
            
            case "*.bin":
                le_bios.Text = path;
                dataEmulator.pathBios[dataEmulator.selectedID] = le_bios.Text;
                break;
        }
    }

    void OnBtDonePressed() 
    {
        DataManager.SaveEmulator(dataEmulator);
        Transition.instance.ChangeScene(Transition.SCENE.main_menu);
    }

    void OnCBNextTime(bool pressed)
    {
        dataEmulator.skipWindow = pressed;
    }

    void OnTestPressed()
    {
        //OS.execute(les[TYPE.EMULATOR].text,["-bios",les[TYPE.BIOS].text,"-loadbin",les[TYPE.ISO].text,"-nogui"])
        int exit_code = -1;

        if (bt_open_bios.Disabled == false)
        {
            exit_code = OS.Execute(le_exe.Text,new string[] {"-bios",le_bios.Text,"-loadbin",le_cue.Text,"-nogui"});
        }
        else
        {
            exit_code = OS.Execute(le_exe.Text, new string[] {le_cue.Text});
        }
        

        if (exit_code != 0) {
            GD.Print("EXIT CODE: ", exit_code);
            return;
        }

        bt_done.Disabled = false;

        // if (bt_open_bios.Disabled == true)
        // {
        //     ExecuteProcess.Main(this,ExecuteProcess.PROGRAM.EMULATOR,dataEmulator);
        // }
        // else //Assume it is EPSXE
        // {
        //     ExecuteProcess.Main(this,ExecuteProcess.PROGRAM.EMULATOR_BIOS,dataEmulator);
        // }

    }

    void OnOutput(string data)
    {
        if (data == "OK")
        {
            bt_done.Disabled = false;
        }
        else
        {
            GD.Print("Error: ", data);
        }
    }

    [Signal]
    public delegate void output(string arg);
}
