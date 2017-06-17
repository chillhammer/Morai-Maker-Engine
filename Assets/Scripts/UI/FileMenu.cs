using Assets.Scripts.Core;
using System.IO;
using UnityEngine;

public class FileMenu : MonoBehaviour
{
    // TODO Temporary
    private static readonly string filePath = @"/level.csv";

    public void OnSave()
    {
        File.WriteAllText(Application.dataPath + filePath, GridManager.Instance.FormatToCSV());
    }

    public void OnLoad()
    {
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

    public void OnOptions()
    {

    }

    public void OnExit()
    {
        Application.Quit();
    }
}