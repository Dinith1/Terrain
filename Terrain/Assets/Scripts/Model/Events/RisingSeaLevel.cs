using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingSeaLevel : Event
{
    private double probability;

    public RisingSeaLevel(Game game) : base(-150, -5, 0)
    {
        this.Type = EventType.TileChanger;
        this.Game = game;
        this.Title = "Rising Sea Level";
        this.Description =  "This is the result of extra water from melting glaciers and increasing sea levels from global warming." +
            " Rising sea levels is extremely worrying and harmful for the environment and future. " +
            "Events like rising sea levels have a higher chance of happening the lower your green points are " +
            "and the longer it takes you to reach 1000 green points! Thankfully, sea levels won't rise for the foreseeable" +
            "future.";
        this.TileDeltaDesc = "The outer tiles of your map has been converted to water tiles.";
    }

    public double Probability { get => probability; set => probability = value; }



    public override void TileDelta(Tile[,] tiles, bool doDestroyBuildings)
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (i == 0)
                {
                    tiles[i, j].Type = Tile.TileType.Water;
                }
                else if (i == tiles.GetLength(0) - 1)
                {
                    tiles[i, j].Type = Tile.TileType.Water;
                }

                else if (j == 0)
                {
                    tiles[i, j].Type = Tile.TileType.Water;
                }
                else if (j == tiles.GetLength(1) - 1)
                {
                    tiles[i, j].Type = Tile.TileType.Water;
                }
            }
        }
    }

}
