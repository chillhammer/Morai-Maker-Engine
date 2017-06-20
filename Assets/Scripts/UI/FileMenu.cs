using Assets.Scripts.Core;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FileMenu : MonoBehaviour
    {
        [SerializeField]
        private DialogueMenu dialogueMenu;
        [SerializeField]
        private OptionsMenu optionsMenu;

        [SerializeField]
        private InputField loadLevelInput;

        public void OnRun()
        {
            // Write level representation to Java file
            File.WriteAllText(Application.dataPath + "/StreamingAssets/src/dk/itu/mario/level/MyLevel.java", GridManager.Instance.FormatToJava());

            // Recompile the simulator
            Process process = new Process();
            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = "run.py";
            process.StartInfo.WorkingDirectory = Application.dataPath + "/StreamingAssets";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
            process.Close();

            // Run the simulator
            process = new Process();
            process.StartInfo.FileName = "java";
            process.StartInfo.Arguments = "-cp bin dk.itu.mario.engine.PlayCustomized";
            process.StartInfo.WorkingDirectory = Application.dataPath + "/StreamingAssets";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.Close();
        }

        public void OnSave()
        {
            string fileName = optionsMenu.LevelName + ".csv";
            File.WriteAllText(Application.dataPath + "/StreamingAssets/" + fileName, GridManager.Instance.FormatToCSV());
        }

        public void OnLoad()
        {
            string newLevelName = loadLevelInput.text.ToLower().Replace(' ', '_');
            string filePath = Application.dataPath + "/StreamingAssets/" + newLevelName + ".csv";

            if(File.Exists(filePath))
            {
                optionsMenu.SetLevelName(newLevelName);
                
                // Parse file
                string[] lines = File.ReadAllLines(filePath);
                string[] gridSize = lines[0].Split(',');
                GridManager.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]));
                for(int i = 1; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]));
                }
            }
            else
            {
                dialogueMenu.OpenDialogue(Dialogue.LoadLevel);
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
    }
}