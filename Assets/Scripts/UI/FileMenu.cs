using Assets.Scripts.Core;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FileMenu : MonoBehaviour
    {
        private string levelName;
        public string LevelName
        {
            get { return levelName; }
            set { levelName = FormatLevelName(value); }
        }

        [SerializeField]
        private DialogueMenu dialogueMenu;
        [SerializeField]
        private OptionsMenu optionsMenu;

        [SerializeField]
        private InputField loadLevelInput;

        public void OnRun()
        {
            // Write level representation to Java file
            File.WriteAllText(Application.dataPath + "/StreamingAssets/Simulator/src/dk/itu/mario/level/MyLevel.java", GridManager.Instance.FormatToJava());

            // Recompile the simulator
            Process process = new Process();
            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = "run.py";
            process.StartInfo.WorkingDirectory = Application.dataPath + "/StreamingAssets/Simulator";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
            process.Close();

            // Run the simulator
            process = new Process();
            process.StartInfo.FileName = "java";
            process.StartInfo.Arguments = "-cp bin dk.itu.mario.engine.PlayCustomized";
            process.StartInfo.WorkingDirectory = Application.dataPath + "/StreamingAssets/Simulator";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.Close();
        }

        public void OnSave()
        {
            string fileName = LevelName + ".csv";
            File.WriteAllText(Application.dataPath + "/StreamingAssets/Levels/" + fileName, GridManager.Instance.FormatToCSV());
        }

        public void OnLoad()
        {
            // Validate input
            string newLevelName = FormatLevelName(loadLevelInput.text);
            if(newLevelName == null)
                return;

            // Check level exists
            string filePath = Application.dataPath + "/StreamingAssets/Levels/" + newLevelName + ".csv";
            if(File.Exists(filePath))
            {
                LevelName = newLevelName;
                
                // Parse file
                string[] lines = File.ReadAllLines(filePath);
                string[] gridSize = lines[0].Split(',');
                GridManager.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
                for(int i = 1; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]));
                }

                dialogueMenu.CloseDialogue();
            }
        }

        public void OnClear()
        {
            GridManager.Instance.ClearGrid();
        }

        public void OnExit()
        {
            Application.Quit();
        }

        public static string FormatLevelName(string levelName)
        {
            if(levelName == null)
                return null;

            string formattedLevelName = levelName.ToLower().Replace(' ', '_').Trim('_');
            return formattedLevelName == string.Empty ? null : formattedLevelName;
        }
    }
}