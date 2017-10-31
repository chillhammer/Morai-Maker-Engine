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
		private FileMenu fileMenu;

        [SerializeField]
        private float totalDuration = 4;

		private Process process;

        protected override void Awake()
        {
            base.Awake();

            dialogueMenu.DialogueOpened += () => AddLock(dialogueMenu);
            dialogueMenu.DialogueClosed += () => RemoveLock(dialogueMenu);
        }

        public void OnEndTurn()
        {
            if(IsLocked())
                return;

            // Save and run model
			if(fileMenu.ExternalSave())
            {
                // Block input
                gridPlacement.AddLock(this);
                AddLock(this);

                // Open prompt
                dialogueMenu.OpenDialogue(Dialogue.AIThinking);

                // Run process
	            process = new Process();
			
	            process.StartInfo.FileName = "python";
				process.StartInfo.Arguments = "runmodel.py ../Levels/" + fileMenu.LevelName + ".csv";
	            process.StartInfo.WorkingDirectory = Application.dataPath + "/StreamingAssets/Model";
	            process.StartInfo.CreateNoWindow = true;
	            process.StartInfo.UseShellExecute = false;

                process.Start();
	            StartCoroutine(EndTurnCoroutine(process));
			}
        }

        private IEnumerator EndTurnCoroutine(Process process)
        {
            yield return new WaitUntil(() => process.HasExited);

            // Close prompt
            dialogueMenu.CloseDialogue();
            
            // Read new additions from file
            string[] lines = File.ReadAllLines(Application.dataPath + "/StreamingAssets/Model/additions.csv");
			float rate = totalDuration / lines.Length; // Was weird to have this change depending on # of additions
			if (lines.Length < 8)
            {
				rate = 0.5f;
			}
				
            foreach(string line in lines)
            {
                // - Parse values
                string[] values = line.Split(',');
                SpriteData sprite = SpriteManager.Instance.GetSprite(values[0]);
                int spriteX = int.Parse(values[1]);
                int spriteY = int.Parse(values[2]);

                if(GridManager.Instance.CanAddGridObject(sprite, spriteX, spriteY))
                {
                    // - Scroll window to addition location
                    windowScroll.ScrollOverTime(spriteX + sprite.Width / 2);
                    float time = 0;
                    while(time < rate * 0.75f)
                    {
                        yield return null;

                        if(!IsLocked(dialogueMenu))
                            time += Time.deltaTime;
                    }

                    // - Fade addition in
                    GridObject addition = GridManager.Instance.AddGridObject(sprite, spriteX, spriteY);
                    addition.SetAlpha(0);
                    while(time < rate)
                    {
                        yield return null;

                        if(!IsLocked(dialogueMenu))
                        {
                            time += Time.deltaTime;
                            addition.SetAlpha((time - rate * 0.75f) / (rate * 0.25f));
                        }
                    }
                    addition.SetAlpha(1);
                }
            }

            // Unblock input
            windowScroll.StopScrolling();
            gridPlacement.RemoveLock(this);
            RemoveLock(this);
        }
    }
}