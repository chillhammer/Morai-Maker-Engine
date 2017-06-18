using Assets.Scripts.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public enum Dialogue { RunLevel, LoadLevel, ClearLevel, OptionsMenu, LevelName, Exit }

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
        private GameObject exitPrompt;

        private Dialogue? activeDialogue;

        private void Awake()
        {
            DialogueOpened += () => GridPlacement.Instance.CanPlace = false;
            DialogueClosed += () => GridPlacement.Instance.CanPlace = true;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                OpenDialogue(Dialogue.RunLevel);
            }
            if(Input.GetKeyDown(KeyCode.P))
            {
                CloseDialogue();
            }
        }

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

            background.gameObject.SetActive(true);
            switch(dialogue)
            {
                case Dialogue.RunLevel:
                    runLevelPrompt.gameObject.SetActive(true);
                    break;
                case Dialogue.LoadLevel:
                    loadLevelPrompt.gameObject.SetActive(true);
                    break;
                case Dialogue.ClearLevel:
                    clearLevelPrompt.gameObject.SetActive(true);
                    break;
                case Dialogue.OptionsMenu:
                    optionsPrompt.gameObject.SetActive(true);
                    break;
                case Dialogue.LevelName:
                    levelNamePrompt.gameObject.SetActive(true);
                    break;
                case Dialogue.Exit:
                    exitPrompt.gameObject.SetActive(true);
                    break;
            }

            activeDialogue = dialogue;
            DialogueOpened();
        }

        public void CloseDialogue()
        {
            if(!activeDialogue.HasValue)
                return;

            background.gameObject.SetActive(false);
            switch(activeDialogue.Value)
            {
                case Dialogue.RunLevel:
                    runLevelPrompt.gameObject.SetActive(false);
                    break;
                case Dialogue.LoadLevel:
                    loadLevelPrompt.gameObject.SetActive(false);
                    break;
                case Dialogue.ClearLevel:
                    clearLevelPrompt.gameObject.SetActive(false);
                    break;
                case Dialogue.OptionsMenu:
                    optionsPrompt.gameObject.SetActive(false);
                    break;
                case Dialogue.LevelName:
                    levelNamePrompt.gameObject.SetActive(false);
                    break;
                case Dialogue.Exit:
                    exitPrompt.gameObject.SetActive(false);
                    break;
            }

            activeDialogue = null;
            DialogueClosed();
        }
    }
}