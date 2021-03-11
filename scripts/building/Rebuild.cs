using Godot;
using System;
using data;
namespace building
{
    public class Rebuild
    {
        public static string customCuePath = "";
        private static DataEmulator data;
        public async static void Build(Node target, string filename, bool copy = true, bool run = true)
        {
            
            GD.Print("Stating build the bin file");

            //Create the custom build
            string path = Software.pathWorkbench+"/BUILD.BAT";
            string customPath = Software.pathWorkbench+"/CUSTOM_BUILD.BAT";
            System.IO.File.Copy(@path,@customPath, overwrite: true);

            //Set custom output name
            string text = System.IO.File.ReadAllText(customPath);
            text = text.Replace("_out_name_",filename);
            System.IO.File.WriteAllText(customPath,text);


            // int exit_code = OS.Execute(@customPath, new string[] {});
            ExecuteProcess.Main(target,ExecuteProcess.PROGRAM.BUILD,@customPath);
            await target.ToSignal(target,"output");
            int exit_code;
            if (copy)
            {
                exit_code = Copy(filename);
                if (exit_code != 0)
                {
                    GD.Print("ERROR WHEN COPYING INSIDE BUILD()");
                    return;
                }
                
                if (run)
                {
                    exit_code = StartEmulator();
                    return;
                }
                
            }
        }

        private static int Copy(string fileName)
        {
            try
            {
                
                string input = Software.pathWorkbench+string.Format("/{0}.bin",fileName);

                data = DataManager.LoadEmulator();

                string baseOutput = StringExtensions.GetBaseDir(data.pathCue[data.selectedID]); //Get the cue directory from the selected emulator
                
                string output = baseOutput + "/" + fileName + ".bin";

                System.IO.File.Copy(input,output,overwrite: true);

                if (GenerateCueFile(baseOutput, fileName) != 0)
                {
                    GD.Print("GENERATING CUE FROM COPY() error");
                    return -2;
                }
            }
            catch(Exception e )
            {
                GD.Print("ERROR COPYING FILE: ", e.Message);
                return -1;
            }
            return 0;
        }


        private static int GenerateCueFile(string path, string fileName)
        {
            int exit_code = 0;
            try
            {
                string fileText = @"FILE ""_name_.bin"" BINARY
                  TRACK 01 MODE2/2352
                      INDEX 01 00:00:00";
                string output = path+"/"+fileName+".cue";
                customCuePath = output;
                GD.Print("Custom cue dir: ", customCuePath);
                System.IO.File.WriteAllText(output,fileText.Replace("_name_",fileName));
            }
            catch(Exception e)
            {
                GD.Print("ERROR GENERATING CUE: ", e.Message);
                exit_code = -1;
            }
            return exit_code;
        }

        private static int StartEmulator()
        {
            int exit_code = -1;
            bool useBios = data.selectedID == 3; //Hacky way to EPSXE
            string biosPath = data.pathBios[data.selectedID];
            string exePath = data.pathExe[data.selectedID];
            

            if (useBios == false)
            {
                exit_code = OS.Execute(exePath,new string[] {"-bios",biosPath,"-loadbin",customCuePath,"-nogui"});
            }
            else
            {
                exit_code = OS.Execute(exePath, new string[] {customCuePath});
            }

            return exit_code;
        }

         

        //Call rebuild tool

        //Get generated bin file and throw into the le_cue path
    }
}