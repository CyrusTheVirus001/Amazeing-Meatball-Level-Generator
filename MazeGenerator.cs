using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class MazeGenerator : MonoBehaviour {

    public AstarPath aGraph;
    public Transform Player;
    public Transform StartPosition;
    public Transform EndPosition;
    public TerrainGenerator Terrain;
    public Transform Scarab;
    public Transform Redminataur;
    public Transform Purpleminataur;
    public Transform Yellowminataur;
    public Transform Watermelon;
    public Transform AncientArmors;
    public Transform BirdEyes;
    public Transform StrangeCrystals;
    private Transform terrain;
    private MazeGameObjectives MGO;
    private bool GameStarted = false;
    public int LevelSelection = 0;
    private bool[] SquaresTraverseable;
    private bool[] SquaresPopulated;
    private int GGarraySize;
    private int RandomNumber;
    private int startNode;
    private int endNode;
    private bool MazeStateOK;
    private int CheckOne;
    private int CheckTwo;
    private int CheckThree;
    private int CheckFour;
    private int width = 10;
    private int depth = 10;


    public bool GoldenScarab = false;
    public int NumberofScarabs = 0;
    public bool RedMinataurs = false;
    public int NumberofRedMinataurs = 0;
    public bool PurpleMinataurs = false;
    public int NumberofPurpleMinataurs = 0;
    public bool YellowMinataurs = false;
    public int NumberofYellowMinataurs = 0;
    public bool WaterMelons = false;
    public int NumberofWatermelons = 0;
    public bool AncientArmor = false;
    public int NumberofAncientArmor = 0;
    public bool BirdsEye = false;
    public int NumberofBirdsEye = 0;
    public bool StrangeCrystal = false;
    public int NumberofStrangeCrystals = 0;





    // Use this for initialization
    void Start () {
        FindGameObjectives();
        AstarPath.active.data.gridGraph.SetDimensions(width, depth, 5);
        AstarPath.active.Scan();
        var gg = AstarPath.active.data.gridGraph;
        
        SquaresTraverseable = new bool[gg.nodes.Length+1];
        SquaresPopulated = new bool[gg.nodes.Length+1];
        GGarraySize = (gg.width * gg.depth);

        
        
            Debug.Log("Create MAZE");
            createStartEndandBound(gg);
            GenerateMaze(gg);
            GenerateMazeContinue(gg);
            PopulateArray();
        // GenerateMazeContinued(gg);
        AstarPath.active.Scan();
            gg = AstarPath.active.data.gridGraph;
            CheckMaze(gg);

        
        AddGoldenScarab(gg);
        RedMinataur(gg);
        AddPurpleMinataurs(gg);
        AddYellowMinataurs(gg);
        AddWatermelons(gg);
        AddPowerUps(gg);
        
        // CheckMaze(gg);
        

    }
	
	// Update is called once per frame
	void Update () {

        if (!GameStarted)
        {
            Invoke("StartGame", 1f);
            GameStarted = true;
        }
        
		
	}

    public void createStartEndandBound(GridGraph GG)
    {
        int StartNode = Random.Range(2, GG.width-1);
        startNode = StartNode;
        int EndNode = Random.Range((GG.nodes.Length+2 - GG.width), GG.nodes.Length-1);
        endNode = EndNode;
        for(int i = 0; i <= GG.width-1; i++)//Find Start Position
        {
            if(GG.nodes[i].NodeInGridIndex == StartNode)
            {

                
                Instantiate(StartPosition, (Vector3)GG.nodes[StartNode].position, StartPosition.rotation);
                Instantiate(Player, (Vector3)GG.nodes[StartNode].position, Player.rotation);
                SquaresTraverseable[StartNode] = true;
                SquaresPopulated[StartNode] = true;
            }
            else if(GG.nodes[i].NodeInGridIndex != StartNode)
            {
                terrain = TerrainDeterminer();
                terrain.GetComponent<SortingGroup>().sortingOrder = (GGarraySize - GG.nodes[i].NodeInGridIndex);
                Instantiate(terrain, (Vector3)GG.nodes[i].position, terrain.rotation);
                SquaresPopulated[i] = true;
            }

        }
        for(int i = GG.nodes.Length - GG.width; i < GG.nodes.Length; i++)//Find End Position
        {
            if (GG.nodes[i].NodeInGridIndex == EndNode)
            {
                
                Instantiate(EndPosition, (Vector3)GG.nodes[EndNode].position, EndPosition.rotation);
                SquaresTraverseable[EndNode] = true;
                SquaresPopulated[EndNode] = true;
            }
            else if (GG.nodes[i].NodeInGridIndex != EndNode)
            {
                terrain = TerrainDeterminer();
                terrain.GetComponent<SortingGroup>().sortingOrder = (GGarraySize - GG.nodes[i].NodeInGridIndex);
                Instantiate(terrain, (Vector3)GG.nodes[i].position, terrain.rotation);
                SquaresPopulated[i] = true;
            }
        }
        for(int i = GG.width; i < GG.nodes.Length; i += GG.width)//create left bounds
        {
            if(SquaresTraverseable[i] == false)
            {
                terrain = TerrainDeterminer();
                terrain.GetComponent<SortingGroup>().sortingOrder = (GGarraySize - GG.nodes[i].NodeInGridIndex);
                Instantiate(terrain, (Vector3)GG.nodes[i].position, terrain.rotation);
                SquaresPopulated[i] = true;

            }
        }
        for (int i = GG.width-1; i < GG.nodes.Length; i += GG.width)//create right bounds
        {
            if (SquaresTraverseable[i] == false)
            {
                terrain = TerrainDeterminer();
                terrain.GetComponent<SortingGroup>().sortingOrder = (GGarraySize - GG.nodes[i].NodeInGridIndex);
                Instantiate(terrain, (Vector3)GG.nodes[i].position, terrain.rotation);
                SquaresPopulated[i] = true;

            }
        }
    }

    public void GenerateMaze(GridGraph GG)
    {
        foreach (GridNode nodes in GG.nodes)
        {
           
            if(SquaresPopulated[nodes.NodeInGridIndex] == false)//Check if square is already populated
            {
        
                    if (nodes.NodeInGridIndex - 1 > 0 && nodes.NodeInGridIndex + 1 < GG.nodes.Length && nodes.NodeInGridIndex - GG.width+1 > 0 && nodes.NodeInGridIndex + GG.width+1 < GG.nodes.Length)
                {


                    if (SquaresTraverseable[nodes.NodeInGridIndex - 1] == false && SquaresTraverseable[nodes.NodeInGridIndex + 1] == false &&
                        SquaresTraverseable[nodes.NodeInGridIndex - GG.width] == false && SquaresTraverseable[nodes.NodeInGridIndex + GG.width] == false
                        && SquaresTraverseable[nodes.NodeInGridIndex - GG.width + 1] == false && SquaresTraverseable[nodes.NodeInGridIndex - GG.width - 1] == false
                        && SquaresTraverseable[nodes.NodeInGridIndex + GG.width + 1] == false && SquaresTraverseable[nodes.NodeInGridIndex + GG.width - 1] == false) 
                    {
                        SquaresTraverseable[nodes.NodeInGridIndex] = RandomBool();
                        Debug.Log(SquaresTraverseable[nodes.NodeInGridIndex] + "sqTraversable");
                        
                        if (SquaresTraverseable[nodes.NodeInGridIndex] == false)
                        {
                            SquaresPopulated[nodes.NodeInGridIndex] = true;
                            terrain = TerrainDeterminer();
                            terrain.GetComponent<SortingGroup>().sortingOrder = (GGarraySize - nodes.NodeInGridIndex);
                            Instantiate(terrain, (Vector3)nodes.position, terrain.rotation);
                        }
                    }
                   
                }
            }
        }
    }

    public void GenerateMazeContinue(GridGraph GG)
    {
        foreach (GridNode nodes in GG.nodes)
        {
            Debug.Log("First Enter" + SquaresTraverseable[nodes.NodeInGridIndex]);
            if (SquaresTraverseable[nodes.NodeInGridIndex] == true)
            {
                Debug.Log("Sq Enter" + SquaresTraverseable[nodes.NodeInGridIndex]);
                if (nodes.NodeInGridIndex - 1 > 0 && nodes.NodeInGridIndex + 1 < GG.nodes.Length && nodes.NodeInGridIndex - GG.width + 1 > 0 && nodes.NodeInGridIndex + GG.width + 1 < GG.nodes.Length)
                {

                    CheckOne = ConvertBoolean((SquaresPopulated[nodes.NodeInGridIndex + 1]));
                    CheckTwo = ConvertBoolean((SquaresPopulated[nodes.NodeInGridIndex - 1]));
                    CheckThree = ConvertBoolean((SquaresPopulated[nodes.NodeInGridIndex + GG.width]));
                    CheckFour = ConvertBoolean((SquaresPopulated[nodes.NodeInGridIndex - GG.width]));
                   // CheckFive = ConvertBoolean(SquaresPopulated[nodes.NodeInGridIndex - GG.width + 1]);
                    //CheckSix = ConvertBoolean(SquaresPopulated[nodes.NodeInGridIndex - GG.width - 1]);
                   // Checkseven = ConvertBoolean(SquaresPopulated[nodes.NodeInGridIndex + GG.width + 1]);
                   // CheckEight = ConvertBoolean(SquaresTraverseable[nodes.NodeInGridIndex + GG.width - 1]);

                    Debug.Log("squares Added" + (CheckOne + CheckTwo + CheckThree + CheckFour));
                    Debug.Log("Sq After Addition" + SquaresTraverseable[nodes.NodeInGridIndex]);
                    if ((CheckOne + CheckTwo + CheckThree + CheckFour) <= 1)
                    {
                        Debug.Log("Sq During IF" + SquaresTraverseable[nodes.NodeInGridIndex]);
                        terrain = TerrainDeterminer();
                        terrain.GetComponent<SortingGroup>().sortingOrder = (GGarraySize - nodes.NodeInGridIndex);
                        Instantiate(terrain, (Vector3)nodes.position, terrain.rotation);
                        SquaresTraverseable[nodes.NodeInGridIndex] = false;
                        SquaresPopulated[nodes.NodeInGridIndex] = true;
                        
                    }
            
                
                }
            }
            Debug.Log("Sq leaving" + SquaresTraverseable[nodes.NodeInGridIndex]);
        }
    }

    public int ConvertBoolean(bool x)
    {
        if (x)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }



    public void CheckMaze(GridGraph GG)
    {
        foreach(GraphNode node in GG.nodes)
        {
            if(node.Area > 1)
            {
                MazeStateOK = false;
                SceneManager.LoadScene("MazeGame");
            }

        }

    }

    public bool RandomBool()
    {
        float RN = Random.value;
        
        if(RN >= .5)
        {
            return true;
        }
        return false;   
    }

    public Transform TerrainDeterminer()
    {
        switch (LevelSelection)
        {
            case 0:
                return Terrain.BasicForestTerrain();
                
            case 1:
                return terrain = Terrain.BasicSnowTerrain();

            case 2:
                return terrain = Terrain.BasicDesertTerrain();

            case 3:
                return terrain = Terrain.BasicVillageTerrain();

            case 4:
                return terrain = Terrain.BasicCrystalTerrain();

            case 5:
                return terrain = Terrain.BasicMushroomTerrain();

            case 6:
                return terrain = Terrain.BasicDenseForestTerrain();

            case 7:
                return terrain = Terrain.DenseForestVillage();

            case 8:
                return terrain = Terrain.CrystalVillage();

            case 9:
                return terrain = Terrain.MushroomDenseForest();

            case 10:
                return terrain = Terrain.DesertVillageForest();

            case 11:
                return terrain = Terrain.BasicRockyBattlementsTerrain();

            case 12:
                return terrain = Terrain.RockyBattlementsDesert();

            case 13:
                return terrain = Terrain.RockyBattlementsDenseForest();

            case 14:
                return terrain = Terrain.SpikeBallsRockyBattlementsEasy();

            case 15:
                return terrain = Terrain.SpikeBallsRockyBattlementsMedium();

            case 16:
                return terrain = Terrain.SpikeBallsCrystals();

            case 17:
                return terrain = Terrain.SpikeBallsOnly();

            case 18:
                return terrain = Terrain.ALittleBitofEverything();

            default:
                Debug.Log("Error In Terrain Selection");
                return null;
        }

    }

    public void FindTilesforGoldenScarab(GridGraph GG)
    {
        
        {
            RandomNumber = Random.Range(0, GG.nodes.Length);
                
            foreach(GraphNode node in GG.nodes)
            {
                                
                if(SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
                {
                   
                    Instantiate(Scarab, (Vector3)GG.nodes[node.NodeInGridIndex].position, Scarab.rotation);
                    return;
                }

            }
            
            FindTilesforGoldenScarab(GG);
        }
    }

    public void PopulateArray()
    {
      for(int i = 0; i < SquaresTraverseable.Length; i++)
        {
            if (SquaresPopulated[i] == false)
            {
                Debug.Log("How Many Unpopulated");
                SquaresTraverseable[i] = true;
            }
        }
    }

    public void RedMinataur(GridGraph GG)
    {
        if (RedMinataurs)
        {
            for(int i=0;i < NumberofRedMinataurs; i++)
            {
                FindTilesforMinataurs(GG);
            }
        }
    }

    public void FindTilesforMinataurs(GridGraph GG)
    {
        RandomNumber = Random.Range((GG.width*3), GG.nodes.Length);
        foreach(GraphNode node in GG.nodes)
        {

            if (SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
            {
                RandomNumber = Random.Range(50, 150);
                Redminataur.GetComponent<BetterEnemyAI>().speed = RandomNumber;
                Instantiate(Redminataur, (Vector3)GG.nodes[node.NodeInGridIndex].position, Redminataur.rotation);
                return;
            }
        }
        FindTilesforMinataurs(GG);    
    }

    public void AddWatermelons(GridGraph GG)
    {
        if (WaterMelons)
        {
            for (int i = 0; i < NumberofWatermelons; i++)
            {
                FindTilesForWatermelons(GG);
            }
        }
    }

    public void FindTilesForWatermelons(GridGraph GG)
    {
        RandomNumber = Random.Range(0, GG.nodes.Length);
        foreach (GraphNode node in GG.nodes)
        {

            if (SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
            {                
                Instantiate(Watermelon, (Vector3)GG.nodes[node.NodeInGridIndex].position, Watermelon.rotation);
                return;
            }
        }
        FindTilesForWatermelons(GG);
    }

    public void AddPurpleMinataurs(GridGraph GG)
    {
        if (PurpleMinataurs)
        {
            for (int i = 0; i < NumberofPurpleMinataurs; i++)
            {
                FindTileForPurpleMinataurs(GG);
            }
        }
    }

    public void FindTileForPurpleMinataurs(GridGraph GG)
    {
        RandomNumber = Random.Range((GG.width * 3), GG.nodes.Length);
        foreach (GraphNode node in GG.nodes)
        {

            if (SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
            {
                Instantiate(Purpleminataur, (Vector3)GG.nodes[node.NodeInGridIndex].position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                return;
            }
        }
        FindTileForPurpleMinataurs(GG);
    }

    public void AddYellowMinataurs(GridGraph GG)
    {
        if (YellowMinataurs)
        {
            for (int i = 0; i < NumberofYellowMinataurs; i++)
            {
                FindTilesforYellowMinataurs(GG);
            }
        }
    }

    public void FindTilesforYellowMinataurs(GridGraph GG)
    {
        RandomNumber = Random.Range((GG.width * 3), GG.nodes.Length);
        foreach (GraphNode node in GG.nodes)
        {

            if (SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
            {
                Instantiate(Yellowminataur, (Vector3)GG.nodes[node.NodeInGridIndex].position, Yellowminataur.rotation);
                return;
            }
        }
        FindTilesforYellowMinataurs(GG);
    }

    public void AddGoldenScarab(GridGraph GG)
    {
        if (GoldenScarab)
        {
            for (int i = 0; i < NumberofScarabs; i++)
            {
                FindTilesforGoldenScarab(GG);
            }
        }
    }

    public void AddPowerUps(GridGraph GG)
    {
        if (AncientArmor)
        {
            for (int i = 0; i < NumberofAncientArmor; i++)
            {
                FindTilesforAncientArmor(GG);
            }
        }
        if (BirdsEye)
        {
            for (int i = 0; i < NumberofBirdsEye; i++)
            {
                FindTilesforBirdsEye(GG);
            }
        }
        if (StrangeCrystal)
        {
            for (int i = 0; i < NumberofStrangeCrystals; i++)
            {
                FindTilesforStrangeCrystals(GG);
            }
        }
    }

    public void FindTilesforAncientArmor(GridGraph GG)
    {
        RandomNumber = Random.Range(0, GG.nodes.Length);
        foreach (GraphNode node in GG.nodes)
        {

            if (SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
            {
                Instantiate(AncientArmors, (Vector3)GG.nodes[node.NodeInGridIndex].position, AncientArmors.rotation);
                return;
            }
        }
        FindTilesforAncientArmor(GG);
    }

    public void FindTilesforBirdsEye(GridGraph GG)
    {
        RandomNumber = Random.Range(0, GG.nodes.Length);
        foreach (GraphNode node in GG.nodes)
        {

            if (SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
            {
                Instantiate(BirdEyes, (Vector3)GG.nodes[node.NodeInGridIndex].position, BirdEyes.rotation);
                return;
            }
        }
        FindTilesforBirdsEye(GG);
    }

    public void FindTilesforStrangeCrystals(GridGraph GG)
    {
        RandomNumber = Random.Range(0, GG.nodes.Length);
        foreach (GraphNode node in GG.nodes)
        {

            if (SquaresTraverseable[node.NodeInGridIndex] == true && node.NodeInGridIndex == RandomNumber)
            {
                Instantiate(StrangeCrystals, (Vector3)GG.nodes[node.NodeInGridIndex].position, StrangeCrystals.rotation);
                return;
            }
        }
        FindTilesforStrangeCrystals(GG);
    }

    public void FindGameObjectives()
    {
        if(MGO == null)
        {
            MGO = FindObjectOfType<MazeGameObjectives>();
        }

        
        width = MGO.MazeWidth;
        depth = MGO.MazeDepth;
        
        LevelSelection = MGO.ScenerySelection;

        GoldenScarab = MGO.GoldenScarab;
        NumberofScarabs = MGO.NumberofScarabs;
        RedMinataurs = MGO.RedMinataurs;
        NumberofRedMinataurs = MGO.NumberofRedMinataurs;
        PurpleMinataurs = MGO.PurpleMinataurs;
        NumberofPurpleMinataurs = MGO.NumberofPurpleMinataurs;
        YellowMinataurs = MGO.YellowMinataurs;
        NumberofYellowMinataurs = MGO.NumberofYellowMinataurs;
        WaterMelons = MGO.WaterMelons;
        NumberofWatermelons = MGO.NumberofWatermelons;
        AncientArmor = MGO.AncientArmor;
        NumberofAncientArmor = MGO.NumberofAncientArmor;
        BirdsEye = MGO.BirdsEye;
        NumberofBirdsEye = MGO.NumberofBirdsEye;
        StrangeCrystal = MGO.StrangeCrystals;
        NumberofStrangeCrystals = MGO.NumberofStrangeCrystals;
    }

    public void StartGame()
    {
         GameObject.FindGameObjectWithTag("LoadScreen").GetComponent<Animator>().SetBool("ScreenLoading", false);

         LoadScreen.LS.HideInformation();

    }


}
