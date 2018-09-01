using Assets.Scripts.UI;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Generic;
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

        private List<GridObject> gridObjects;

        [SerializeField]
        private float totalDuration = 4;

        #region private members     
        private TcpClient socketConnection;     
        #endregion  

        private string[] allNames = new string[]{"Ground", "Stair", "Treetop", "Block", "Bar", "Koopa", "Koopa 2", "PipeBody", "Pipe", "Question", "Coin", "Goomba", "CannonBody", "Cannon", "Lakitu", "Bridge", "Hard Shell", "SmallCannon", "Plant", "Waves", "Hill", "Castle", "Snow Tree 2", "Cloud 2", "Cloud", "Bush", "Tree 2", "Bush 2", "Tree", "Snow Tree", "Fence", "Bark", "Flag", "Mario"};
        private string[] actions = new string[]{"Ground", "Stair", "Treetop", "Block", "Bar", "Koopa", "Koopa 2", "PipeBody", "Pipe", "Question", "Coin", "Goomba", "CannonBody", "Cannon", "Lakitu", "Bridge", "Hard Shell", "SmallCannon", "Plant", "Waves", "Hill", "Castle", "Snow Tree 2", "Cloud 2", "Cloud", "Bush", "Tree 2", "Bush 2", "Tree", "Snow Tree", "Fence", "Bark", "Nothing"};
        private int startX;

		//private Process process;

        protected override void Awake()
        {
            base.Awake();

            dialogueMenu.DialogueOpened += () => AddLock(dialogueMenu);
            dialogueMenu.DialogueClosed += () => RemoveLock(dialogueMenu);
        }

        public void OnEndTurn()
        {
            gridObjects = new List<GridObject>();
            if(IsLocked)
                return;

            // Block input
            gridPlacement.AddLock(this);

			//Save
			bool saved = fileMenu.ExternalSave();
			LogHandler.Instance.WriteLine ("Ended Turn:  time = "+Time.time);

			//Open Prompt
			dialogueMenu.OpenDialogue(Dialogue.AIThinking);

            // Run model
			if (saved){

                string[] lines = File.ReadAllLines(Constants.directory+ "/StreamingAssets/Levels/"+fileMenu.LevelName+ ".csv");
                string toSend = "";
                bool readRow = false;
                int finalX = int.Parse(lines[lines.Length-1].Split(new char[] {','})[1]);
                startX = finalX-20;

                if(startX<0){
                    startX = 0;
                }

                if(startX+40>=100){
                    startX-=(startX+40)-99;
                }

                foreach(string line in lines){
                    if (readRow){
                        string[] splitLine = line.Split(new char[] {','});
                        int thisX =int.Parse(splitLine[1]);
                        if (thisX>=startX && thisX<startX+40){ 
                            toSend+=(thisX-startX)+"*"+splitLine[2]+"*"+Array.IndexOf(allNames, splitLine[0])+"-";
                        }
                    }
                    else{
                        readRow = true;
                    }
                }
                
                toSend+="|"+startX;
                             
	            if (socketConnection == null) {             
                    socketConnection = new TcpClient("127.0.0.1", 5015);
                }       
                try {
                    // Get a stream object for writing.             
                    NetworkStream stream = socketConnection.GetStream();            
                    if (stream.CanWrite) {
                        // Convert string message to byte array.                 
                        byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(toSend);               
                        // Write byte array to socketConnection stream.                 
                        stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
                    }         
                }       
                catch (SocketException socketException) {             
                    UnityEngine.Debug.Log("Socket exception: " + socketException);         
                }     
	            StartCoroutine(EndTurnCoroutine());
			}
        }

        public void RemoveAIAdditions(){
            if (gridObjects!=null){
                foreach(GridObject g in gridObjects){
                    GridManager.Instance.RemoveGridObject(g.Data.Functional, g.X, g.Y);
                }
                gridObjects = null;
            }
        }

        private IEnumerator EndTurnCoroutine()
        {
			yield return new WaitForSeconds(0.5f);
            /**
            int numAttempts = 0;
            while (!System.IO.File.Exists(Constants.directory+ "/StreamingAssets/Model/additions.csv") && numAttempts<10){
                numAttempts+=1;
                UnityEngine.Debug.Log (numAttempts);
                yield return new WaitForSeconds(0.5f);

            }
            */
            //process.Close();
            string receivedFromServer = "";
            try {           
                Byte[] bytes = new Byte[8112];  
                bool listening = true;           
                while (listening) {              
                    // Get a stream object for reading              
                    using (NetworkStream stream = socketConnection.GetStream()) {                   
                        int length;                     
                        // Read incomming stream into byte arrary.                  
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {                       
                            var incommingData = new byte[length];                       
                            Array.Copy(bytes, 0, incommingData, 0, length);                         
                            // Convert byte array to string message.                        
                            string serverMessage = Encoding.ASCII.GetString(incommingData);                         
                            UnityEngine.Debug.Log("server message received as: " + serverMessage);                  
                            receivedFromServer +=serverMessage; 
                        } 
                        listening =false;              
                    }           
                }         
            }         
            catch (SocketException socketException) {             
                UnityEngine.Debug.Log("Socket exception: " + socketException);         
            }   

            string[] eachAddition = receivedFromServer.Split(new char[] {'*'});

            socketConnection.Close();
            socketConnection = null; 
            float rate = 2.0f/eachAddition.Length;//Was weird to have this change depending on # of additions
            if (eachAddition.Length < 8) {
                rate = 0.3f;
            }

            dialogueMenu.CloseDialogue();
            foreach(string value in eachAddition){
                string[] additionSplitFormat = value.Split(new char[] {','});
                if (additionSplitFormat.Length==4){
                    SpriteData sprite = SpriteManager.Instance.GetSprite(actions[int.Parse(additionSplitFormat[2])]);
                    int spriteX = int.Parse(additionSplitFormat[0])+startX;
                    int spriteY = int.Parse(additionSplitFormat[1]);

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
                    GridObject addition = GridManager.Instance.AddGridObject(sprite, spriteX, spriteY, true);

                    if(addition != null)
                    {
                        gridObjects.Add(addition);
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
            }

            


            // Unblock input
            windowScroll.StopScrolling();
            gridPlacement.RemoveLock(this);
			LogHandler.Instance.WriteLine ("Player Turn:  time = "+Time.time);
            
        }
    }
}