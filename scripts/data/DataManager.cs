using System.IO;
using Newtonsoft.Json;

namespace data
{
    public class DataManager
    {
        private static int Save<T>(T data, string path) where T : class
        {
            try
            {

                
                File.WriteAllText(@path,JsonConvert.SerializeObject(data));
                return 0;
                
            }
            catch(System.Exception e)
            {
                Godot.GD.Print("Error saving emulator data: ", e.Message);
                return -1;
            }
        }

        private static T Load<T>(string path) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(@path));
            } catch(System.Exception e)
            {
                Godot.GD.Print("Error loading emulator: ", e.Message);
                return null;
            }
        }
        public static int SaveEmulator(DataEmulator data)
        {
            string path = Software.pathWorkbench+"/emulator_data.json";
            Godot.GD.Print(@path);
            return Save<DataEmulator>(data,path);
        }

        public static DataEmulator LoadEmulator()
        {
            string path = Software.pathWorkbench+"/emulator_data.json";
            return Load<DataEmulator>(path);
        }
    }
}