using Assets.Scripts.UI;
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
        [SerializeField]
        private CameraScroll windowScroll;

        private static readonly float TOTAL_DURATION = 3f;

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
            float rate = 6f / lines.Length;
            foreach(string line in lines)
            {
                string[] values = line.Split(',');
                SpriteData sprite = SpriteManager.Instance.GetSprite(values[0]);
                int spriteX = int.Parse(values[1]);
                int spriteY = int.Parse(values[2]);

                windowScroll.ScrollOverTime(spriteX + sprite.Width / 2);

                yield return new WaitForSeconds(rate * 0.2f);

                GridObject temp = GridManager.Instance.AddGridObject(sprite, spriteX, spriteY);
                if(temp != null)
                {
                    temp.SetAlpha(0);
                    temp.SetAlphaOverTime(1, rate * 0.8f);
                }

                yield return new WaitForSeconds(rate * 0.8f);
            }

            // TODO Unblock input
            windowScroll.StopScrolling();
        }
    }
}