using Assets.Scripts.UI;
using Assets.Scripts.Util;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class EndTurn : Lockable
    {
        [SerializeField]
        private DialogueMenu dialogueMenu;
        [SerializeField]
        private GridPlacement gridPlacement;
        [SerializeField]
        private CameraScroll windowScroll;

        [SerializeField]
        private float totalDuration = 4;

        protected override void Awake()
        {
            base.Awake();

            dialogueMenu.DialogueOpened += () => AddLock(dialogueMenu);
            dialogueMenu.DialogueClosed += () => RemoveLock(dialogueMenu);
        }

        public void OnEndTurn()
        {
            if(IsLocked)
                return;

            // Block input
            gridPlacement.AddLock(this);

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
            float rate = totalDuration / lines.Length;
            foreach(string line in lines)
            {
                // - Parse values
                string[] values = line.Split(',');
                SpriteData sprite = SpriteManager.Instance.GetSprite(values[0]);
                int spriteX = int.Parse(values[1]);
                int spriteY = int.Parse(values[2]);

                // - Scroll window to addition location
                windowScroll.ScrollOverTime(spriteX + sprite.Width / 2);
                float time = 0;
                while(time < rate * 0.25f)
                {
                    yield return null;

                    if(!IsLocked)
                        time += Time.deltaTime;
                }

                // - Fade addition in
                GridObject addition = GridManager.Instance.AddGridObject(sprite, spriteX, spriteY);
                if(addition != null)
                {
                    addition.SetAlpha(0);
                    time = 0;
                    while(time < rate)
                    {
                        yield return null;

                        if(!IsLocked)
                        {
                            time += Time.deltaTime;
                            addition.SetAlpha((time - 0.25f) / (rate * 0.75f));
                        }
                    }
                }
                else
                {
                    yield return new WaitForSeconds(rate - time);
                }
            }

            // Unblock input
            windowScroll.StopScrolling();
            gridPlacement.RemoveLock(this);
        }
    }
}