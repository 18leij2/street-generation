using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class GenerateStreet : MonoBehaviour
{
    public int seed = 0;
    public int deletedTiles = 25;
    public int maxDepth = 7;
    public GameObject ground;
    public GameObject straight;
    public GameObject deadEnd;
    public GameObject intersection;
    public GameObject threeWay;
    public GameObject turn;
    public GameObject building;
    public GameObject house;
    public GameObject tree;

    private int gridSize = 100;
    private bool[,] streetGrid;

    void Start()
    {
        Random.InitState(seed);
        // mark all grids as false when instantiating
        streetGrid = new bool[gridSize, gridSize];

        // gives the rect we work with, the current depth we're at, and wether or not we cut horizontal (0) or vertical (1)
        GenerateStreets(new Rect(0, 0, gridSize, gridSize), maxDepth, 1);
        DeleteStreetEnds();

        for (int i = 0; i < 100; i++)
        {
            // Debug.Log("" + i + ": " + streetGrid[i, 0]);
        }

        PlaceStreets();
        PlaceBuildings();
    }

    // generate all the streets first using recursive subdivision
    void GenerateStreets(Rect area, int depth, int direction)
    {
        // stop if too many subdivisions, or area too small
        if (depth <= 0 || area.width <= 1 || area.height <= 1)
        {
            return;
        }

        // find a random point to cut
        if (direction == 1 && area.width > 3)
        {
            int tempX = 0;
            Rect tempLeft = area;
            Rect tempRight = area;
            
            // find a good value that leaves both sides wide enough (avoid double streets)
            do
            {
                tempX = (int)area.x + ((int)Mathf.Floor(Random.value * area.width));

                tempLeft = new Rect(area.x, area.y, tempX - area.x, area.height);
                tempRight = new Rect(tempX + 1, area.y, area.width - (tempX - area.x) - 1, area.height);
            } while (tempLeft.width < 1 || tempRight.width < 1);

            // int pointX = (int)area.x + ((int)Mathf.Floor(Random.value * area.width));
            int pointX = tempX;

            // mark the whole cross section as a street
            for (int i = (int)area.y; i < (area.y + area.height); i++)
            {
                streetGrid[pointX, i] = true;
                
                // Instantiate(ground, new Vector3(pointX, 0, i), Quaternion.identity);
            }

            // get new areas
            // Rect leftArea = new Rect(area.x, area.y, pointX - area.x, area.height);
            // Rect rightArea = new Rect(pointX + 1, area.y, area.width - (pointX - area.x) - 1, area.height);
            Rect leftArea = tempLeft;
            Rect rightArea = tempRight;

            GenerateStreets(leftArea, depth - 1, 0);
            GenerateStreets(rightArea, depth - 1, 0);
        }
        else if (direction == 0 && area.height > 3)
        {
            int tempY = 0;
            Rect tempTop = area;
            Rect tempBottom = area;

            // find a good value that leaves both sides wide enough (avoid double streets)
            do
            {
                tempY = (int)area.y + ((int)Mathf.Floor(Random.value * area.height));

                tempTop = new Rect(area.x, tempY + 1, area.width, area.height - (tempY - area.y) - 1);
                tempBottom = new Rect(area.x, area.y, area.width, tempY - area.y);
            } while (tempTop.height < 1 || tempBottom.height < 1);

            // int pointY = (int)area.y + ((int)Mathf.Floor(Random.value * area.height));
            int pointY = tempY;

            // mark the whole cross section as a street
            for (int i = (int)area.x; i < (area.x + area.width); i++)
            {
                streetGrid[i, pointY] = true;
                // Instantiate(ground, new Vector3(i, 0, pointY), Quaternion.identity);
            }

            // get new areas
            // Rect topArea = new Rect(area.x, pointY + 1, area.width, area.height - (pointY - area.y) - 1);
            // Rect bottomArea = new Rect(area.x, area.y, area.width, pointY - area.y);
            Rect topArea = tempTop;
            Rect bottomArea = tempBottom;

            GenerateStreets(topArea, depth - 1, 1);
            GenerateStreets(bottomArea, depth - 1, 1);
        }
    }

    // each street end has a chance to get up to 2 tiles cut off (this will also then showcase the dead-end street models)
    // when cutting this off, there is also a chance it will instead delete all the way intil it reaches an intersection, this will help show some turns
    // also pick up to 50 random street tiles to delete across the board
    void DeleteStreetEnds()
    {   
        for (int i = 0; i < gridSize; i++)
        {
            // check all the bottom row
            if (streetGrid[i, 0] == true)
            {
                float tempRand = Random.value;
                if (tempRand > 0.8)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        // check if there are side tiles available
                        if (i - 1 >= 0 && i + 1 < gridSize)
                        {
                            // while we are not at an intersection, continue deleting
                            if (!streetGrid[i - 1, j] && !streetGrid[i + 1, j])
                            {
                                streetGrid[i, j] = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else if (tempRand > 0.7 && tempRand <= 0.8)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        streetGrid[i, j] = false;
                        // Instantiate(straight, new Vector3(i, 0, j), Quaternion.identity);
                    }
                }
                else if (tempRand > 0.6 && tempRand <= 0.7)
                {
                    streetGrid[i, 0] = false;
                    // Instantiate(straight, new Vector3(i, 0, 0), Quaternion.identity);
                }
            }

            // check all the top row
            if (streetGrid[i, 99] == true)
            {
                float tempRand = Random.value;
                if (tempRand > 0.8)
                {
                    for (int j = 99; j >= 0; j--)
                    {
                        // check if there are side tiles available
                        if (i - 1 >= 0 && i + 1 < gridSize)
                        {
                            // while we are not at an intersection, continue deleting
                            if (!streetGrid[i - 1, j] && !streetGrid[i + 1, j])
                            {
                                streetGrid[i, j] = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else if (tempRand > 0.7 && tempRand <= 0.8)
                {
                    for (int j = 99; j > 97; j--)
                    {
                        streetGrid[i, j] = false;
                        // Instantiate(straight, new Vector3(i, 0, j), Quaternion.identity);
                    }
                }
                else if (tempRand > 0.6 && tempRand <= 0.7)
                {
                    streetGrid[i, 99] = false;
                    // Instantiate(straight, new Vector3(i, 0, 99), Quaternion.identity);
                }
            }

            // check all the left row
            if (streetGrid[0, i] == true)
            {
                float tempRand = Random.value;
                if (tempRand > 0.8)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        // check if there are side tiles available
                        if (i - 1 >= 0 && i + 1 < gridSize)
                        {
                            // while we are not at an intersection, continue deleting
                            if (!streetGrid[j, i - 1] && !streetGrid[j, i + 1])
                            {
                                streetGrid[j, i] = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else if (tempRand > 0.7 && tempRand <= 0.8)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        streetGrid[j, i] = false;
                        // Instantiate(straight, new Vector3(j, 0, i), Quaternion.identity);
                    }
                }
                else if (tempRand > 0.6 && tempRand <= 0.7)
                {
                    streetGrid[0, i] = false;
                    // Instantiate(straight, new Vector3(0, 0, i), Quaternion.identity);
                }
            }

            // check all the right row
            if (streetGrid[99, i] == true)
            {
                float tempRand = Random.value;
                if (tempRand > 0.8)
                {
                    for (int j = 99; j >= 0; j--)
                    {
                        // check if there are side tiles available
                        if (i - 1 >= 0 && i + 1 < gridSize)
                        {
                            // while we are not at an intersection, continue deleting
                            if (!streetGrid[j, i - 1] && !streetGrid[j, i + 1])
                            {
                                streetGrid[j, i] = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else if (tempRand > 0.7 && tempRand <= 0.8)
                {
                    for (int j = 99; j > 97; j--)
                    {
                        streetGrid[j, i] = false;
                        // Instantiate(straight, new Vector3(j, 0, i), Quaternion.identity);
                    }
                }
                else if (tempRand > 0.6 && tempRand <= 0.7)
                {
                    streetGrid[99, i] = false;
                    // Instantiate(straight, new Vector3(99, 0, i), Quaternion.identity);
                }
            }
        }

        int tempX = 0;
        int tempY = 0;

        for (int i = 0; i < deletedTiles; i++)
        {
            tempX = ((int)Mathf.Floor(Random.value * 100));
            tempY = ((int)Mathf.Floor(Random.value * 100));

            // Debug.Log(tempX.ToString() + " " + tempY.ToString());

            if (streetGrid[tempX, tempY] == true)
            {
                Debug.Log("Tile Deleted");
                streetGrid[tempX, tempY] = false;
                // Instantiate(straight, new Vector3(tempX, 0, tempY), Quaternion.identity);
            }
        }
    }

    // placed all the streets with template matching
    void PlaceStreets()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                // Debug.Log("Tile " + i + "," + j + ": " + (streetGrid[i, j]).ToString());
                bool up = false;
                bool down = false;
                bool left = false;
                bool right = false;
                int numTrue = 0;

                // check tile above
                if (j + 1 < gridSize)
                {
                    up = streetGrid[i, j + 1];

                    if (up)
                    {
                        numTrue++;
                    }
                }

                // check tile below
                if (j - 1 >= 0)
                {
                    down = streetGrid[i, j - 1];

                    if (down)
                    {
                        numTrue++;
                    }
                }

                // check tile right
                if (i + 1 < gridSize)
                {
                    right = streetGrid[i + 1, j];

                    if (right)
                    {
                        numTrue++;
                    }
                }

                // check tile left
                if (i - 1 >= 0)
                {
                    left = streetGrid[i - 1, j];

                    if (left)
                    {
                        numTrue++;
                    }
                }

                if (streetGrid[i, j] == true)
                {
                    switch (numTrue)
                    {
                        case 4:
                            Instantiate(intersection, new Vector3(i, 0, j), Quaternion.identity);
                            break;
                        case 3:
                            // need to figure out which ones are marked to determine orientation
                            if (left && down && right && !up)
                            {
                                Instantiate(threeWay, new Vector3(i, 0, j), Quaternion.identity);
                            }
                            else if (up && left && down && !right)
                            {
                                Instantiate(threeWay, new Vector3(i, 0, j), Quaternion.Euler(0, 90f, 0));
                            }
                            else if (right && up && left && !down)
                            {
                                Instantiate(threeWay, new Vector3(i, 0, j), Quaternion.Euler(0, 180f, 0));
                            }
                            else if (down && right && up && !left)
                            {
                                Instantiate(threeWay, new Vector3(i, 0, j), Quaternion.Euler(0, 270f, 0));
                            }
                            else
                            {
                                Debug.Log("3 neighbor marked tiles but couldn't place threeway");
                            }
                            break;
                        case 2:
                            // could be straight or a turn
                            if (left && right && !up && !down)
                            {
                                Instantiate(straight, new Vector3(i, 0, j), Quaternion.identity);
                            }
                            else if (up && down && !left && !right)
                            {
                                Instantiate(straight, new Vector3(i, 0, j), Quaternion.Euler(0, 90f, 0));
                            }
                            else if (left && down && !up && !right)
                            {
                                Instantiate(turn, new Vector3(i, 0, j), Quaternion.identity);
                            }
                            else if (left && !down && up && !right)
                            {
                                Instantiate(turn, new Vector3(i, 0, j), Quaternion.Euler(0, 90f, 0));
                            }
                            else if (!left && !down && up && right)
                            {
                                Instantiate(turn, new Vector3(i, 0, j), Quaternion.Euler(0, 180f, 0));
                            }
                            else if (!left && down && !up && right)
                            {
                                Instantiate(turn, new Vector3(i, 0, j), Quaternion.Euler(0, 270f, 0));
                            }
                            else
                            {
                                Debug.Log("2 neighbor marked tiles but couldn't place straight or turn");
                            }
                            break;
                        case 1:
                            // could be any of 4 directions, if on edge don't make deadend
                            if (left && !right && !up && !down)
                            {
                                if (i + 1 >= 100)
                                {
                                    Instantiate(straight, new Vector3(i, 0, j), Quaternion.identity);
                                } 
                                else
                                {
                                    Instantiate(deadEnd, new Vector3(i, 0, j), Quaternion.identity);
                                }               
                            }
                            else if (!left && !down && up && !right)
                            {
                                if (j - 1 < 0)
                                {
                                    Instantiate(straight, new Vector3(i, 0, j), Quaternion.Euler(0, 90f, 0));
                                }
                                else
                                {
                                    Instantiate(deadEnd, new Vector3(i, 0, j), Quaternion.Euler(0, 90f, 0));
                                }          
                            }
                            else if (!left && !down && !up && right)
                            {
                                if (i - 1 < 0)
                                {
                                    Instantiate(straight, new Vector3(i, 0, j), Quaternion.identity);
                                }
                                else
                                {
                                    Instantiate(deadEnd, new Vector3(i, 0, j), Quaternion.Euler(0, 180f, 0));
                                }     
                            }
                            else if (!left && down && !up && !right)
                            {
                                if (j + 1 >= 100)
                                {
                                    Instantiate(straight, new Vector3(i, 0, j), Quaternion.Euler(0, 90f, 0));
                                }
                                else
                                {
                                    Instantiate(deadEnd, new Vector3(i, 0, j), Quaternion.Euler(0, 270f, 0));
                                }
                            }
                            else
                            {
                                Debug.Log("1 neighbor marked tiles but couldn't place deadend");
                            }
                            break;
                        case 0:
                            Instantiate(ground, new Vector3(i, 0, j), Quaternion.identity);
                            Debug.Log("Made a ground tile when it should have not even considered this");
                            break;
                        default:
                            Debug.Log("Error determining number of neighbors");
                            break;
                    }
                
                }
                else
                {
                    Instantiate(ground, new Vector3(i, 0, j), Quaternion.identity);
                }
            }
        }
    }

    // extra buildings and houses placed
    void PlaceBuildings()
    {
        int tempX = 0;
        int tempY = 0;

        // place up to 15 buildings
        for (int i = 0; i < 15; i++)
        {
            tempX = ((int)Mathf.Floor(Random.value * 100));
            tempY = ((int)Mathf.Floor(Random.value * 100));

            // Debug.Log(tempX.ToString() + " " + tempY.ToString());

            if (streetGrid[tempX, tempY] == false)
            {
                Debug.Log("Building placed");
                streetGrid[tempX, tempY] = false;
                Instantiate(building, new Vector3(tempX, 0, tempY), Quaternion.identity);
            }
        }

        // place up to 10 houses
        for (int i = 0; i < 10; i++)
        {
            tempX = ((int)Mathf.Floor(Random.value * 100));
            tempY = ((int)Mathf.Floor(Random.value * 100));

            // Debug.Log(tempX.ToString() + " " + tempY.ToString());

            if (streetGrid[tempX, tempY] == false)
            {
                Debug.Log("House placed");
                streetGrid[tempX, tempY] = false;
                Instantiate(house, new Vector3(tempX, 0, tempY), Quaternion.identity);
            }
        }

        // place up to 30 trees
        for (int i = 0; i < 30; i++)
        {
            tempX = ((int)Mathf.Floor(Random.value * 100));
            tempY = ((int)Mathf.Floor(Random.value * 100));

            // Debug.Log(tempX.ToString() + " " + tempY.ToString());

            if (streetGrid[tempX, tempY] == false)
            {
                Debug.Log("Tree placed");
                streetGrid[tempX, tempY] = false;
                Instantiate(tree, new Vector3(tempX, 0, tempY), Quaternion.identity);
            }
        }
    }
}
