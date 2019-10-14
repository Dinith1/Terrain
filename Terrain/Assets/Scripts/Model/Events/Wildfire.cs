﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wildfire : Event
{
    private double probability;

    public Wildfire(Game game) : base(-5, -1, -5)
    {
        this.Type = EventType.Random;
        this.Description = "Severe heat and drought fuel wildfires, conditions scientists have linked to climate change. The hotter weather makes forests drier and more susceptible to burning with the average wildfire season three and a half months longer than it was a few decades back.";
        this.Game = game;
    }

    public double Probability { get => probability; set => probability = value; }

    public override void TileDelta(Tile[,] tiles, bool doDestroyBuildings)
    {

        if (!doDestroyBuildings)
        {
            return;
        }

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i, j].Building.GetType().Name.ToString() == "Forest")
                {
                    Debug.Log("Found a tile with a forest");
                    int random = Random.Range(0, 2);
                    // 50% chance to destroy forest
                    if (random == 1)
                    {
                        float buildingGreenGen = tiles[i, j].Building.GenerateGreen;
                        float buildingMoneyGen = tiles[i, j].Building.GenerateMoney;
                        float buildingHappinessGen = tiles[i, j].Building.GenerateHappiness;

                        if (tiles[i, j].removeBuilding())
                        {
                            Game.GenerateGreen = Game.GenerateGreen - buildingGreenGen;
                            Game.GenerateMoney = Game.GenerateMoney - buildingMoneyGen;
                            Game.GenerateHappiness = Game.GenerateHappiness - buildingHappinessGen;
                        }
                    }
                }
            }
        }
    }
}
