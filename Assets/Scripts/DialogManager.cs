using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TMP_Text interactionText;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    private Queue<string> dialogueQueue = new Queue<string>();

    public Image npcIcon;
    public bool isScene1;
    public string scene1FilePath;
    
    public Sprite rattlesSprite;
    public Sprite currentSprite;
    public Sprite bardSprite;
    public Sprite cloverSprite;
    public Sprite cocoSprite;
    public Sprite petrichorSprite;
    public Sprite denprite;

    private bool typing;

    void Start()
    {
        dialogueText.text = "";
        dialoguePanel.gameObject.SetActive(false);
        
        if (isScene1)
        {
            StartDialogue(scene1FilePath);
        }
    }
    
    void Update()
    {
        if (Input.anyKeyDown && dialogueText.gameObject.activeSelf && !typing)
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
        
        dialoguePanel.gameObject.SetActive(true);
        if (interactionText != null) interactionText.gameObject.SetActive(false);

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
        typing = true;
        dialogueText.text = "";
        dialoguePanel.gameObject.SetActive(true);

        npcIcon.enabled = true;
        
        if (line.StartsWith("CLOVER THE DEER"))
        {
            npcIcon.sprite = cloverSprite;
        }
        else if (line.StartsWith("RATTLES"))
        {
            npcIcon.sprite = rattlesSprite;
        }
        else if (line.StartsWith("THE CURRENT"))
        {
            npcIcon.sprite = currentSprite;
        }
        else if (line.StartsWith("PETRICHOR"))
        {
            npcIcon.sprite = petrichorSprite;
        }
        else if (line.StartsWith("BARD"))
        {
            npcIcon.sprite = bardSprite;
        }
        else if (line.StartsWith("COCO"))
        {
            npcIcon.sprite = cocoSprite;
        }
        else if (line.StartsWith("DEN"))
        {
            npcIcon.sprite = denprite;
        }
        else
        {
            npcIcon.enabled = false;
        }

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        typing = false;
    }

    void EndDialogue()
    {
        dialoguePanel.gameObject.SetActive(false);
        
        if (isScene1)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
