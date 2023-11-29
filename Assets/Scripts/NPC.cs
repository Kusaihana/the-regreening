using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string dialogueFilePath;
    public DialogManager dialogManager;
    public bool inConversation;
    public bool canTalk;
    
    void Update()
    {
        if (canTalk && !inConversation && Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
            inConversation = true;
        }
    }

    void Interact()
    {
        dialogManager.StartDialogue(dialogueFilePath);
    }
    
}
