using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string dialogueFilePath;
    public DialogManager dialogManager;
    public bool inConversation;
    
    void Update()
    {
        if (inConversation && Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    void Interact()
    {
        dialogManager.StartDialogue(dialogueFilePath);
    }
    
}
