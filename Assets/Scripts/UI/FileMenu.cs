using Assets.Scripts.Core;
using Assets.Scripts.UI;
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
        private Text loadLevelTextField;

        // TODO Temporary
        // Note: Lowercase file names only
        private static readonly string filePath = @"/level.csv";

        public void OnRun()
        {
            dialogueMenu.CloseDialogue();
        }

        public void OnSave()
        {
            File.WriteAllText(Application.dataPath + filePath, GridManager.Instance.FormatToCSV());
        }

        public void OnLoad()
        {
            dialogueMenu.CloseDialogue();
            Debug.Log(loadLevelTextField.text.ToLower());

            // TODO Check file exists
            string[] lines = File.ReadAllLines(Application.dataPath + filePath);
            string[] gridSize = lines[0].Split(',');
            GridManager.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]));
            for(int i = 1; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(',');
                GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]));
            }
        }

        public void OnClear()
        {
            dialogueMenu.CloseDialogue();
            GridManager.Instance.ClearGrid();
        }

        public void OnOptions()
        {
            dialogueMenu.CloseDialogue();
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}