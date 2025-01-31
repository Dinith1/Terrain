﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
This class controls the placement of buildings on the map. This includes
permanantly placing buildings on the map, and temporarily placing a preview
of a building at the player's cursor point.
*/
public class BuildingController : MonoBehaviour
{
    private Dictionary<string, GameObject> modelDictionary = new Dictionary<string, GameObject>();
    public GameObject model_AnimalFarm;
    public GameObject model_BeeFarm;
    public GameObject model_CoalMine;
    public GameObject model_Factory;
    public GameObject model_Forest;
    public GameObject model_Greenhouse;
    public GameObject model_Hydro;
    public GameObject model_MovieTheatre;
    public GameObject model_NationalPark;
    public GameObject model_Nuclear;
    public GameObject model_OilRefinery;
    public GameObject model_Pollutant;
    public GameObject model_RaceTrack;
    public GameObject model_RecyclingPlant;
    public GameObject model_SolarFarm;
    public GameObject model_TownHall;
    public GameObject model_VegetableFarm;
    public GameObject model_WindTurbine;
    public GameObject model_Zoo;
    private GameObject previewBuilding;
    public static BuildingController Instance { get; protected set; }

    void Awake()
    {
        Instance = this;

        // Add models to dictionary for easy access when building
        modelDictionary.Add("AnimalFarm", model_AnimalFarm);
        modelDictionary.Add("BeeFarm", model_BeeFarm);
        modelDictionary.Add("CoalMine", model_CoalMine);
        modelDictionary.Add("Factory", model_Factory);
        modelDictionary.Add("Forest", model_Forest);
        modelDictionary.Add("Greenhouse", model_Greenhouse);
        modelDictionary.Add("Hydro", model_Hydro);
        modelDictionary.Add("MovieTheatre", model_MovieTheatre);
        modelDictionary.Add("NationalPark", model_NationalPark);
        modelDictionary.Add("Nuclear", model_Nuclear);
        modelDictionary.Add("OilRefinery", model_OilRefinery);
        modelDictionary.Add("Pollutant", model_Pollutant);
        modelDictionary.Add("RaceTrack", model_RaceTrack);
        modelDictionary.Add("RecyclingPlant", model_RecyclingPlant);
        modelDictionary.Add("SolarFarm", model_SolarFarm);
        modelDictionary.Add("TownHall", model_TownHall);
        modelDictionary.Add("VegetableFarm", model_VegetableFarm);
        modelDictionary.Add("WindTurbine", model_WindTurbine);
        modelDictionary.Add("Zoo", model_Zoo);
    }

    /*
    Adds the specified building to a specific tile. Returns true if the building
    has been successfully built, and false otherwise.
    */
    public bool addBuildingToTile(string buildingType, Tile tile)
    {
        Building building = GameController.Instance.Game.addBuildingToTile(buildingType, tile);
        return (building != null);
    }

    /*
    Updates the tile with the speicifed building
    */
    public void ChangeBuildingModel(Tile tile, GameObject buildingGO)
    {
        // Remove the old building's callback method
        tile.unregisterMethodCallbackBuildingCreated((tileBuildingData) => { BuildingController.Instance.ChangeBuildingModel(tileBuildingData, buildingGO); });

        // Place the new building
        GameObject newBuilding = PlaceBuildingOnMap(tile, buildingGO);

        // Add the new building's callback method
        tile.registerMethodCallbackBuildingCreated((tileBuildingData) => { BuildingController.Instance.ChangeBuildingModel(tileBuildingData, newBuilding); });
    }

    /*
    Places the building on the tile in the game world
    */
    private GameObject PlaceBuildingOnMap(Tile tile, GameObject building)
    {
        string newBuildingModel = null;
        GameObject newBuilding;
        if (tile.Building != null)
        {
            // Get the class name of the building
            newBuildingModel = tile.Building.GetType().Name;

            // Create new building GameObject
            newBuilding = Instantiate(modelDictionary[tile.Building.GetType().Name]);
        }
        else
        {
            newBuilding = new GameObject();
        }

        // Set the building GameObject's name and position
        newBuilding.name = "Building(" + tile.X + ", " + tile.Y + ", " + tile.Z + ")";
        newBuilding.transform.position = new Vector3(tile.X, tile.Y, tile.Z);

        if (newBuildingModel != null)
        {
            if (newBuildingModel == "TownHall")
            {
                newBuilding.transform.position = new Vector3(tile.X - 0.3f, (float)tile.Y + 0.2f, tile.Z - 0.3f);
            }
            else if (newBuildingModel == "NuclearPlant")
            {
                newBuilding.transform.position = new Vector3(tile.X + 0.2f, (float)tile.Y + 0.2f, tile.Z + 0.2f);
            }
            else if (newBuildingModel != "CoalMine" && newBuildingModel != "Hydro")
            {
                newBuilding.transform.position = new Vector3(tile.X, (float)tile.Y + 0.1f, tile.Z);
            }
        }

        // Delete old (possibly empty) building GameObject
        Destroy(building);

        return newBuilding;
    }

    /*
    Shows a preview of the building that the player has selected from the shop at the cursor point.
    The preview will be green if the building can be built on the tile, and red otherwise.
    */
    public void ShowBuildingPreview(string name, Vector3 mousePoint)
    {
        // Remove the preview building from where the cursor previously was
        HideBuildingPreview();

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // Instantiate the preview building GameObject in the world
            string buildingClassName = Building.resolveBuildingClassName(name);
            previewBuilding = Instantiate(modelDictionary[buildingClassName]);
            previewBuilding.name = "PreviewBuilding";

            // Remove the preview object's box collider to prevent a hover changing its colour
            Destroy(previewBuilding.GetComponent<Collider>());

            // Show the preview building on the cursor point
            previewBuilding.transform.position = mousePoint;

            if (buildingClassName != null)
            {
                if (buildingClassName == "TownHall")
                {
                    previewBuilding.transform.position = new Vector3(mousePoint.x - 0.3f, (float)mousePoint.y + 0.2f, mousePoint.z - 0.3f);
                }
                else if (buildingClassName == "NuclearPlant")
                {
                    previewBuilding.transform.position = new Vector3(mousePoint.x + 0.2f, (float)mousePoint.y + 0.2f, mousePoint.z + 0.2f);
                }
                else if (buildingClassName != "CoalMine" && buildingClassName != "Hydro")
                {
                    previewBuilding.transform.position = new Vector3(mousePoint.x, (float)mousePoint.y + 0.1f, mousePoint.z);
                }
            }

            if (canBuildOnPoint(buildingClassName, mousePoint))
            {
                // Set the preview building to a green colour
                previewBuilding.GetComponent<Renderer>().material.color = new Color32(0, 200, 0, 100);
            }
            else
            {
                // Set the preview building to a red colour
                previewBuilding.GetComponent<Renderer>().material.color = new Color32(200, 0, 0, 100);
            }
        }
    }

    /*
    Remove the preview of the building
    */
    public void HideBuildingPreview()
    {
        Destroy(previewBuilding);
    }

    /*
    Returns true if the building can be built on the point, false otherwise.
    */
    private bool canBuildOnPoint(string buildingClassName, Vector3 point)
    {
        // Get the tile at the specified point
        Tile tile = GameController.Instance.Game.getTileAt((int)point.x, (int)point.z);

        if (tile != null)
        {
            // Create a temporary building for the specified building name
            Building building = (Building)Activator.CreateInstance(Type.GetType(buildingClassName));

            // Check if the tile already has a building on it
            if (tile.Building != null) return false;

            // Check the building can be built on the tile type
            if (tile.IsBuildable(building)) return true;
        }

        return false;
    }

}