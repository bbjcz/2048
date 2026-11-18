using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private enum GameState
    {
        Idle,
        Generating,
        Moving,
        Finished
    }

    private enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public GameObject tilePrefab;

    public float gen4Prob;


    private GameState state = GameState.Moving;
    private int[,] mat = new int[4, 4];
    private Tile[,] tiles = new Tile[4, 4];

    private List<(int, int)> toBeGenerated = new List<(int, int)>();
    private List<Tile> toBeDeleted = new List<Tile>();


    private void GenerateNewTile()
    {
        int emptyCount = 0;
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (mat[r, c] == 0)
                {
                    emptyCount++;
                }
            }
        }
        int pos = Random.Range(0, emptyCount);
        int value = Random.value < gen4Prob ? 4 : 2;

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (mat[r, c] == 0)
                {
                    if (pos == 0)
                    {
                        mat[r, c] = value;
                        toBeGenerated.Add((r, c));
                        return;
                    }
                    pos--;
                }
            }
        }
    }

    private bool AllTilesIdle()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (tiles[r, c] != null && !tiles[r, c].IsIdle())
                {
                    return false;
                }
            }
        }

        foreach (var tile in toBeDeleted)
        {
            if (tile != null && !tile.IsIdle())
            {
                return false;
            }
        }

        return true;
    }

    private bool isFinished()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (mat[r, c] == 0)
                {
                    return false;
                }
                if (r > 0 && mat[r, c] == mat[r - 1, c])
                {
                    return false;
                }
                if (c > 0 && mat[r, c] == mat[r, c - 1])
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool Move(MoveDirection dir)
    {
        int startR, startC;
        int stepR, stepC;
        int bigStepR, bigStepC;

        bool changed = false;

        switch (dir)
        {
            case MoveDirection.Up:
                startR = 0; startC = 0;
                stepR = 1; stepC = 0;
                bigStepR = 0; bigStepC = 1;
                break;
            case MoveDirection.Down:
                startR = 3; startC = 0;
                stepR = -1; stepC = 0;
                bigStepR = 0; bigStepC = 1;
                break;
            case MoveDirection.Left:
                startR = 0; startC = 0;
                stepR = 0; stepC = 1;
                bigStepR = 1; bigStepC = 0;
                break;
            case MoveDirection.Right:
                startR = 0; startC = 3;
                stepR = 0; stepC = -1;
                bigStepR = 1; bigStepC = 0;
                break;
            default:
                startR = 0; startC = 0;
                stepR = 0; stepC = 0;
                bigStepR = 0; bigStepC = 0;
                break;
        }

        for (int i = 0; i < 4; i++)
        {
            int p = 0;
            int lastValue = 0;
            bool canMerge = false;

            for (int j = 0; j < 4; j++)
            {
                int r = startR + i * bigStepR + j * stepR;
                int c = startC + i * bigStepC + j * stepC;
                // Do something with the tile at position (r, c)

                int r2 = startR + i * bigStepR + p * stepR;
                int c2 = startC + i * bigStepC + p * stepC;

                if (mat[r, c] != 0)
                {
                    if (canMerge && mat[r, c] == lastValue)
                    {
                        r2 -= stepR;
                        c2 -= stepC;
                        mat[r2, c2] *= 2;
                        mat[r, c] = 0;
                        tiles[r, c].MoveTo(r2, c2, true);
                        toBeDeleted.Add(tiles[r, c]);
                        toBeDeleted.Add(tiles[r2, c2]);
                        toBeGenerated.Add((r2, c2));
                        tiles[r, c] = null;
                        tiles[r2, c2] = null;
                        canMerge = false;
                        changed = true;
                    }
                    else
                    {
                        lastValue = mat[r, c];
                        if (r2 != r || c2 != c)
                        {
                            mat[r2, c2] = mat[r, c];
                            mat[r, c] = 0;

                            tiles[r, c].MoveTo(r2, c2, false);
                            tiles[r2, c2] = tiles[r, c];
                            tiles[r, c] = null;

                            changed = true;
                        }

                        //if (tiles[r, c] == null)
                        //{
                        //    Debug.LogError($"Tile at ({r}, {c}) is null when trying to move to ({r2}, {c2}).");
                        //    Debug.LogError($"I and j are {i} and {j}, mat: {mat[r2, c2]}");
                        //}

                        
                        canMerge = true;
                        p++;
                    }
                }
            }

            
        }
        if (changed)
        {
            GenerateNewTile();
        }

        return changed;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateNewTile();
        GenerateNewTile();
    }

    void Update()
    {
        if (state == GameState.Moving)
        {
            if (AllTilesIdle())
            {
                foreach (var pos in toBeGenerated)
                {
                    var (r, c) = pos;
                    GameObject tileObj = Instantiate(tilePrefab, transform);
                    tiles[r, c] = tileObj.GetComponent<Tile>();
                    tiles[r, c].BornAt(r, c, mat[r, c]);
                }
                toBeGenerated.Clear();
                state = GameState.Generating;
            }
        }
        else if (state == GameState.Generating)
        {
            if (AllTilesIdle())
            {
                foreach (var tile in toBeDeleted)
                {
                    Destroy(tile.gameObject);
                }
                toBeDeleted.Clear();

                if (isFinished())
                {
                    state = GameState.Finished;
                }
                else
                {
                    state = GameState.Idle;
                    //debug log shows mat
                    //string row = "";
                    //for (int r = 0; r < 4; r++)
                    //{
                    //    for (int c = 0; c < 4; c++)
                    //    {
                    //        row += mat[r, c] + " ";
                    //    }
                    //    row += "\n";
                    //}
                    //Debug.Log(row);
                }
            }
        }
        else if (state == GameState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (Move(MoveDirection.Up))
                {
                    state = GameState.Moving;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (Move(MoveDirection.Down))
                {
                    state = GameState.Moving;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (Move(MoveDirection.Left))
                {
                    state = GameState.Moving;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (Move(MoveDirection.Right))
                {
                    state = GameState.Moving;
                }
            }


        }
    }
}
