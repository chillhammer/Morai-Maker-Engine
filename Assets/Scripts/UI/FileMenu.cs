using Assets.Scripts.Core;
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

        }

        public void OnSave()
        {
            string fileName = optionsMenu.LevelName + ".csv";
            File.WriteAllText(Application.dataPath + "/" + fileName, GridManager.Instance.FormatToCSV());
        }

        public void OnLoad()
        {
            string newLevelName = loadLevelInput.text.ToLower().Replace(' ', '_');
            string filePath = Application.dataPath + "/" + newLevelName + ".csv";

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