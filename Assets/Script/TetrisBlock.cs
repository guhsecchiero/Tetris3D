using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime; 
    public static int height = 20;
    public static int width = 10;
    public static Transform[,] grid = new Transform[width, height];
    public bool gameOver = false;


    void Start()
    {
        fallTime = FindObjectOfType<SpawnBlocks>().fallTimeManager;
    }

    void Update()
    {        

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if(!ValidMove())
                transform.position -= new Vector3(-1, 0, 0);
        } 
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!ValidMove())
                transform.position -= new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            if (!ValidMove())
            {
                foreach (Transform children in transform)
                {
                    int roundedX = Mathf.RoundToInt(children.transform.position.x);
                    int roundedY = Mathf.RoundToInt(children.transform.position.y);

                    if (roundedX >= width || roundedY < 0)
                    {
                        transform.Rotate(new Vector3(0, 0, 0), 90f);
                        transform.position -= new Vector3(1, 0, 0);
                    }
                    if (roundedX <= 0 || roundedY < 0)
                    {                        
                        transform.Rotate(new Vector3(0, 0, 0), 90f);
                        transform.position -= new Vector3(-1, 0, 0);
                    }
                }
            }
        }

        if (gameOver == false)
        {
            if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
            {
                transform.position += new Vector3(0, -1, 0);
                if (!ValidMove())
                {
                    transform.position -= new Vector3(0, -1, 0);
                    AddToGrid();
                    CheckForLines();
                    this.enabled = false;
                    FindObjectOfType<SpawnBlocks>().NewBlock();
                    FindObjectOfType<SpawnBlocks>().previewDropped = true;
                }
                previousTime = Time.time;
            }
        }

        if (gameOver == true)
        {
            FindObjectOfType<SpawnBlocks>().GameOver();
            fallTime = 0f;
            StopCoroutine("NewBlock");
        }
    }

    void CheckForLines()
    {        
        for (int i = height - 1 ; i >= 0; i--)
        {
            if(HasLine(i))
            {                
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i)
    {        
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }
        return true;
    }

    void DeleteLine(int i)
    {
        FindObjectOfType<SpawnBlocks>().lineComplete = true;
        FindObjectOfType<AudioManager>().Play("DeleteLine");
        FindObjectOfType<SpawnBlocks>().points += 100;
        FindObjectOfType<SpawnBlocks>().lines += 1;
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j,i].gameObject);
            grid[j, i] = null;
        }       
    }

    void RowDown(int i)
    {
        for(int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if(grid[j,y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;            

            if (roundedY == 18)
                gameOver = true;
        }
    }

    public bool ValidMove()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);
            

            if(roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
                return false;

        }

        return true;
    }
}
