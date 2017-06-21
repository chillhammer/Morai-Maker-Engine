using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class EndTurn : Singleton<EndTurn>
    {
        public void OnEndTurn()
        {
            // TODO Block input

            // Run model
            Process process = new Process();
            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = "model.py ../Levels/level.csv";
            process.StartInfo.WorkingDirectory = Application.dataPath + "/StreamingAssets/Model";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            StartCoroutine(EndTurnCoroutine(process));
        }

        private IEnumerator EndTurnCoroutine(Process process)
        {
            // Wait until the model is done
            bool running = true;
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => running = false;
            process.Start();
            yield return new WaitWhile(() => running);
            process.Close();

            // Read new additions from file
            string[] lines = File.ReadAllLines(Application.dataPath + "/StreamingAssets/Model/additions.csv");
            foreach(string line in lines)
            {
                string[] values = line.Split(',');
                GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(values[0]), int.Parse(values[1]), int.Parse(values[2]));
            }
        }
    }
}