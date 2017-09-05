using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public enum Dialogue { RunLevel, SaveFailed, LoadLevel, ClearLevel, OptionsMenu, LevelName, LevelSize, Exit, AIThinking}

    public class DialogueMenu : MonoBehaviour
    {
        public event Action DialogueOpened;
        public event Action DialogueClosed;

        [SerializeField]
        private GameObject background;
        [SerializeField]
        private GameObject[] prompts;

        private int? activeDialogue;

        public bool DialogueActive()
        {
            return activeDialogue.HasValue;
        }

        public void OpenDialogue(Dialogue dialogue)
        {
            OpenDialogue((int)dialogue);
        }

        public void OpenDialogue(int dialogueNumber)
        {
            if(activeDialogue.HasValue)
                CloseDialogue();

            background.SetActive(true);
            prompts[dialogueNumber].SetActive(true);
            activeDialogue = dialogueNumber;
            DialogueOpened();
        }

        public void CloseDialogue()
        {
            if(!activeDialogue.HasValue)
                return;

            background.SetActive(false);
            prompts[activeDialogue.Value].SetActive(false);
            activeDialogue = null;
            DialogueClosed();
        }
    }
}