using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SpawnBlocks : MonoBehaviour
{
    List<GameObject> blockInstantiate = new List<GameObject>();
    public GameObject[] Blocks;
    public TMP_Text[] texts;
    public float time;
    public bool timeActive = false;
    public float points;
    public float highScore;
    public float lines;
    public GameObject[] scenes;
    public GameObject[] canvas;
    public GameObject nextWindow;
    public float fallTimeManager;

    public bool previewDropped;
    public bool lineComplete = false;
    public bool pause;
    public bool classicTetris;

    public Animator blockDelete;
    public bool activeGame = false;

    private GameObject previewBlock;
    private GameObject nextBlock;
    private Vector3 previewBlockPosition = new Vector3(16.5f, 16f, 0f);


    void Awake()
    {
        LoadGame();
    }

    void Start()
    {        
        texts[5].text = highScore.ToString();
        time = 0;
        texts[0].text = time.ToString();
    }

    void Update()
    {
        if (timeActive == true)
        { 
        time += Time.deltaTime;
        texts[0].text = Mathf.Round(time).ToString();
        texts[1].text = lines.ToString();
        texts[2].text = points.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
            Pause();
        }

        if (lineComplete == true)
        {
            PlayAnim();
            lineComplete = false;
        }
    }

    public void NewBlock()
    {
        if (activeGame)
        {
            if (!timeActive)
            {
                //blockInstantiate.Add(Instantiate(Blocks[Random.Range(0, Blocks.Length)], transform.position, Quaternion.identity));
                timeActive = true;
                previewBlock = GameObject.Instantiate(Blocks[Random.Range(0, Blocks.Length)], previewBlockPosition, Quaternion.identity);
                nextBlock = GameObject.Instantiate(Blocks[Random.Range(0, Blocks.Length)], transform.position, Quaternion.identity);
                blockInstantiate.Add(nextBlock);
                previewBlock.GetComponent<TetrisBlock>().enabled = false;
                blockInstantiate.Add(previewBlock);
            }
            else
            {
                nextBlock = previewBlock;
                nextBlock.transform.position = this.transform.position;
                nextBlock.GetComponent<TetrisBlock>().enabled = true;
                blockInstantiate.Add(nextBlock);
                previewBlock = GameObject.Instantiate(Blocks[Random.Range(0, Blocks.Length)], previewBlockPosition, Quaternion.identity);
                previewBlock.GetComponent<TetrisBlock>().enabled = false;
                blockInstantiate.Add(previewBlock);
            }
        }
    }

    public void PlayAnim()
    {
        Debug.Log(lineComplete);
        blockDelete.SetTrigger("Delete");
    }

    public void ClassicButton()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Invoke("ClassicTetris", 1f);
    }

    public void SpeedButton()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Invoke("SpeedTetris", 1f);
    }

    public void ClassicTetris()
    {
        FindObjectOfType<AudioManager>().Play("Track");
        time = 0;
        texts[3].text = "Classic";
        classicTetris = true;
        activeGame = true;
        scenes[0].SetActive(false);
        scenes[1].SetActive(true);
        NewBlock();
        fallTimeManager = 0.8f;
    }

    public void SpeedTetris()
    {
        FindObjectOfType<AudioManager>().Play("Track");
        time = 0;
        texts[3].text = "Speed";
        classicTetris = false;
        activeGame = true;
        scenes[0].SetActive(false);
        scenes[1].SetActive(true);
        NewBlock();
        fallTimeManager = 0.3f;
    }

    public void MainMenu()
    {
        FindObjectOfType<AudioManager>().Stop("Track");
        FindObjectOfType<AudioManager>().Play("Button");
        lines = 0;
        points = 0;
        ResumeGame();
        for (int i = 0; i < blockInstantiate.Count; i++)
        {
            Destroy(blockInstantiate[i]);
        }
        blockInstantiate.Clear();
        scenes[1].SetActive(false);
        scenes[0].SetActive(true);
        canvas[0].SetActive(false);
        canvas[1].SetActive(false);
        canvas[2].SetActive(true);
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        if (pause)
        {
            Time.timeScale = 0f;
            canvas[1].SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            canvas[1].SetActive(false);
        }        
    }

    public void ResumeGame()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Time.timeScale = 1f;
        canvas[1].SetActive(false);
        pause = !pause;
    }

    public void GameOver()
    {
        SaveGame();
        if(highScore < points)
            highScore = points;
        Debug.Log("GameOver");
        activeGame = false;
        texts[4].text = points.ToString();
        canvas[0].SetActive(true);
        canvas[2].SetActive(false);
    }

    public void ExitGame()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Application.Quit();
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }

    public void SaveGame()
    {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        Debug.Log(data.highScoreSaved);
        highScore = data.highScoreSaved;
    }
}
