using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class DialogManager : MonoBehaviour
{
    public TMP_Text interactionText;
    public TMP_Text dialogueText;
    private Queue<string> dialogueQueue = new Queue<string>();

    void Start()
    {
        dialogueText.text = "";
        dialogueText.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.E) && dialogueText.gameObject.activeSelf)
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(string filePath)
    {
        dialogueQueue.Clear();
        
        dialogueText.gameObject.SetActive(true);
        interactionText.gameObject.SetActive(false);
        
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            dialogueQueue.Enqueue(line);
        }
        
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        StartCoroutine(TypeDialogue(line));
    }

    IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        dialogueText.gameObject.SetActive(false);
    }
}
