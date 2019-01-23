using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameMindScript : MonoBehaviour
{

    private static GameType typeOfGame = GameType.UseHeu;
    public static GameMove currentMove = null;
    public static GameObject lastMarble = null;
    private static GameState nextState;
    /*
   * 00-11: horizontal
   * 12-23: verticle
   * 24-27: top left bot right diag
   * 28-31: top right bot left diag
   */
    public static GameData currentGameData;
    static System.Random r = new System.Random();
    static int startQuadrant = -1;
    private static GameState stateOfGame = GameState.NotTurn;
    static GameWinner whoWon = 0;



    public class GameData : ICloneable
    {
        public TileVals[,] gameBoard;
        public int[] winValues = new int[32];
        public bool isGameOver = false;
        public int[] lastTurn = new int[85];
        public GameMove lastMove;
        public List<int[]> turns = new List<int[]>();
        public bool isXTurn = false;
        public GameData()
        {
            gameBoard = new TileVals[6, 6];
            winValues = new int[32];
            isGameOver = false;
        }

        public GameData(TileVals[,] gameBoard, int[] winValues, bool isGameOver, int[] lastTurn, GameMove lastMove, List<int[]> turns, bool isXTurn)
        {
            this.gameBoard = gameBoard;
            this.winValues = winValues;
            this.isGameOver = isGameOver;
            this.lastTurn = lastTurn;
            this.lastMove = lastMove;
            this.turns = turns;
            this.isXTurn = isXTurn;
        }

        public object Clone()
        {
            return new GameData(
                (TileVals[,])this.gameBoard.Clone(),
                (int[])this.winValues.Clone(),
                this.isGameOver,
                (int[])this.lastTurn.Clone(),
                this.lastMove==null?null:(GameMove)this.lastMove.Clone(),
                (List<int[]>)this.turns.Clone(),
                this.isXTurn
            );
        }
    }

    

    public static void SetGameState(GameState s)
    {
        if (GetGameState() != GameState.Over)
        {
            if (GetGameType() != GameType.TwoPlayer && s == GameState.NotTurn)
            {
                SetMenuText("Computer Turn", false);

            }
            else
            {
                switch (s)
                {
                    case GameState.NotTurn:
                        break;
                    case GameState.SecondPlayerTurn:
                    case GameState.PickingCoord:
                        SetMenuText("Picking Coord", false);
                        break;
                    case GameState.PickingRotation:
                        SetMenuText("Picking Rotation", false);
                        break;
                    case GameState.Moving:
                        break;
                    case GameState.Over:
                        SetMenuText("Game Over", true);
                        break;
                }
            }

            stateOfGame = s;
        }
    }

    public static GameState GetGameState()
    {
        return stateOfGame;
    }

    public static void SetNextGameState(GameState s)
    {
        if (nextState != GameState.Over)
        {
            nextState = s;
        }
    }

    public static GameState GetNextGameState()
    {
        return nextState;
    }

    public enum GameState
    {
        NotTurn,
        PickingCoord,
        PickingRotation,
        Moving,
        SecondPlayerTurn,
        Over
    }

    public enum GameType
    {
        TwoPlayer,
        UseHeu,
        UseAI
    }

    public enum GameWinner
    {
        FirstPlayer,
        SecondPlayer,
        AI,
        CPU
    }

    // Use this for initialization
    void Start()
    {
        RestartGame();
        MoveOther();
    }

    public static void SetGameType(GameType g)
    {
        typeOfGame = g;
    }

    public static GameType GetGameType()
    {
        return typeOfGame;
    }

    public static void AdvanceGameState()
    {
        switch (GetNextGameState())
        {
            case GameState.Over:
                break;
            case GameState.NotTurn:
                break;
            case GameState.PickingCoord:
                break;
            case GameState.PickingRotation:
                break;
            case GameState.Moving:
                break;
            case GameState.SecondPlayerTurn:

                switch (typeOfGame)
                {
                    case GameType.TwoPlayer:
                        SetGameState(GameState.PickingCoord);
                        break;
                    case GameType.UseHeu:
                    case GameType.UseAI:
                        SetGameState(GameState.NotTurn);
                        MoveOther();
                        break;
                    default:
                        break;

                }
                return;

            default:
                break;
        }
        SetGameState(GetNextGameState());
    }

    void RestartGame()
    {
        playOnce = true;
        GameType s = GetGameType();
        switch (s)
        {
            case GameType.TwoPlayer:
                SetTitleText("Pentago\nTwo Player");
                break;
            case GameType.UseHeu:
                SetTitleText("Pentago\nVs Heuristic");
                break;
            case GameType.UseAI:
                SetTitleText("Pentago\nVs Neural Net");
                break;
        }
        currentGameData = new GameData();

        currentGameData.lastTurn = new int[85];
        currentGameData.lastMove = null;
        currentGameData.turns = new List<int[]>();
        currentGameData.isXTurn = false;
        //winValues = new int[32];
        r = new System.Random();
        startQuadrant = -1;
        //gameBoard = new TileVals[6, 6];
        //needs to be direct
        stateOfGame = GameState.NotTurn;
        SetGameState(GameState.NotTurn);
        SetNextGameState(GameState.NotTurn);

    }

    // Update is called once per frame
    void Update()
    {


    }

    public static void MovePlayer()
    {
        RotateSquareInUnity(currentMove.rotIndex, currentMove.rotLeft);
        UpdateWinCondition(currentMove, ref currentGameData);
        if (currentGameData.isGameOver)
        {
            EndGame();
        }

        currentMove = null;
    }

    public static void EndGame()
    {
        // Make a background box
        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        SetNextGameState(GameState.Over);
    }

    static bool playOnce;
    void OnGUI()
    {
        if (GetGameState() == GameState.Over)
        {
            if (playOnce)
            {
                GetComponent<AudioSource>().Play();
                playOnce = false;
            }

            var buttonObject = new GameObject("Button");

            if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 80, 180, 60), whoWon.ToString() + " Won"))
            {
                GameMindScript.typeOfGame = GameMindScript.GameType.TwoPlayer;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("SampleScene");


            }
        }

    }

    public static void MoveOther()
    {

        nextState = GameState.PickingCoord;
        GameMove g;

        if (typeOfGame == GameType.TwoPlayer)
        {
            SetGameState(GameState.PickingCoord);
        }
        else
        {
            if (typeOfGame == GameType.UseAI)
            {
                g = NNTurn(currentGameData.gameBoard);
            }
            else if (typeOfGame == GameType.UseHeu)
            {
                g = PentagoHeuristic(currentGameData);
            }
            else
            {
                throw new Exception("Not a valid cpu option");
            }



            PlaceAndTurnInUnity(g);
            UpdateWinCondition(g, ref currentGameData);
            if (currentGameData.isGameOver)
            {
                EndGame();
            }
        }




    }

    public static int IndexFromArray(int x, int y)
    {
        return y * 6 + x;
    }

    public static MyTuple<int, int> ArrayLocationFromIndex(int index)
    {

        return new MyTuple<int, int>(index % 6, index / 6);



    }

    public static void RotateSquareInUnity(int index, bool rotLeft)
    {
        var mQuad = GameObject.Find("BR0" + GameMindScript.GetQuadFromPoint(GameMindScript.currentMove.xCord, GameMindScript.currentMove.yCord));
        mQuad.GetComponent<AudioSource>().Play();

        var ScriptThatYouWantM = mQuad.GetComponent<BoardQuadScript>();
        ScriptThatYouWantM.marbles.Add(lastMarble);


        var rotator = GameObject.Find("BR0" + index);
        var ScriptThatYouWant = rotator.GetComponent<BoardQuadScript>();
        ScriptThatYouWant.Rotate(rotLeft);

    }

    public static void PlaceAndTurnInUnity(GameMove g)
    {
        currentMove = g;

        int indexToPlace = IndexFromArray(g.xCord, g.yCord);

        String objectName = String.Format("BallCollider{0:00}", indexToPlace);
        var placeToPut = GameObject.Find(objectName);


        var mar = PlaceMarble(placeToPut.transform.position);

        GameMindScript.lastMarble = mar;

        RotateSquareInUnity(g.rotIndex, g.rotLeft);

    }


    public static void SetTitleText(string s)
    {
        var mQuad = GameObject.Find("TextTitle");

        GameMindScript.GetGameType();
        var t = mQuad.GetComponent<Text>();
        t.text = s + "\n-----------";
    }


    public static void SetMenuText(string s, bool ended)
    {
        var mQuad = GameObject.Find("TextMenu");

        GameMindScript.GetGameType();
        var t = mQuad.GetComponent<Text>();
        if (ended)
        {
            t.text = "Turn #" + (GetTurns(currentGameData).Count) + "\n" + s + "\n" + (!currentGameData.isXTurn ? "(Red)" : "(Black)");

        }
        else
        {
            t.text = "Turn #" + (GetTurns(currentGameData).Count + 1) + "\n" + s + "\n" + (currentGameData.isXTurn ? "(Red)" : "(Black)");

        }
    }

    public static GameObject PlaceMarble(Vector3 place)
    {

        var mat = Resources.Load("RedMarble");
        var marble = GameObject.Find("Marble");

        GameObject marbleToPlace = Instantiate(marble);
        marbleToPlace.GetComponent<Renderer>().material.color = currentGameData.isXTurn ? Color.red : Color.black;
        marbleToPlace.transform.position = place;
        marbleToPlace.transform.AddPos(y: 1);

        return marbleToPlace;
    }



    /// <summary>
    /// Updates the window condition.
    /// </summary>
    /// <returns><c>true</c>, if game is over, <c>false</c> otherwise.</returns>
    /// <param name="g">The move to mark down.</param>
    public static void UpdateWinCondition(GameMove g, ref GameData d)
    {
        d.isXTurn = !d.isXTurn;
        //change turn
        d.lastMove = g;


        d.gameBoard[g.xCord, g.yCord] = d.isXTurn ? TileVals.X : TileVals.O;
        UpdatePoint(d.gameBoard, g.xCord, g.yCord, ref d);
        TrackPlacement(g.xCord, g.yCord, ref d);

        TrackRotation(g.rotIndex, g.rotLeft, ref d);

        //rotation
        d.gameBoard = RotateSquare(d.gameBoard, g.rotIndex, g.rotLeft);


        UpdateRotation(d.gameBoard, g.rotIndex, ref d);
        UpdateTurn(ref d);
        int gameOver = IsGameWon(d);
        if (gameOver > 0)
        {
            Console.Write("Game Over On Turn " + GetTurns(d).Count + ".\n");
            if (gameOver == 1)
            {
                whoWon = GameWinner.FirstPlayer;
                Console.Write("X Won");
            }
            else if (gameOver == 2)
            {
                whoWon = GameWinner.SecondPlayer;

                Console.Write("O Won");
            }
            else
            {
                Console.Write("Draw");
            }
            d.isGameOver = true;
            return;
        }
        d.isGameOver = false;
    }




    static List<int[]> GetTurns(GameData d)
    {
        return d.turns;
    }
    //keeps track of which player is allowed to move

    static void AddTurn(int[] t, ref GameData d)
    {
        if (t.Length != 85)
        {
            throw new ArgumentException("Array did not have 85 elements");
        }
        d.turns.Add(t);
    }
    static void TrackPlacement(int x, int y, ref GameData d)
    {
        d.lastTurn[x * 6 + y] = 1;
    }
    static void TrackRotation(int x, String y, ref GameData d)
    {
        if (y == "right" || y == "Right" || y == "r" || y == "R")
        {
            TrackRotation(x, false, ref d);
        }
        else
        {
            TrackRotation(x, true, ref d);
        }
    }
    static void TrackRotation(int x, bool rotLeft, ref GameData d)
    {
        if (rotLeft)
        {
            d.lastTurn[84 - ((x * 2) + 1)] = 1;

        }
        else
        {
            d.lastTurn[84 - (x * 2)] = 1;

        }
    }
    static void UpdateTurn(ref GameData d)
    {
        AddTurn(d.lastTurn, ref d);
        d.lastTurn = new int[85];
    }



    //rotational index of values in squares
    static readonly int[] rotIndex = {
                                        0, 1, 2,
                                        6, 7, 8,
                                        12, 13, 14
                                };
    //swuare that has been left rotated
    static readonly int[] leftRotIndex = {
                                        12, 6, 0,
                                        13, 7, 1,
                                        14, 8, 2
                                };
    //square that has been right rotated
    static readonly int[] rightRotIndex = {
                                        2, 8, 14,
                                        1, 7, 13,
                                        0, 6, 12
                                };



    static TupleList<int, int> PointsFromWinCondition(int index)
    {
        TupleList<int, int> returnValues = new TupleList<int, int>();
        if (index <= 11)
        {
            //horizontal
            int additive = index % 2;
            for (int i = 0; i < 5; i++)
            {
                returnValues.Add(new MyTuple<int, int>(i + additive, index / 2));
            }
        }
        else if (index <= 23)
        {
            //verticle
            int additive = index % 2;
            for (int i = 0; i < 5; i++)
            {
                returnValues.Add(new MyTuple<int, int>((index - 12) / 2, i + additive));
            }
        }
        else if (index <= 27)
        {
            //diag 1
            int x = -1;
            int y = -1;
            switch (index)
            {
                case 24:
                    x = 0;
                    y = 1;
                    break;
                case 25:
                    x = 0;
                    y = 0;
                    break;
                case 26:
                    x = 1;
                    y = 1;
                    break;
                case 27:
                    x = 1;
                    y = 0;
                    break;
                default:
                    break;
            }
            return DiagFromPoint(x, y, true);

        }
        else if (index <= 31)
        {
            //diag 2        
            int x = -1;
            int y = -1;
            switch (index)
            {
                case 28:
                    x = 4;
                    y = 0;
                    break;
                case 29:
                    x = 5;
                    y = 0;
                    break;
                case 30:
                    x = 4;
                    y = 1;
                    break;
                case 31:
                    x = 5;
                    y = 1;
                    break;
                default:
                    break;
            }
            return DiagFromPoint(x, y, false);

        }
        else
        {
            throw new Exception("Out of bounds of win array");
        }
        return returnValues;
    }


    static void UpdatePoint(TileVals[,] board, int x, int y, ref GameData d)
    {

        #region Update Horizontal
        if (x != 0)
        {
            //update leftmost to right
            TileVals[] xTiles = CustomArray<TileVals>.GetRowMinusFirst(board, y);
            var xSum = Array.ConvertAll(xTiles, value => (int)value).Sum();
            d.winValues[y * 2 + 1] = xSum;
        }
        if (x != 5)
        {
            //update left to rightmost
            TileVals[] xTiles = CustomArray<TileVals>.GetRowMinusLast(board, y);
            var xSum = Array.ConvertAll(xTiles, value => (int)value).Sum();
            d.winValues[y * 2] = xSum;
        }
        #endregion

        #region Update Verticle
        if (y != 0)
        {
            //update topmost to bot
            TileVals[] yTiles = CustomArray<TileVals>.GetColumnMinusFirst(board, x);
            var ySum = Array.ConvertAll(yTiles, value => (int)value).Sum();
            d.winValues[12 + x * 2] = ySum;
        }
        if (y != 5)
        {
            //update top to botmost
            TileVals[] yTiles = CustomArray<TileVals>.GetColumnMinusLast(board, x);
            var ySum = Array.ConvertAll(yTiles, value => (int)value).Sum();
            d.winValues[12 + x * 2 + 1] = ySum;
        }
        #endregion

        //update diag (2)
        int tmpX;
        int tmpY;

        #region Update Top left to Bot right diag
        tmpX = x;
        tmpY = y;
        while (tmpX != 0 && tmpY != 0)
        {
            tmpX--;
            tmpY--;
        }
        if (tmpX == 0 && tmpY == 0)
        {
            //top left point
            var diagOne = (DiagFromPoint(0, 0, true));
            var diagTwo = (DiagFromPoint(1, 1, true));
            d.winValues[24 + 1] = SumDiag(board, diagOne);
            d.winValues[24 + 2] = SumDiag(board, diagTwo);

        }
        if ((tmpX + tmpY) == 1)
        {
            //other 2 left to right
            var diag = DiagFromPoint(tmpX, tmpY, true);
            TupleList<int, int>[] diags = new TupleList<int, int>[4];
            var diagOne = (DiagFromPoint(0, 1, true));
            var diagTwo = (DiagFromPoint(1, 0, true));
            d.winValues[24 + 0] = SumDiag(board, diagOne);
            d.winValues[24 + 3] = SumDiag(board, diagTwo);

        }
        #endregion

        #region Update Top right to Bot left diag
        tmpX = x;
        tmpY = y;
        while (tmpX != 5 && tmpY != 0)
        {
            tmpX++;
            tmpY--;
        }
        if (tmpX == 5 && tmpY == 0)
        {
            //top right point
            var diagOne = (DiagFromPoint(5, 0, false));
            var diagTwo = (DiagFromPoint(4, 1, false));
            d.winValues[28 + 1] = SumDiag(board, diagOne);
            d.winValues[28 + 2] = SumDiag(board, diagTwo);
        }
        else if (tmpX == 4 && tmpY == 0)
        {
            //diags[0] = (DiagFromPoint(4, 0, false));
            //diags[1] = (DiagFromPoint(5, 0, false));
            //diags[2] = (DiagFromPoint(4, 1, false));
            //diags[3] = (DiagFromPoint(5, 1, false));
            //winValues[24 + i] = sumDiag(board, diags[i]);

            var diagOne = (DiagFromPoint(4, 0, false));
            d.winValues[28 + 0] = SumDiag(board, diagOne);
        }
        else if (tmpX == 5 && tmpY == 1)
        {
            var diagOne = (DiagFromPoint(5, 1, false));
            d.winValues[28 + 3] = SumDiag(board, diagOne);
        }
        #endregion

    }

    #region Update wins on square rotation
    static void UpdateRotation(TileVals[,] board, int quad, ref GameData d)
    {
        //update horizontal (6)
        //update verticle (6)
        //update diag (4)
        //update one othe diag (1)
        UpdateHorizontal(board, quad, ref d);
        UpdateVerticle(board, quad, ref d);
        UpdateDiagonal(board, quad, ref d);
        UpdateOther(board, quad, ref d);


    }
    static GameData UpdateHorizontal(TileVals[,] board, int quad, ref GameData d)
    {

        int additive = (quad < 2) ? 0 : 3;

        for (int i = additive; i < 3 + additive; i++)
        {
            // Iterate through the second dimension
            TileVals[] x = CustomArray<TileVals>.GetRowMinusLast(board, i);
            TileVals[] y = CustomArray<TileVals>.GetRowMinusFirst(board, i);
            var xSum = Array.ConvertAll(x, value => (int)value).Sum();
            var ySum = Array.ConvertAll(y, value => (int)value).Sum();

            d.winValues[i * 2] = xSum;
            d.winValues[i * 2 + 1] = ySum;

            Console.WriteLine("X1: " + xSum);
            Console.WriteLine("X2: " + ySum);

        }
        return d;

    }
    static GameData UpdateVerticle(TileVals[,] board, int quad, ref GameData d)
    {
        int additive = (quad == 0 || quad == 2) ? 0 : 3;

        for (int i = additive; i < 3 + additive; i++)
        {
            TileVals[] x = CustomArray<TileVals>.GetColumnMinusLast(board, i);
            TileVals[] y = CustomArray<TileVals>.GetColumnMinusFirst(board, i);
            var xSum = Array.ConvertAll(x, value => (int)value).Sum();
            var ySum = Array.ConvertAll(y, value => (int)value).Sum();

            d.winValues[12 + i * 2] = xSum;
            d.winValues[12 + i * 2 + 1] = ySum;

            Console.WriteLine("Y1: " + xSum);
            Console.WriteLine("Y2: " + ySum);
        }
        return d;

    }
    static void UpdateDiagonal(TileVals[,] board, int quad, ref GameData d)
    {
        if (quad == 0 || quad == 3)
        {
            TupleList<int, int>[] diags = new TupleList<int, int>[4];
            diags[0] = (DiagFromPoint(0, 1, true));
            diags[1] = (DiagFromPoint(0, 0, true));
            diags[2] = (DiagFromPoint(1, 1, true));
            diags[3] = (DiagFromPoint(1, 0, true));
            for (int i = 0; i < 4; i++)
            {
                d.winValues[24 + i] = SumDiag(board, diags[i]);
            }


        }
        else
        {
            TupleList<int, int>[] diags = new TupleList<int, int>[4];
            diags[0] = (DiagFromPoint(4, 0, false));
            diags[1] = (DiagFromPoint(5, 0, false));
            diags[2] = (DiagFromPoint(4, 1, false));
            diags[3] = (DiagFromPoint(5, 1, false));
            for (int i = 0; i < 4; i++)
            {
                d.winValues[28 + i] = SumDiag(board, diags[i]);
            }
        }

    }

    public static int GetQuadFromPoint(int x, int y)
    {
        if (x <= 2 && y <= 2)
        {
            return 0;
        }
        else if (x <= 2)
        {
            return 2;
        }
        else if (y <= 2)
        {
            return 1;
        }
        else
        {
            return 3;
        }
    }

    static void UpdateOther(TileVals[,] board, int quad, ref GameData d)
    {
        switch (quad)
        {
            case 0:
                d.winValues[28] = SumDiag(board, DiagFromPoint(4, 0, false));
                break;
            case 1:
                d.winValues[24 + 3] = SumDiag(board, DiagFromPoint(1, 0, true));

                break;
            case 2:
                d.winValues[24 + 0] = SumDiag(board, DiagFromPoint(0, 1, true));

                break;
            case 3:
                d.winValues[28 + 3] = SumDiag(board, DiagFromPoint(5, 1, false));
                break;
            default:
                throw new ArgumentException("quad greater than 3");
        }
    }
    #endregion

    static TileVals[,] RotateSquare(TileVals[,] board, int squareToRotate, bool rotLeft)
    {
        int baseForIndex = 0;
        if (squareToRotate > 1)
        {
            baseForIndex += 18;
        }
        if (squareToRotate == 1 || squareToRotate == 3)
        {
            baseForIndex += 3;
        }
        TileVals[] tempTiles = new TileVals[9];
        for (int i = 0; i < 9; i++)
        {
            int fromSpot = rotIndex[i] + baseForIndex;
            int fromX = fromSpot % 6;
            int fromY = fromSpot / 6;
            tempTiles[i] = board[fromX, fromY];
        }
        if (rotLeft)
        {
            for (int i = 0; i < 9; i++)
            {
                int x = (leftRotIndex[i] + baseForIndex) % 6;
                int y = (leftRotIndex[i] + baseForIndex) / 6;
                board[x, y] = tempTiles[i];
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                int x = (rightRotIndex[i] + baseForIndex) % 6;
                int y = (rightRotIndex[i] + baseForIndex) / 6;
                board[x, y] = tempTiles[i];

            }
        }
        return board;

    }














    //static GameMove PlayerTurn(TileVals[,] gameBoard)
    //{
    //    GameMove playerMove = new GameMove();
    //    //set vals to illegal by default
    //    var xVal = -1;
    //    var yVal = -1;
    //    while (true)
    //    {
    //        xVal = TryGetInt("x value", 0, 5);
    //        yVal = TryGetInt("y value", 0, 5);
    //        //make sure the value selected is actually open
    //        if (gameBoard[xVal, yVal] != TileVals.Blank)
    //        {
    //            PrintBoard(gameBoard);
    //            Console.WriteLine("Square already taken\n");
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }
    //    //place an X or O depending on whos turn it is
    //    playerMove.xCord = xVal;
    //    playerMove.yCord = yVal;

    //    int square = TryGetInt("index of square to rotate:\n0 1\n2 3", 0, 3);
    //    string rot = "";
    //    Console.WriteLine("Enter (L)eft or (R)ight for rotation");
    //    rot = Console.ReadLine();

    //    //list of valid values for rotation
    //    var rotationInput = new List<string> { "right", "left", "r", "l" };

    //    while (!rotationInput.Contains(rot.ToLower()))
    //    {
    //        Console.WriteLine("rotate not valid");
    //        rot = Console.ReadLine();
    //    }

    //    Console.WriteLine("You entered " + rot);
    //    playerMove.rotIndex = square;
    //    playerMove.rotLeft = (rot.ToLower() == "left" || rot.ToLower() == "l");
    //    return playerMove;
    //}

    static GameMove NNTurn(TileVals[,] gameBoard)
    {
        return RunPythonThing(gameBoard);

    }


    static int SumDiag(TileVals[,] board, TupleList<int, int> diags)
    {
        int retVal = 0;

        foreach (var item in diags)
        {
            retVal += (int)board[item.Item1, item.Item2];
        }
        return retVal;
    }
    static TupleList<int, int> DiagFromPoint(int x, int y, bool leftToRight)
    {
        var diag = new TupleList<int, int>();
        diag.Add(x, y);
        for (int i = 0; i < 4; i++)
        {
            x = leftToRight ? x + 1 : x - 1;
            y++;
            diag.Add(x, y);
        }
        return diag;

    }

    static int MatchArray(int[] basArr, List<int[]> matches)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            if (basArr.SequenceEqual(matches[i]))
            {
                return i;
            }
        }
        return -1;
    }

    static string EnumArrToNNArr(TileVals[,] enumArr)
    {
        var x = enumArr.Cast<TileVals>().ToList().ToArray();
        int[] arr = Array.ConvertAll(x, value => (int)value);
        string retString = arr[0].ToString();
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] == 10)
            {
                retString += " -1";
            }
            else
            {
                retString += " " + arr[i].ToString();
            }
        }
        return retString;
    }

    static TileVals[,] RotateBoard(TileVals[,] board, int n)
    {
        TileVals[,] ret = new TileVals[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                ret[i, j] = board[n - j - 1, i];
            }
        }
        return ret;
    }
    static int IsGameWon(GameData d)
    {
        bool didXWin = false;
        bool didOWin = false;
        foreach (var item in d.winValues)
        {
            if (item == 50)
            {
                didOWin = true;
                Console.WriteLine("O won");
            }
            if (item == 5)
            {
                didXWin = true;
                Console.WriteLine("X won");
            }
        }
        if (didXWin && didOWin)
        {
            return 3;
        }
        if (didXWin)
        {
            return 1;
        }
        if (didOWin)
        {
            return 2;
        }
        return 0;
    }


    static string TileToString(TileVals t)
    {
        switch (t)
        {
            case TileVals.X:
                return "X";
            case TileVals.O:
                return "O";
            case TileVals.Blank:
                return " ";
            default:
                return "error";
        }
    }


    public class LookaheadHelper
    {
        private int min;
        private int max;
        private GameMove move;

        public LookaheadHelper(int min, int max, GameMove move)
        {
            this.Min = min;
            this.Max = max;
            this.Move = move;
        }

        public int Min
        {
            get
            {
                return min;
            }

            set
            {
                min = value;
            }
        }

        public int Max
        {
            get
            {
                return max;
            }

            set
            {
                max = value;
            }
        }

        public GameMove Move
        {
            get
            {
                return move;
            }

            set
            {
                move = value;
            }
        }
    }

    /// <summary>
    /// Gets the best move the cpu could do with lookahead at the player
    /// </summary>
    /// <param name="d"></param>
    /// <returns>The optimal move</returns>
    static GameMove GetFromLookaheadCPU(GameData d)
    {
        d = (GameData) d.Clone();
        List<LookaheadHelper> possibleMoves = new List<LookaheadHelper>();
        LookaheadHelper bestMove = new LookaheadHelper(-1, -1, null); ;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (d.gameBoard[i, j] == TileVals.Blank)
                {
                    //for each rotation possible
                    for (int k = 0; k < 8; k++)
                    {
                        if (bestMove.Move == null)
                        {
                            GameMove potentialMove = new GameMove(i, j, k / 2, k % 2 == 0);

                            GameData tmp = (GameData) d.Clone();
                            bestMove.Move = potentialMove;
                            bestMove.Max = BestXFromWinConditions(tmp, potentialMove);
                            UpdateWinCondition(potentialMove, ref tmp);
                            bestMove.Min = BestOFromWinConditions(d, potentialMove);


                        }
                        else
                        {
                            LookaheadHelper tempLook = new LookaheadHelper(-1,-1,null);

                            GameMove potentialMove = new GameMove(i, j, k / 2, k % 2 == 0);

                            tempLook.Move = potentialMove;

                            GameData tmp = (GameData)d.Clone();

                            int tmpMax = BestXFromWinConditions(tmp, potentialMove);
                            UpdateWinCondition(potentialMove, ref tmp);
                            int tmpMin = BestOFromWinConditions(tmp, potentialMove);


                            if(tmpMax >= 5){
                                return potentialMove;
                            }
                           
                           
                            if ((tmpMax > bestMove.Max && tmpMin<5) || bestMove.Min >= 5)
                            {
                                bestMove = tempLook;
                            }
                        }

                    }
                }
            }
        }



        return bestMove.Move; ;
    }

    //gets the best move that the player could do with no lookahead
    static LookaheadHelper GetLookaheadPlayer(GameData d, GameMove g)
    {
        d = (GameData) d.Clone();
        UpdateWinCondition(g, ref d);

        LookaheadHelper best = new LookaheadHelper(-1, -1, null);
        List<TupleList<int, int>> possibleMoves = new List<TupleList<int, int>>();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (d.gameBoard[i, j] == TileVals.Blank)
                {
                    //for each rotation possible
                    for (int k = 0; k < 8; k++)
                    {
                        GameMove potentialMove = new GameMove(i, j, k / 2, k % 2 == 0);
                        int bestX = BestXFromWinConditions(d, potentialMove);
                      

                        if (bestX > best.Max)
                        {
                            best.Max = bestX;
                            best.Move = potentialMove;
                        }
                    }
                    possibleMoves.Add(new TupleList<int, int>(i, j));
                }
            }
        }
        return best;
    }

    /// <summary>
    /// Looks at the whole board, applies move, and sees what the greatest win val us
    /// </summary>
    /// <param name="d"></param>
    /// <param name="g"></param>
    /// <returns></returns>
    static int BestXFromWinConditions(GameData d, GameMove g)
    {
        d = (GameData)d.Clone();

        UpdateWinCondition(g, ref d);

        //x is 1s
        int max = -1;
        for (int i = 0; i < d.winValues.Length; i++)
        {


            int x = d.winValues[i];
            int Os = x / 10;
            int Xs = x % 10;
            //if a better win condition
            if (Os == 0 && Xs > max)
            {
                max = Xs;
            }
        }


        return max;
    }
    static int BestOFromWinConditions(GameData d, GameMove g)
    {
        d = (GameData)d.Clone();

        UpdateWinCondition(g, ref d);

        //x is 1s
        int max = -1;
        for (int i = 0; i < d.winValues.Length; i++)
        {


            int x = d.winValues[i];
            int Os = x / 10;
            int Xs = x % 10;
            //if a better win condition
            if (Xs == 0 && Os > max)
            {
                max = Os;
            }
        }


        return max;
    }

    static MyTuple<int, int> GetMin(TileVals[,] board)
    {
        int max = -1;
        int min = -1;

        return new MyTuple<int, int>(min, max);
    }


    static GameMove PentagoHeuristic(GameData d)
    {

        d = (GameData)d.Clone();


        for (int i = 0; i < d.winValues.Length; i++)
        {
            if (d.winValues[i] == 4 || d.winValues[i] == 40)
            {
                return GetFromLookaheadCPU(currentGameData);
                //var points = PointsFromWinCondition(i);
                //foreach (var item in points)
                //{
                //    if (d.gameBoard[item.Item1, item.Item2] == TileVals.Blank)
                //    {
                //        //int quad = GetQuadFromPoint(item.Item1, item.Item2);
                //        //return new GameMove(item.Item1, item.Item2, quad, false);
                //    }

                //}
            }
        }
        int turnCounter = GetTurns(d).Count / 2;
        turnCounter++;
        GameMove ret = new GameMove();
        ret.rotIndex = -1;
        //EARLY GAME
        //first turn
        if (turnCounter == 1)
        {
            //startQuadrant = r.Next(4);
            startQuadrant = 1;
            if (startQuadrant == 0)
            {
                /*
                xVal = 2;
                yVal = 2;
                RotateSquare(board, 3, true);
                */
                ret = new GameMove(2, 2, 3, true);
            }
            else if (startQuadrant == 1)
            {
                /*
                xVal = 3;
                yVal = 2;
                RotateSquare(board, 2, true);
                */
                ret = new GameMove(3, 2, 2, true);
            }
            else if (startQuadrant == 2)
            {
                /*
                xVal = 2;
                yVal = 3;
                RotateSquare(board, 1, true);
                */
                ret = new GameMove(2, 3, 1, true);
            }
            else if (startQuadrant == 3)
            {
                /*
                xVal = 3;
                yVal = 3;
                RotateSquare(board, 0, true);
                */
                ret = new GameMove(3, 3, 0, true);
            }
        }
        //second turn
        else if (turnCounter == 2)
        {
            if (startQuadrant == 0)
            {
                if ((int)d.gameBoard[2, 0] == 0)
                {
                    ret = new GameMove(2, 0, 3, true);
                }
                else if ((int)d.gameBoard[0, 2] == 0)
                {
                    ret = new GameMove(0, 2, 3, true);
                }
                else
                {
                    ret = new GameMove(2, 2, 3, true);
                }
            }
            if (startQuadrant == 1)
            {
                if ((int)d.gameBoard[3, 0] == 0)
                {
                    ret = new GameMove(3, 0, 0, true);
                }
                else if ((int)d.gameBoard[5, 2] == 0)
                {
                    ret = new GameMove(5, 2, 2, true);
                }
                else
                {
                    ret = new GameMove(3, 2, 2, true);
                }
            }
            if (startQuadrant == 2)
            {
                if ((int)d.gameBoard[2, 5] == 0)
                {
                    ret = new GameMove(2, 5, 1, true);
                }
                else if ((int)d.gameBoard[0, 3] == 0)
                {
                    ret = new GameMove(0, 3, 1, true);
                }
                else
                {
                    ret = new GameMove(2, 3, 1, true);
                }
            }
            if (startQuadrant == 3)
            {
                if ((int)d.gameBoard[3, 5] == 0)
                {
                    ret = new GameMove(3, 5, 0, true);
                }
                else if ((int)d.gameBoard[5, 3] == 0)
                {
                    ret = new GameMove(5, 3, 0, true);
                }
                else
                {
                    ret = new GameMove(3, 3, 0, true);
                }
            }
        }
        //third turn
        else if (turnCounter == 3)
        {
            if (startQuadrant == 0)
            {
                if ((int)d.gameBoard[0, 0] == 0)
                {
                    /*
                    xVal = 0;
                    yVal = 0;
                    RotateSquare(d.gameBoard, 1, true);
                    */
                    ret = new GameMove(0, 0, 1, true);
                }
                else if ((int)d.gameBoard[0, 2] == 0)
                {
                    /*
                    xVal = 0;
                    yVal = 2;
                    RotateSquare(d.gameBoard, 1, true);
                    */
                    ret = new GameMove(0, 2, 1, true);
                }
                else
                {
                    /*
                    xVal = 0;
                    yVal = 5;
                    RotateSquare(d.gameBoard, 2, true);
                    */
                    ret = new GameMove(0, 5, 2, true);
                }
            }
            if (startQuadrant == 1)
            {
                if ((int)d.gameBoard[5, 0] == 0)
                {
                    /*
                    xVal = 5;
                    yVal = 0;
                    RotateSquare(d.gameBoard, 2, true);
                    */
                    ret = new GameMove(5, 0, 2, true);
                }
                else if ((int)d.gameBoard[5, 2] == 0)
                {
                    /*
                    xVal = 5;
                    yVal = 2;
                    RotateSquare(d.gameBoard, 2, true);
                    */
                    ret = new GameMove(5, 2, 2, true);
                }
                else
                {
                    /*
                    xVal = 5;
                    yVal = 5;
                    RotateSquare(d.gameBoard, 3, false);
                    */
                    ret = new GameMove(5, 5, 3, false);
                }
            }
            if (startQuadrant == 2)
            {
                if ((int)d.gameBoard[0, 5] == 0)
                {
                    /*
                    xVal = 0;
                    yVal = 5;
                    RotateSquare(d.gameBoard, 1, true);
                    */
                    ret = new GameMove(0, 5, 1, true);
                }
                else if ((int)d.gameBoard[0, 3] == 0)
                {
                    /*
                    xVal = 5;
                    yVal = 2;
                    RotateSquare(d.gameBoard, 1, true);
                    */
                    ret = new GameMove(5, 2, 1, true);
                }
                else
                {
                    /*
                    xVal = 0;
                    yVal = 0;
                    RotateSquare(d.gameBoard, 0, false);
                    */
                    ret = new GameMove(0, 0, 0, false);
                }
            }
            if (startQuadrant == 3)
            {
                if ((int)d.gameBoard[5, 5] == 0)
                {
                    /*
                    xVal = 5;
                    yVal = 5;
                    RotateSquare(d.gameBoard, 2, true);
                    */
                    ret = new GameMove(5, 5, 2, true);
                }
                else if ((int)d.gameBoard[5, 3] == 0)
                {
                    /*
                    xVal = 5;
                    yVal = 2;
                    RotateSquare(d.gameBoard, 2, true);
                    */
                    ret = new GameMove(5, 2, 2, true);
                }
                else
                {
                    /*
                    xVal = 5;
                    yVal = 0;
                    RotateSquare(d.gameBoard, 3, true);
                    */
                    ret = new GameMove(5, 0, 3, true);
                }
            }
        }
        //fourth turn
        else if (turnCounter == 4)
        {
            List<TupleList<int, int>> fourthTurnPoint = new List<TupleList<int, int>>();
            if (startQuadrant == 0 || startQuadrant == 3)
            {
                //FIRST HALF OF DIAGONAL EDGE CASES
                //directly through the middle cases
                if ((int)d.gameBoard[3, 2] + (int)d.gameBoard[2, 3] == 20)
                {
                    if ((int)d.gameBoard[4, 1] == 10)
                    {
                        /*
                        xVal = 1;
                        yVal = 4;
                        RotateSquare(d.gameBoard, 2, true);
                        */
                        ret = new GameMove(1, 4, 2, true);
                    }
                    else if ((int)d.gameBoard[1, 4] == 10)
                    {
                        /*
                        xVal = 4;
                        yVal = 1;
                        RotateSquare(d.gameBoard, 1, true);
                        */
                        ret = new GameMove(4, 1, 1, true);
                    }
                    else if ((int)d.gameBoard[1, 4] + (int)d.gameBoard[2, 3] == 20)
                    {
                        /*
                         xVal = 3;
                         yVal = 2;
                         RotateSquare(d.gameBoard, 2, true);                        
                         */
                        ret = new GameMove(3, 2, 2, true);
                    }
                    else if ((int)d.gameBoard[4, 1] + (int)d.gameBoard[3, 2] == 20)
                    {
                        /*
                         xVal = 2;
                         yVal = 3;
                         RotateSquare(d.gameBoard, 1, true);                        
                         */
                        ret = new GameMove(2, 3, 1, true);
                    }
                }
                else
                {
                    if (startQuadrant == 0)
                    {
                        int[] arr = { 0, 1, 4, 5, 12, 13, 16, 17 };
                        int min = d.winValues[arr[0]];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (d.winValues[arr[i]] < min)
                            {
                                min = d.winValues[arr[i]];
                            }
                            fourthTurnPoint.Add(PointsFromWinCondition(d.winValues[arr[i]]));
                        }
                        foreach (var pointArr in fourthTurnPoint)
                        {
                            foreach (var point in pointArr)
                            {
                                if (d.gameBoard[point.Item1, point.Item2] != TileVals.Blank)
                                {
                                    continue;
                                }
                                ret = new GameMove(point.Item1, point.Item2, GetQuadFromPoint(point.Item1, point.Item2), false);
                            }
                        }
                    }
                    else
                    {
                        int[] arr = { 6, 7, 10, 11, 18, 19, 22, 23 };
                        int min = d.winValues[arr[0]];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (d.winValues[arr[i]] < min)
                            {
                                min = d.winValues[arr[i]];
                            }
                            fourthTurnPoint.Add(PointsFromWinCondition(d.winValues[arr[i]]));
                        }
                        foreach (var pointArr in fourthTurnPoint)
                        {
                            foreach (var point in pointArr)
                            {
                                if (d.gameBoard[point.Item1, point.Item2] != TileVals.Blank)
                                {
                                    continue;
                                }
                                ret = new GameMove(point.Item1, point.Item2, GetQuadFromPoint(point.Item1, point.Item2), false);
                            }
                        }
                    }
                }
            }
            else
            {
                //SECOND HALF OF DIAGONAL EDGE CASES
                //directly through the middle cases
                if ((int)d.gameBoard[2, 2] + (int)d.gameBoard[3, 3] == 20)
                {
                    if ((int)d.gameBoard[4, 4] == 10)
                    {
                        /*
                        xVal = 1;
                        yVal = 1;
                        RotateSquare(d.gameBoard, 0, true);
                        */
                        ret = new GameMove(1, 1, 0, true);
                    }
                    else if ((int)d.gameBoard[1, 1] == 10)
                    {
                        /*
                        xVal = 4;
                        yVal = 4;
                        RotateSquare(d.gameBoard, 3, true);
                        */
                        ret = new GameMove(4, 4, 3, true);
                    }
                    else if ((int)d.gameBoard[1, 1] + (int)d.gameBoard[2, 2] == 20)
                    {
                        /*
                         xVal = 3;
                         yVal = 3;
                         RotateSquare(d.gameBoard, 2, true);                        
                         */
                        ret = new GameMove(3, 3, 3, true);
                    }
                    else if ((int)d.gameBoard[3, 3] + (int)d.gameBoard[4, 4] == 20)
                    {
                        /*
                         xVal = 2;
                         yVal = 2;
                         RotateSquare(d.gameBoard, 0, true);                        
                         */
                        ret = new GameMove(2, 2, 0, true);
                    }
                }
                else
                {

                    if (startQuadrant == 1)
                    {
                        int[] arr = { 0, 1, 4, 5, 18, 19, 22, 23 };
                        int min = d.winValues[arr[0]];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (d.winValues[arr[i]] < min)
                            {
                                min = d.winValues[arr[i]];
                            }
                            fourthTurnPoint.Add(PointsFromWinCondition(d.winValues[arr[i]]));
                        }
                        foreach (var pointArr in fourthTurnPoint)
                        {
                            foreach (var point in pointArr)
                            {
                                if (d.gameBoard[point.Item1, point.Item2] != TileVals.Blank)
                                {
                                    continue;
                                }
                                ret = new GameMove(point.Item1, point.Item2, GetQuadFromPoint(point.Item1, point.Item2), false);
                            }
                        }
                    }
                    else
                    {
                        int[] arr = { 6, 7, 10, 11, 12, 13, 16, 17 };
                        int min = d.winValues[arr[0]];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (d.winValues[arr[i]] < min)
                            {
                                min = d.winValues[arr[i]];
                            }
                            fourthTurnPoint.Add(PointsFromWinCondition(d.winValues[arr[i]]));
                        }
                        foreach (var pointArr in fourthTurnPoint)
                        {
                            foreach (var point in pointArr)
                            {
                                if (d.gameBoard[point.Item1, point.Item2] != TileVals.Blank)
                                {
                                    continue;
                                }
                                ret = new GameMove(point.Item1, point.Item2, GetQuadFromPoint(point.Item1, point.Item2), false);
                            }
                        }
                    }
                }
            }
        }

        else
        {




            int[,] zerothQuad = { { 2, 0 }, { 2, 1 }, { 2, 2 }, { 1, 2 }, { 0, 2 } };
            int zerothInt = GetSumFromPoints(d.gameBoard, zerothQuad);
            int[,] firstQuad = { { 3, 0 }, { 3, 1 }, { 3, 2 }, { 4, 2 }, { 5, 2 } };
            int firstInt = GetSumFromPoints(d.gameBoard, firstQuad);
            int[,] secondQuad = { { 0, 3 }, { 1, 3 }, { 2, 3 }, { 2, 4 }, { 2, 5 } };
            int secondInt = GetSumFromPoints(d.gameBoard, secondQuad);
            int[,] thirdQuad = { { 5, 5 }, { 4, 5 }, { 3, 5 }, { 3, 4 }, { 3, 3 } };
            int thirdInt = GetSumFromPoints(d.gameBoard, thirdQuad);
            if (turnCounter < 11)
            {

                List<TupleList<int, int>> possibleWinPoints = new List<TupleList<int, int>>();
                for (int i = 0; i < d.winValues.Length; i++)
                {
                    if ((d.winValues[i] > 1 && d.winValues[i] < 5) || (d.winValues[i] > 11 && d.winValues[i] < 15))
                    {
                        possibleWinPoints.Add(PointsFromWinCondition(i));

                    }
                }
                foreach (var pointArr in possibleWinPoints)
                {
                    foreach (var point in pointArr)
                    {
                        if (d.gameBoard[point.Item1, point.Item2] != TileVals.Blank)
                        {
                            continue;
                        }
                        if (
                            ((-1 < point.Item1 && point.Item1 < 3) && (-1 < point.Item2 && point.Item2 < 3))
                         || ((2 < point.Item1 && point.Item1 < 6) && (2 < point.Item2 && point.Item2 < 6)))
                        {
                            if (firstInt < secondInt)
                            {
                                ret = new GameMove(point.Item1, point.Item2, 1, false);
                            }
                            else
                            {
                                ret = new GameMove(point.Item1, point.Item2, 2, false);
                            }
                        }
                        else if (
                               ((2 < point.Item1 && point.Item1 < 6) && (-1 < point.Item2 && point.Item2 < 3))
                            || ((-1 < point.Item1 && point.Item1 < 3) && (2 < point.Item2 && point.Item2 < 6)))
                        {
                            if (zerothInt < thirdInt)
                            {
                                ret = new GameMove(point.Item1, point.Item2, 0, false);
                            }
                            else
                            {
                                ret = new GameMove(point.Item1, point.Item2, 3, false);
                            }
                        }
                    }
                }

            }
            //LATE GAME
            else
            {
                ret = GetFromLookaheadCPU(currentGameData);

                //List<TupleList<int, int>> possibleWinPoints = new List<TupleList<int, int>>();
                //for (int i = 0; i < d.winValues.Length; i++)
                //{
                //    if (d.winValues[i] >= 23)
                //    {
                //        possibleWinPoints.Add(PointsFromWinCondition(i));
                //    }
                //}
                ////for each possible win condition
                //foreach (var pointArr in possibleWinPoints)
                //{
                //    foreach (var point in pointArr)
                //    {

                //        if (
                //               ((-1 < point.Item1 && point.Item1 < 3) && (-1 < point.Item2 && point.Item2 < 3))
                //            || ((2 < point.Item1 && point.Item1 < 6) && (2 < point.Item2 && point.Item2 < 6))
                //            )
                //        {
                //            if (firstInt > secondInt)
                //            {
                //                ret = new GameMove(point.Item1, point.Item2, 1, true);
                //            }
                //            else
                //            {
                //                ret = new GameMove(point.Item1, point.Item2, 2, false);
                //            }
                //        }
                //        else if (
                //               ((2 < point.Item1 && point.Item1 < 6) && (-1 < point.Item2 && point.Item2 < 3))
                //            || ((-1 < point.Item1 && point.Item1 < 3) && (2 < point.Item2 && point.Item2 < 6)))
                //        {

                //            if (zerothInt > thirdInt)
                //            {
                //                ret = new GameMove(point.Item1, point.Item2, 0, false);
                //            }
                //            else
                //            {
                //                ret = new GameMove(point.Item1, point.Item2, 3, false);
                //            }
                //        }
                //    }
                //}
            }
            if (ret.rotIndex == -1)
            {
                throw new Exception("Ret was never initalized in the heuristic");
            }

        }
        if (d.gameBoard[ret.xCord, ret.yCord] != TileVals.Blank)
        {
            throw new Exception("The AI overwrote a user tile...");
        }
        return ret;
    }


    public enum TileVals
    {
        X = 1,
        O = 10,
        Blank = 0
    }


    public static string Run(string cmd)
    {
        cmd = cmd.Replace("\"", "\\\"");
        ProcessStartInfo start = new ProcessStartInfo();
        //start.FileName = "python";
        start.FileName = "/bin/bash";
        start.Arguments = "-l -c \"" + cmd + "\"";
        start.LoadUserProfile = true;
        start.UseShellExecute = false;// Do not use OS shell
        start.CreateNoWindow = true; // We don't need new window
        start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
        start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                return result;
            }
        }
    }

    static GameMove RunPythonThing(TileVals[,] board)
    {
        var dir = PlayerPrefs.GetString("dir").Replace(" ", "\\ ");
        var pyFile = PlayerPrefs.GetString("file");

        //var dir = "/Users/connorgoldsmith/Documents/Git/pentago-ai/4444\\ AI\\ Proj";
        var cmdSetup = "cd " + dir;
        var condaEnvSetup = "conda activate tensorflow_env";
        //var pyFile = "single_move.py";
        var gameboardString = EnumArrToNNArr(board);

        var lineCmd = string.Format("python {0} {1}", pyFile, gameboardString);
        var lineToActuallyRun = string.Format("source ~/.profile && {0} && {1} && {2}", cmdSetup, condaEnvSetup, lineCmd);

        var res = Run(lineToActuallyRun);

        Console.WriteLine(res);
        try
        {
            string[] rets = new string[4];
            string input = "";


            using (System.IO.StringReader reader = new System.IO.StringReader(res))
            {
                for (int i = 0; i < 4; i++)
                {
                    string line = reader.ReadLine();
                    input += line;
                    //rets[i] = line;
                }



            }
            var matches = Regex.Match(input, @".*x: %s (\d*)y: %s (\d*)quarter:  (\d*)direction:  (\d*).*");

            GameMove g = new GameMove
            (
                Int32.Parse(matches.Groups[1].Value),
                Int32.Parse(matches.Groups[2].Value),
                Int32.Parse(matches.Groups[3].Value),
                matches.Groups[4].Value == "1"
                );
            return g;
        }
        catch (Exception ex)
        {
            Console.WriteLine("NN did not return correct format");
            throw ex;
        }


    }

    static int GetSumFromPoints(TileVals[,] gameboard, int[,] pointsToSum)
    {
        int ret = 0;
        for (int i = 0; i < pointsToSum.GetLength(0); i++)
        {
            for (int j = 0; j < pointsToSum.GetLength(1); j++)
            {
                ret += (int)gameboard[i, j];
            }
        }
        return ret;
    }

}


#region Classes for helping with win conditions
public class CustomArray<T>
{
    public static T[] GetColumn(T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0) - 1)
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public static T[] GetRow(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1) - 1)
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

    public static T[] GetColumnMinusLast(T[,] matrix, int columnNumber)
    {
        //return Enumerable.Range(0, matrix.GetLength(0) - 1)
        //.Select(x => matrix[x, columnNumber])
        //.ToArray();
        var colLength = matrix.GetLength(0) - 1;
        var colVector = new T[colLength];

        for (var i = 0; i < colLength; i++)
        {
            colVector[i] = matrix[columnNumber, i];
        }

        return colVector;
    }

    public static T[] GetRowMinusLast(T[,] matrix, int rowNumber)
    {
        //return Enumerable.Range(0, matrix.GetLength(1) - 1)
        //.Select(x => matrix[rowNumber, x])
        //.ToArray();
        var rowLength = matrix.GetLength(1) - 1;
        var rowVector = new T[rowLength];

        for (var i = 0; i < rowLength; i++)
        {
            rowVector[i] = matrix[i, rowNumber];
        }

        return rowVector;
    }


    public static T[] GetColumnMinusFirst(T[,] matrix, int columnNumber)
    {
        //return Enumerable.Range(1, matrix.GetLength(0) - 1)
        //.Select(x => matrix[x, columnNumber])
        //.ToArray();
        var colLength = matrix.GetLength(0) - 1;
        var colVector = new T[colLength];

        for (var i = 0; i < colLength; i++)
        {
            colVector[i] = matrix[columnNumber, i + 1];
        }

        return colVector;
    }

    public static T[] GetRowMinusFirst(T[,] matrix, int rowNumber)
    {
        //return Enumerable.Range(1, matrix.GetLength(1) - 1)
        //.Select(x => matrix[rowNumber, x])
        //.ToArray();
        var rowLength = matrix.GetLength(1) - 1;
        var rowVector = new T[rowLength];

        for (var i = 0; i < rowLength; i++)
        {
            rowVector[i] = matrix[i + 1, rowNumber];
        }

        return rowVector;
    }
}

public class TupleList<T1, T2> : List<MyTuple<T1, T2>>
{
    public TupleList()
    {
    }
    public TupleList(T1 one, T2 two)
    {
        Add(one, two);
    }
    public void Add(T1 item, T2 item2)
    {
        Add(new MyTuple<T1, T2>(item, item2));
    }
}

public class MyTuple<T1, T2>
{
    private T1 _item1;
    private T2 _item2;
    public MyTuple(T1 item1, T2 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }

    public T1 Item1
    {
        get
        {
            return _item1;
        }

        set
        {
            _item1 = value;
        }
    }

    public T2 Item2
    {
        get
        {
            return _item2;
        }

        set
        {
            _item2 = value;
        }
    }

    public void Add(T1 item1, T2 item2)
    {
        this.Item1 = item1;
        this.Item2 = item2;
    }
}

public class GameMove : ICloneable
{
    public int xCord;
    public int yCord;
    public int rotIndex;
    public bool rotLeft;

    public GameMove()
    {
    }

    public GameMove(int xCord, int yCord, int rotIndex, bool rotLeft)
    {
        this.xCord = xCord;
        this.yCord = yCord;
        this.rotIndex = rotIndex;
        this.rotLeft = rotLeft;
    }


    public object Clone()

    {
        return new GameMove(this.xCord, this.yCord, this.rotIndex, this.rotLeft);
    }

    }
static class Extensions
{
    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }
}


#endregion