using Assets.Scripts.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public enum Dialogue { RunLevel, LoadLevel, ClearLevel, OptionsMenu, LevelName, LevelSize, Exit }

    public class DialogueMenu : MonoBehaviour
    {
        public event Action DialogueOpened;
        public event Action DialogueClosed;

        [SerializeField]
        private GameObject background;
        [SerializeField]
        private GameObject runLevelPrompt;
        [SerializeField]
        private GameObject loadLevelPrompt;
        [SerializeField]
        private GameObject clearLevelPrompt;
        [SerializeField]
        private GameObject optionsPrompt;
        [SerializeField]
        private GameObject levelNamePrompt;
        [SerializeField]
        private GameObject levelSizePrompt;
        [SerializeField]
        private GameObject exitPrompt;

        private Dialogue? activeDialogue;

        public bool DialogueActive()
        {
            return activeDialogue.HasValue;
        }

        public void OpenDialogue(int dialogueNumber)
        {
            OpenDialogue((Dialogue)dialogueNumber);
        }

        public void OpenDialogue(Dialogue dialogue)
        {
            if(activeDialogue.HasValue)
                CloseDialogue();

            background.SetActive(true);
            switch(dialogue)
            {
                case Dialogue.RunLevel:
                    runLevelPrompt.SetActive(true);
                    break;
                case Dialogue.LoadLevel:
                    loadLevelPrompt.SetActive(true);
                    break;
                case Dialogue.ClearLevel:
                    clearLevelPrompt.SetActive(true);
                    break;
                case Dialogue.OptionsMenu:
                    optionsPrompt.SetActive(true);
                    break;
                case Dialogue.LevelName:
                    levelNamePrompt.SetActive(true);
                    break;
                case Dialogue.LevelSize:
                    levelSizePrompt.SetActive(true);
                    break;
                case Dialogue.Exit:
                    exitPrompt.SetActive(true);
                    break;
            }

            activeDialogue = dialogue;
            DialogueOpened();
        }

        public void CloseDialogue()
        {
            if(!activeDialogue.HasValue)
                return;

            background.SetActive(false);
            switch(activeDialogue.Value)
            {
                case Dialogue.RunLevel:
                    runLevelPrompt.SetActive(false);
                    break;
                case Dialogue.LoadLevel:
                    loadLevelPrompt.SetActive(false);
                    break;
                case Dialogue.ClearLevel:
                    clearLevelPrompt.SetActive(false);
                    break;
                case Dialogue.OptionsMenu:
                    optionsPrompt.SetActive(false);
                    break;
                case Dialogue.LevelName:
                    levelNamePrompt.SetActive(false);
                    break;
                case Dialogue.LevelSize:
                    levelSizePrompt.SetActive(false);
                    break;
                case Dialogue.Exit:
                    exitPrompt.SetActive(false);
                    break;
            }

            activeDialogue = null;
            DialogueClosed();
        }
    }
}