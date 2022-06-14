using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class SpwanMushrooms : MonoBehaviour
{
    GameObject mushroomToCheck;
    struct mushroomToFall
    {
        public GameObject mushroom;
        public Vector3 position;
    }
    bool isCoroutined = false;
    List<mushroomToFall> mushroomsToFall;
    private bool isFalled = false;
    private bool IsAvailable = true;
    List<bool> canBeGeneratedList;
    Vector3 pos;
    public float CooldownDuration = 0.1f;
    Vector2 gridSize;
    GameObject mushroomToInstantiate;
    GameObject[] notAllowrdMushrooms = new GameObject[2];
    GameObject currentMushroom;
    GameObject nextMushroom;
    GameObject tempMushroom;
    private int counter;
    GameObject[] mushroomsToInstantiate;
    [SerializeField] private GameObject[] mushrooms;
    private GameObject[][] gridOfMushrooms;
    public GameObject[][] gridOfSceneMushrooms;
    int[][] potentialMushroomsIndexes;
    int[] currentMushroomsIndex = new int[2];
    int[] cloneCurrentMushroomsIndex = new int[2];
    int[] nextMushroomsIndex = new int[2];
    int[] cloneNextMushroomsIndex = new int[2];
    float step;
    bool isDestroyed = false;
    bool canBeGenerated = true;

    List<Vector2Int> indicesToRemove;


    private bool isVerticalCheck = false;
    private int indexOTheFirstMushroomX;
    private int indexOTheFirstMushroomY;
    private int indexOTheSecondMushroomX;
    private int indexOTheSecondMushroomY;
    Vector3 positionOfTheFirstMushroom;
    Vector3 positionOfTheSecondMushroom;
    [SerializeField] LayerMask clickablleLayer;
    GameObject[] pairOfMushrooms = new GameObject[2] { null, null };


    // Start is called before the first frame update
    public int gap = 5;
    void Start()
    {
        notAllowrdMushrooms[0] = mushrooms[Random.Range(0, 3)];
        notAllowrdMushrooms[1] = mushrooms[Random.Range(0, 3)];
        GenerateRandomMushrooms();

        indicesToRemove = new List<Vector2Int>();
        canBeGeneratedList = new List<bool>();
        mushroomsToFall = new List<mushroomToFall>();



    }
    
    void Update()
    {
        
        FallMushrooms();
        
        if (mushroomsToFall.Count > 0)
        {
            for (int i = 0; i < mushroomsToFall.Count; i++)
            {

                for (int j = i + 1; j < mushroomsToFall.Count; j++)
                {
                    if (mushroomsToFall[i].mushroom == mushroomsToFall[j].mushroom)
                    {
                        mushroomsToFall.RemoveAt(i);
                    }
                }
            }

            foreach (mushroomToFall msh in mushroomsToFall)
            {
                if (msh.mushroom.transform.position != msh.position)
                    msh.mushroom.transform.position = Vector2.MoveTowards(msh.mushroom.transform.position, msh.position, step);
            }
            
        }

        if (canBeGenerated)
        {
            GenerateRandomMushrooms1();
            for (int i = 0; i < gridOfMushrooms.Length; i++)
            {
                if (!gridOfMushrooms[i].Contains(null))
                {
                    DestroyAllObjects();
                }
            }
            
            canBeGenerated = false;
            

        }
        




        MouseInput();
        if (!pairOfMushrooms.Contains(null))
        {

            if (Mathf.Round(Vector2.Distance(positionOfTheFirstMushroom, positionOfTheSecondMushroom)) == gap)
            {
                
                step = Time.deltaTime * 10.0f;

                indexOTheFirstMushroomX = (int)Mathf.Round(positionOfTheFirstMushroom.x) / gap;
                indexOTheFirstMushroomY = (int)Mathf.Round(positionOfTheFirstMushroom.y) / gap;
                indexOTheSecondMushroomX = (int)Mathf.Round(positionOfTheSecondMushroom.x) / gap;
                indexOTheSecondMushroomY = (int)Mathf.Round(positionOfTheSecondMushroom.y) / gap;

                pairOfMushrooms[0].transform.position = Vector2.MoveTowards(pairOfMushrooms[0].transform.position, positionOfTheSecondMushroom, step);


                pairOfMushrooms[1].transform.position = Vector2.MoveTowards(pairOfMushrooms[1].transform.position, positionOfTheFirstMushroom, step);

                if (pairOfMushrooms[0].transform.position == positionOfTheSecondMushroom && pairOfMushrooms[1].transform.position == positionOfTheFirstMushroom)
                {

                    pairOfMushrooms = new GameObject[2] { null, null };

                    tempMushroom = gridOfMushrooms[indexOTheFirstMushroomX][indexOTheFirstMushroomY];

                    gridOfMushrooms[indexOTheFirstMushroomX][indexOTheFirstMushroomY] = gridOfMushrooms[indexOTheSecondMushroomX][indexOTheSecondMushroomY];

                    gridOfMushrooms[indexOTheSecondMushroomX][indexOTheSecondMushroomY] = tempMushroom;

                    DestroyAllObjects();



                }
                //
                
                //isDestroyed = true;
                //Sequence();





            }
            else
            {
                pairOfMushrooms = new GameObject[2] { null, null };
            }
            

        }

        
        

    }
    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //FallMushrooms();
            Vector3 raycastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(raycastPosition.x, raycastPosition.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (pairOfMushrooms[0] == null)
            {
                pairOfMushrooms[0] = hit.collider.gameObject;

                positionOfTheFirstMushroom = pairOfMushrooms[0].transform.position;
                Debug.Log(gridOfMushrooms[(int)Mathf.Round(positionOfTheFirstMushroom.x) / gap][(int)Mathf.Round(positionOfTheFirstMushroom.y / gap)]);




            }
            else
            {
                pairOfMushrooms[1] = hit.collider.gameObject;
                positionOfTheSecondMushroom = pairOfMushrooms[1].transform.position;


            }
            //Debug.Log(Vector2.Distance(positionOfTheFirstMushroom, positionOfTheSecondMushroom));
            //Debug.Log(positionOfTheFirstMushroom[0] + " " + positionOfTheFirstMushroom[1]);
            //Debug.Log(positionOfTheSecondMushroom[0] + " " + positionOfTheSecondMushroom[1]);
            //Debug.Log((Mathf.Round(Mathf.Abs(positionOfTheFirstMushroom[0] - positionOfTheSecondMushroom[0]))) == 4);
            //Debug.Log(Mathf.Abs(positionOfTheSecondMushroom[1] - positionOfTheFirstMushroom[1]));


            /*
            pairOfMushrooms[1] = hit.collider.gameObject;
            Vector2 pos = pairOfMushrooms[0].transform.position;
            Debug.Log(pos);
            pairOfMushrooms[0].transform.position = pairOfMushrooms[1].transform.position;

            Debug.Log(pos);
            pairOfMushrooms[1].transform.position = pos;
            */

        }

    }
    void DestroyVerticalMushrooms()
    {
        counter = 0;
        potentialMushroomsIndexes = new int[(int)gridSize.x][];
        for (int i = 0; i < gridOfMushrooms.Length; i++)
        {
           
            currentMushroomsIndex[0] = i;
            currentMushroomsIndex[1] = 0;

            counter = 0;


            for (int j = 1; j < gridOfMushrooms[0].Length; j++)
            {
                
                currentMushroom = gridOfMushrooms[currentMushroomsIndex[0]][currentMushroomsIndex[1]];

                
                nextMushroom = gridOfMushrooms[i][j];
                nextMushroomsIndex[0] = i;
                nextMushroomsIndex[1] = j;


                if (currentMushroom.tag == nextMushroom.tag)
                {
                    cloneNextMushroomsIndex = (int[])nextMushroomsIndex.Clone();
                    cloneCurrentMushroomsIndex = (int[])currentMushroomsIndex.Clone();

                    potentialMushroomsIndexes[counter] = cloneCurrentMushroomsIndex;
                    potentialMushroomsIndexes[counter + 1] = cloneNextMushroomsIndex;

                    counter++;
                }
                else if (counter >= 2)
                {
                    counter = 0;
                    for (int x = 0; x < potentialMushroomsIndexes.Length; x++)
                    {
                        if (potentialMushroomsIndexes[x] == null)
                        {
                            break;
                        }

                        indicesToRemove.Add(new Vector2Int(potentialMushroomsIndexes[x][0], potentialMushroomsIndexes[x][1]));
                    }
                    Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
                }
                else
                {
                    counter = 0;
                    Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
                }

                currentMushroomsIndex[1] = j;
             
            }
            if (counter >= 2)
            {
                counter = 0;
                for (int x = 0; x < potentialMushroomsIndexes.Length; x++)
                {
                    if (potentialMushroomsIndexes[x] == null)
                    {
                        break;
                    }

                    indicesToRemove.Add(new Vector2Int(potentialMushroomsIndexes[x][0], potentialMushroomsIndexes[x][1]));
                }
                Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
            }
            else
            {
                counter = 0;
                Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
            }
        }
    }


    void DestroyHorizontalMushrooms()
    {
        counter = 0;
        potentialMushroomsIndexes = new int[(int)gridSize.x][];
        for (int i = 0; i < gridOfMushrooms[0].Length; i++)
        {
            
            currentMushroomsIndex[0] = 0;
            currentMushroomsIndex[1] = i;


            counter = 0;

            for (int j = 1; j < gridOfMushrooms.Length; j++)
            {

                currentMushroom = gridOfMushrooms[currentMushroomsIndex[0]][currentMushroomsIndex[1]];

                nextMushroom = gridOfMushrooms[j][i];
                nextMushroomsIndex[0] = j;
                nextMushroomsIndex[1] = i;
                
                if (currentMushroom.tag == nextMushroom.tag)
                {
                    cloneNextMushroomsIndex = (int[])nextMushroomsIndex.Clone();
                    cloneCurrentMushroomsIndex = (int[])currentMushroomsIndex.Clone();

                    potentialMushroomsIndexes[counter] = cloneCurrentMushroomsIndex;
                    potentialMushroomsIndexes[counter + 1] = cloneNextMushroomsIndex;

                    counter++;
                }
                else if (counter >= 2)
                {
                    counter = 0;
                    for (int x = 0; x < potentialMushroomsIndexes.Length; x++)
                    {
                        if (potentialMushroomsIndexes[x] == null)
                        {
                            break;
                        }

                        indicesToRemove.Add(new Vector2Int(potentialMushroomsIndexes[x][0], potentialMushroomsIndexes[x][1]));
                    }
                    Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
                }
                else
                {
                    counter = 0;
                    Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
                }
                currentMushroomsIndex[0] = j;
            }

            if (counter >= 2)
            {
                counter = 0;
                for (int x = 0; x < potentialMushroomsIndexes.Length; x++)
                {
                    if (potentialMushroomsIndexes[x] == null)
                    {
                        break;
                    }

                    indicesToRemove.Add(new Vector2Int(potentialMushroomsIndexes[x][0], potentialMushroomsIndexes[x][1]));
                }
                Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
            }
            else
            {
                counter = 0;
                Array.Clear(potentialMushroomsIndexes, 0, potentialMushroomsIndexes.Length);
            }
        }

    }

    void GenerateMushrooms()
    {
        gridSize = new Vector2(6, 6);
        gridOfMushrooms = new GameObject[(int)gridSize.x][];
        for (int x = 0; x < gridSize.x; x++)
        {
            gridOfMushrooms[x] = new GameObject[(int)gridSize.y];
        }
        // blue 0
        // red 1
        // green 2
        gridOfMushrooms[0][0] = Instantiate(mushrooms[1], new Vector3(0 * gap, 0 * gap, 0), Quaternion.identity);
        gridOfMushrooms[0][1] = Instantiate(mushrooms[2], new Vector3(0 * gap, 1 * gap, 0), Quaternion.identity);
        gridOfMushrooms[0][2] = Instantiate(mushrooms[0], new Vector3(0 * gap, 2 * gap, 0), Quaternion.identity);
        gridOfMushrooms[0][3] = Instantiate(mushrooms[2], new Vector3(0 * gap, 3 * gap, 0), Quaternion.identity);
        gridOfMushrooms[0][4] = Instantiate(mushrooms[0], new Vector3(0 * gap, 4 * gap, 0), Quaternion.identity);
        gridOfMushrooms[0][5] = Instantiate(mushrooms[1], new Vector3(0 * gap, 5 * gap, 0), Quaternion.identity);
        gridOfMushrooms[1][0] = Instantiate(mushrooms[2], new Vector3(1 * gap, 0 * gap, 0), Quaternion.identity);
        gridOfMushrooms[1][1] = Instantiate(mushrooms[1], new Vector3(1 * gap, 1 * gap, 0), Quaternion.identity);
        gridOfMushrooms[1][2] = Instantiate(mushrooms[1], new Vector3(1 * gap, 2 * gap, 0), Quaternion.identity);
        gridOfMushrooms[1][3] = Instantiate(mushrooms[2], new Vector3(1 * gap, 3 * gap, 0), Quaternion.identity);
        gridOfMushrooms[1][4] = Instantiate(mushrooms[1], new Vector3(1 * gap, 4 * gap, 0), Quaternion.identity);
        gridOfMushrooms[1][5] = Instantiate(mushrooms[0], new Vector3(1 * gap, 5 * gap, 0), Quaternion.identity);
        gridOfMushrooms[2][0] = Instantiate(mushrooms[0], new Vector3(2 * gap, 0 * gap, 0), Quaternion.identity);
        gridOfMushrooms[2][1] = Instantiate(mushrooms[2], new Vector3(2 * gap, 1 * gap, 0), Quaternion.identity);
        gridOfMushrooms[2][2] = Instantiate(mushrooms[0], new Vector3(2 * gap, 2 * gap, 0), Quaternion.identity);
        gridOfMushrooms[2][3] = Instantiate(mushrooms[1], new Vector3(2 * gap, 3 * gap, 0), Quaternion.identity);
        gridOfMushrooms[2][4] = Instantiate(mushrooms[0], new Vector3(2 * gap, 4 * gap, 0), Quaternion.identity);
        gridOfMushrooms[2][5] = Instantiate(mushrooms[2], new Vector3(2 * gap, 5 * gap, 0), Quaternion.identity);
        gridOfMushrooms[3][0] = Instantiate(mushrooms[2], new Vector3(3 * gap, 0 * gap, 0), Quaternion.identity);
        gridOfMushrooms[3][1] = Instantiate(mushrooms[0], new Vector3(3 * gap, 1 * gap, 0), Quaternion.identity);
        gridOfMushrooms[3][2] = Instantiate(mushrooms[2], new Vector3(3 * gap, 2 * gap, 0), Quaternion.identity);
        gridOfMushrooms[3][3] = Instantiate(mushrooms[0], new Vector3(3 * gap, 3 * gap, 0), Quaternion.identity);
        gridOfMushrooms[3][4] = Instantiate(mushrooms[2], new Vector3(3 * gap, 4 * gap, 0), Quaternion.identity);
        gridOfMushrooms[3][5] = Instantiate(mushrooms[0], new Vector3(3 * gap, 5 * gap, 0), Quaternion.identity);
        gridOfMushrooms[4][0] = Instantiate(mushrooms[1], new Vector3(4 * gap, 0 * gap, 0), Quaternion.identity);
        gridOfMushrooms[4][1] = Instantiate(mushrooms[1], new Vector3(4 * gap, 1 * gap, 0), Quaternion.identity);
        gridOfMushrooms[4][2] = Instantiate(mushrooms[2], new Vector3(4 * gap, 2 * gap, 0), Quaternion.identity);
        gridOfMushrooms[4][3] = Instantiate(mushrooms[1], new Vector3(4 * gap, 3 * gap, 0), Quaternion.identity);
        gridOfMushrooms[4][4] = Instantiate(mushrooms[0], new Vector3(4 * gap, 4 * gap, 0), Quaternion.identity);
        gridOfMushrooms[4][5] = Instantiate(mushrooms[2], new Vector3(4 * gap, 5 * gap, 0), Quaternion.identity);
        gridOfMushrooms[5][0] = Instantiate(mushrooms[2], new Vector3(5 * gap, 0 * gap, 0), Quaternion.identity);
        gridOfMushrooms[5][1] = Instantiate(mushrooms[2], new Vector3(5 * gap, 1 * gap, 0), Quaternion.identity);
        gridOfMushrooms[5][2] = Instantiate(mushrooms[1], new Vector3(5 * gap, 2 * gap, 0), Quaternion.identity);
        gridOfMushrooms[5][3] = Instantiate(mushrooms[2], new Vector3(5 * gap, 3 * gap, 0), Quaternion.identity);
        gridOfMushrooms[5][4] = Instantiate(mushrooms[1], new Vector3(5 * gap, 4 * gap, 0), Quaternion.identity);
        gridOfMushrooms[5][5] = Instantiate(mushrooms[0], new Vector3(5 * gap, 5 * gap, 0), Quaternion.identity);

    }

    void GenerateRandomMushrooms1()
    {
        


        for (int x = 0; x < gridOfMushrooms.Length; x++)
        {
            for (int y = 0; y < gridOfMushrooms[x].Length; y++)
            {
                if (gridOfMushrooms[x][y] == null && y >= gridSize.y / 2)
                { 
                    int top = Mathf.Max(x - 1, 0);
                    int left = Mathf.Max(y - 1, 0);
                    /*
                    if (y > 0)
                        notAllowrdMushrooms[0] = gridOfMushrooms[x][left];

                    if (x > 0)
                    {
                        notAllowrdMushrooms[1] = gridOfMushrooms[top][y];
                    }
                    else if (y > 0)
                    {
                        notAllowrdMushrooms[1] = gridOfMushrooms[x][left];
                    }
                    GameObject[] allowedMushrooms = mushrooms;

                    for (int i = 0; i < allowedMushrooms.Length; i++)
                    {
                        if (allowedMushrooms[i].tag == notAllowrdMushrooms[0].tag || allowedMushrooms[i].tag == notAllowrdMushrooms[1].tag)
                            allowedMushrooms = allowedMushrooms.Except(new GameObject[] { allowedMushrooms[i] }).ToArray();
                    }*/

                    
                    mushroomToInstantiate = mushrooms[UnityEngine.Random.Range(0, mushrooms.Length)];
                    gridOfMushrooms[x][y] = Instantiate(mushroomToInstantiate, new Vector3(x * gap, y * gap, 0), Quaternion.identity);
                    

                }
            }


        }

        

    }

    void GenerateRandomMushrooms()
    {
        gridSize = new Vector2(6, 12);
        gridOfMushrooms = new GameObject[(int)gridSize.x][];


        for (int x = 0; x < gridSize.x; x++)
        {
            gridOfMushrooms[x] = new GameObject[(int)gridSize.y];


            for (int y = 0; y < gridSize.y; y++)
            {
                int top = Mathf.Max(x - 1, 0);
                int left = Mathf.Max(y - 1, 0);

                if (y > 0)
                    notAllowrdMushrooms[0] = gridOfMushrooms[x][left];

                if (x > 0)
                {
                    notAllowrdMushrooms[1] = gridOfMushrooms[top][y];
                }
                else if (y > 0)
                {
                    notAllowrdMushrooms[1] = gridOfMushrooms[x][left];
                }
                GameObject[] allowedMushrooms = mushrooms;

                for (int i = 0; i < allowedMushrooms.Length; i++)
                {
                    if (allowedMushrooms[i].tag == notAllowrdMushrooms[0].tag || allowedMushrooms[i].tag == notAllowrdMushrooms[1].tag)
                        allowedMushrooms = allowedMushrooms.Except(new GameObject[] { allowedMushrooms[i] }).ToArray();
                }

                if (gridOfMushrooms[x][y] == null)
                {
                    mushroomToInstantiate = allowedMushrooms[UnityEngine.Random.Range(0, allowedMushrooms.Length)];
                    gridOfMushrooms[x][y] = Instantiate(mushroomToInstantiate, new Vector3(x * gap, y * gap, 0), Quaternion.identity);
                }
            }

        }

    }
    void DestroyAllObjects()
    {
        DestroyHorizontalMushrooms();
        DestroyVerticalMushrooms();
        if (indicesToRemove.Count > 2)
        {
            foreach (Vector2Int element in indicesToRemove)
            {
                if (gridOfMushrooms[element.x][element.y] != null)
                {
                    Destroy(gridOfMushrooms[element.x][element.y]);
                    gridOfMushrooms[element.x][element.y] = null;
                }
            }
            indicesToRemove.Clear();
            isDestroyed = true;
        }
        
    }
     void FallMushrooms()
    {
        counter = 0;
        for (int x = 0; x < gridOfMushrooms[0].Length; x++)
        {
            for (int y = 0; y < gridOfMushrooms.Length; y++)
            {
                int top = Mathf.Min(x + 1, gridOfMushrooms.Length);

                if (gridOfMushrooms[y][x] == null)
                {
                    
                    

                    if (gridOfMushrooms[y][top] != null)

                    {
                        pos = new Vector3(y * gap, x * gap, 0);

                        gridOfMushrooms[y][x] = gridOfMushrooms[y][top];
                        //gridOfMushrooms[y][x] = Instantiate(gridOfMushrooms[y][x], new Vector3(y * gap, x * gap, 0), Quaternion.identity);
                        mushroomToFall ms = new mushroomToFall();
                        ms.mushroom = gridOfMushrooms[y][top];
                        ms.position = pos;
                        mushroomsToFall.Add(ms);
                        
                        //gridOfMushrooms[y][top].transform.position = Vector2.MoveTowards(gridOfMushrooms[y][top].transform.position, pos, 10000);
                        gridOfMushrooms[y][top] = null;
                        //Destroy(gridOfMushrooms[y][top]);
                        canBeGeneratedList.Add(false);

                    }
                    else
                    {
                        canBeGeneratedList.Add(true);
                    }
                    

                }

            }

        }
       
        if (!canBeGeneratedList.Contains(false))
        {
            if (mushroomsToFall.Count == 0 || mushroomsToFall.Last().mushroom.transform.position == mushroomsToFall.Last().position)
            {
                mushroomsToFall.Clear();
                canBeGenerated = true;
                
            }

        }
        else
        {
            canBeGenerated = false;
                
        }
        canBeGeneratedList.Clear();
    }

 
    /*
    private void Sequence()
    {
        StartCoroutine(Seq());
 }

    private IEnumerator Seq()
    {
        //yield return FallMushrooms();
        //yield return GenerateRandomMushrooms1();
        
    }
    */
}
   