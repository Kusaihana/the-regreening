using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

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
        if (Input.anyKeyDown && dialogueText.gameObject.activeSelf)
        {
            DisplayNextLine();
        }
    }

    IEnumerator LoadDialogue(string filePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filePath);

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string[] lines = www.downloadHandler.text.Split('\n');
                
                foreach (string line in lines)
                {
                    dialogueQueue.Enqueue(line);
                }
            }
        }
        DisplayNextLine();
    }

    public void StartDialogue(string filePath)
    {
        dialogueQueue.Clear();
        
        dialogueText.gameObject.SetActive(true);
        interactionText.gameObject.SetActive(false);
        
        StartCoroutine(LoadDialogue(filePath));
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
