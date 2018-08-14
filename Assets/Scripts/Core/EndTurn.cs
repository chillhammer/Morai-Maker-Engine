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
            if(IsLocked)
                return;

            // Block input
            gridPlacement.AddLock(this);

			//Save
			bool saved = fileMenu.ExternalSave();

			//Open Prompt
			dialogueMenu.OpenDialogue(Dialogue.AIThinking);

            // Run model
			if (saved){
	            process = new Process();
			
	            process.StartInfo.FileName = "python";
				process.StartInfo.Arguments = "runmodel.py ../Levels/"+fileMenu.LevelName+".csv";
	            process.StartInfo.WorkingDirectory = Application.dataPath + "/StreamingAssets/Model";
	            process.StartInfo.CreateNoWindow = true;
	            process.StartInfo.UseShellExecute = false;
	            StartCoroutine(EndTurnCoroutine(process));
			}
        }

        private IEnumerator EndTurnCoroutine(Process process)
        {
            // Wait until the model is done
            process.Start();

            yield return new WaitWhile(() =>
            {
				foreach(Process temp in Process.GetProcessesByName(process.ProcessName))
                {
						if(process.Id == temp.Id){
                        	return true;
						}
                }
                return false;
            });

			yield return new WaitForSeconds(0.5f);

            process.Close();


			dialogueMenu.CloseDialogue();



            // Read new additions from file
            string[] lines = File.ReadAllLines(Application.dataPath + "/StreamingAssets/Model/additions.csv");
			float rate = 4.0f/lines.Length;//Was weird to have this change depending on # of additions
			if (lines.Length < 8) {
				rate = 0.5f;
			}
				
            foreach(string line in lines)
            {
				//UnityEngine.Debug.Log (line);
                // - Parse values
                string[] values = line.Split(',');
                SpriteData sprite = SpriteManager.Instance.GetSprite(values[0]);
                int spriteX = int.Parse(values[1]);
                int spriteY = int.Parse(values[2]);

                // - Scroll window to addition location
                windowScroll.ScrollOverTime(spriteX + sprite.Width / 2);
                float time = 0;
                while(time < rate * 0.75f)
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
							addition.SetAlpha((time - rate *0.25f) / (rate * 0.25f));
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