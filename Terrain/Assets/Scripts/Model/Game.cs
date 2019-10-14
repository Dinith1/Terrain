﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class Game
{
    int rows;
    int columns;
    Tile[,] tiles;
    float money;
    float green;
    float happiness;
    float generateMoney;
    float generateGreen;
    float generateHappiness;
    Building[,] buildings;
    Event gameEvent;
    float currentTurn;
    float maxTurns;
    float maxGreen;
    bool isEnd = false;
    bool isVictory;
    bool isUnhappy = false;
    float moneyDelta;
    float greenDelta;

    public float modifier = 1;
    

    float prevMoney;
    float prevHappiness;


    GameObject errorMessage;


    public int Rows { get => rows; }
    public int Columns { get => columns; }
    public float Money { get => money; set => money = value; }
    public float Green { get => green; set => green = value; }
    public float Happiness { get => happiness; set => happiness = value; }
    public float GenerateMoney { get => generateMoney; set => generateMoney = value; }
    public float GenerateGreen { get => generateGreen; set => generateGreen = value; }
    public float GenerateHappiness { get => generateHappiness; set => generateHappiness = value; }
    public float CurrentTurn { get => currentTurn; set => currentTurn = value; }
    public float MaxTurns { get => maxTurns; set => maxTurns = value; }
    public float MaxGreen { get => maxGreen; set => maxGreen = value; }
    public bool IsEnd { get => isEnd; set => isEnd = value; }
    public bool IsVictory { get => isVictory; set => isVictory = value; }
    public Event GameEvent { get => gameEvent; set => gameEvent = value; }
    public float MoneyDelta { get => moneyDelta; set => moneyDelta = value; }
    public float GreenDelta { get => greenDelta; set => greenDelta = value; }

    public Game(int rows = 30, int columns = 30)
    {
        this.isEnd = false;
        this.currentTurn = 0;
        this.rows = rows;
        this.columns = columns;
        tiles = new Tile[rows, columns];
        buildings = new Building[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                tiles[i, j] = new Tile(this, i, j);
                tiles[i, j].registerMethodCallbackTypeChanged(stillBuildable);
            }
        }
        Debug.Log("game created");
    }


    public Tile getTileAt(int x, int y)
    {
        if (x >= rows || x < 0 || y >= columns || y < 0)
        {
            return null;
        }
        return tiles[x, y];
    }

    public void InitialiseMetrics(float money, float green, float happiness, float maxGreen)
    {
        Money = money;
        Green = green;
        Happiness = happiness;
        MaxGreen = maxGreen;
    }

    public void InitialiseTurns(float currentTurn, float maxTurn)
    {
        CurrentTurn = currentTurn;
        MaxTurns = maxTurn;
    }

    /* This method proceeds with the next turn after the user clicks the 
     * end turn button. It increments the accumulated points and shows it on 
     * the metrics
     */
    public void nextTurn()
    {
        this.currentTurn++;


        getModifier();

        Happiness = Happiness + GenerateHappiness;


        // Increase the metrics
        if (GenerateMoney > 0)
        {
            moneyDelta = GenerateMoney * modifier;
        } else
        {
            moneyDelta = GenerateMoney *  (1 / modifier) ;
        }


        if(GenerateGreen > 0)
        {
            greenDelta = GenerateGreen * modifier;
        } else
        {
            greenDelta = GenerateGreen * (1 / modifier);
        }
        moneyDelta = (float)System.Math.Round(moneyDelta, 2);
        greenDelta = (float)System.Math.Round(greenDelta, 2);

        Money = Money + moneyDelta;
        Green = Green + greenDelta;

        Debug.Log("Generate Money: " +GenerateMoney);
        Debug.Log("MoneyDelta: " + moneyDelta);
        Debug.Log("Modifier: " + modifier);


        // Check if the user has won the game by reaching the number of green
        // points required
        if (this.green >= maxGreen)
        {
            this.endGame(true);
            // Check if the user has lost the game by exceeding the max number
            // of turns allowed, or having a negative money value (as they
            // now are stuck in debt)

            return;
        }
        else if (currentTurn >= maxTurns || Money < 0)
        {
            this.endGame(false);
            return;
        }

        GameEvent = EventForNextTurn();

        if (GameEvent != null)
        {
            GenerateMoney = GenerateMoney + GameEvent.MoneyDelta;
            GenerateHappiness = GenerateHappiness + GameEvent.HappinessDelta;
            GenerateGreen = GenerateGreen + GameEvent.GreenPointDelta;
            GameEvent.TileDelta(tiles);       
        }


    }

    public void endGame(bool isVictory)
    {
        this.isEnd = true;
        this.IsVictory = isVictory;
    }

    public Building addBuildingToTile(string buildingType, Tile tile)
    {
        Building building = null;
        switch (buildingType)
        {
            case "Hydro Plant":
                building = new Hydro();
                break;
            case "Coal Mine":
                building = new CoalMine();
                break;
            case "Zoo":
                building = new Zoo();
                break;
            case "Wind Turbine":
                building = new WindTurbine();
                break;
            case "Solar Farm":
                building = new SolarFarm();
                break;
            case "Race Track":
                building = new RaceTrack();
                break;
            case "Oil Refinery":
                building = new OilRefinery();
                break;
            case "Nuclear Plant":
                building = new Nuclear();
                break;
            case "National Park":
                building = new NationalPark();
                break;
            case "Movie Theatre":
                building = new MovieTheatre();
                break;
            case "Forest":
                building = new Forest();
                break;
            case "Town Hall":
                building = new TownHall();
                break;
            default:
                return null;
        }


        // Check if funds are sufficient
        if (Money + building.InitialBuildMoney >= 0)
        {
            if (tile.placeBuilding(building))
            {
                buildings[tile.X, tile.Y] = building;
                UpdateMetrics(building);
                return building;
            }
            else
            {
                if (tile.Building != null)
                {
                    GameController.Instance.ShowError("Another building already exists on this tile.");
                }
                else
                {
                    // Show error message
                    GameController.Instance.ShowError(building.Name + " cannot be built on a " + tile.Type + " tile.");
                }

                return null;
            }
        }
        else
        {

            // Show error message
            GameController.Instance.ShowError("You do not have enough money to build a " + building.Name + ". ");

            return null;

        }


    }

    public void SellBuilding(Tile tile)
    {
        Debug.Log("Yeet");
        Building building = tile.Building;
        float CostToSell = building.InitialBuildMoney * (float)0.25 * -1;
        if (tile.removeBuilding())
        {
            buildings[tile.X, tile.Y] = null;
            Money += CostToSell;
            GenerateHappiness -= building.GenerateHappiness;    
            GenerateMoney -= building.GenerateMoney;
            GenerateGreen -= building.GenerateGreen;
            GameController.Instance.SetMetrics(Money, Green, Happiness);
            GameController.Instance.SetDelta(GenerateMoney, GenerateGreen, GenerateHappiness);

        }



    }


    // get the event  for the next turn
    public Event EventForNextTurn()
    {
        List<Event> randomEventList = InitaliseRandomEventList();
        Random random = new Random();
        if (currentTurn == 5)
        {
            return new Drought(this);
        }
        else if (Random.Range(0, 100) < 10)
        {
            return randomEventList[Random.Range(0, randomEventList.Count)];
        }

        return null;
    }


    // method to create list of all random events
    public List<Event> InitaliseRandomEventList()
    {
        List<Event> randomEventList = new List<Event>();

        randomEventList.Add(new AcidRain(this));
        randomEventList.Add(new Drought(this));
        randomEventList.Add(new Flood(this));
        randomEventList.Add(new Hurricane(this));
        randomEventList.Add(new ForestSpawn(this));
        randomEventList.Add(new RisingSeaLevel(this));
        randomEventList.Add(new Wildfire(this));
        randomEventList.Add(new HeatWave(this));

        return randomEventList;
    }

    // Change the metrics with regards to the effects of the building
    // that has just been placed.
    public void UpdateMetrics(Building building)
    {


        Money += building.InitialBuildMoney;
        Green += building.InitialBuildGreen;

        if (Happiness + building.InitialBuildHappiness < 0)
        {
            Happiness = 0;
        }
        else if (Happiness + building.InitialBuildHappiness > 100)
        {
            Happiness = 100;
        }
        else
        {
            Happiness += building.InitialBuildHappiness;
            
        }

        //getModifier();


        GenerateMoney += building.GenerateMoney;
        GenerateGreen += building.GenerateGreen;
        GenerateHappiness += building.GenerateHappiness;

        //if (GenerateMoney > 0)
        //{
        //    moneyDelta = GenerateMoney * modifier;
        //}
        //else
        //{
        //    moneyDelta = GenerateMoney * (1 / modifier);
        //}

        //if (GenerateGreen > 0)
        //{
        //    greenDelta = GenerateGreen * modifier;
        //}
        //else
        //{
        //    greenDelta = GenerateGreen * (1 / modifier);
        //}


        GameController.Instance.SetMetrics(Money, Green, Happiness);
        GameController.Instance.SetDelta(MoneyDelta, GreenDelta, GenerateHappiness);


    }
    
    public void stillBuildable(Tile tile)
    {
        Debug.Log("still buildable called");
        if (tile.Building != null)
        {
            if (!tile.IsBuildable(tile.Building))
            {
                GenerateGreen = GenerateGreen - tile.Building.GenerateGreen;
                GenerateMoney = GenerateMoney - tile.Building.GenerateMoney;
                GenerateHappiness = GenerateHappiness - tile.Building.GenerateHappiness;
            }
        }
    }

    private void CheckHappiness()
    {
        if (Happiness < 50)
        {
            
        } else
        {

        }
    }


    private void getModifier()
    {
        if (Happiness >= 50 && Happiness + GenerateHappiness < 50)
        {
            Debug.Log("50 down");

            modifier -= (float)0.1;
        }

        if (Happiness < 50 && Happiness + GenerateHappiness >= 50)
        {
            Debug.Log("50 up");

            modifier += (float)0.1;

        }

        if (Happiness >= 30 && Happiness + GenerateHappiness < 30)
        {
            Debug.Log("30 down");

            modifier -= (float)0.1;
        }

        if (Happiness < 30 && Happiness + GenerateHappiness >= 30)
        {
            Debug.Log("30 up");

            modifier += (float)0.1;
        }

        if (Happiness < 70 && Happiness + GenerateHappiness >= 70)
        {
            Debug.Log("70 up");

            modifier += (float)0.1;
        }

        if (Happiness >= 70 && Happiness + GenerateHappiness < 70)
        {
            Debug.Log("70 down");

            modifier -= (float)0.1;
        }

        if (Happiness < 90 && Happiness + GenerateHappiness >= 90)
        {
            Debug.Log("90 up");

            modifier += (float)0.1;
        }

        if (Happiness >= 90 && Happiness + GenerateHappiness < 90)
        {
            Debug.Log("90 down");
            modifier -= (float)0.1;
        }
    }
}
