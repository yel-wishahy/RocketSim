using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public Material planetMaterial;
    public float planetSize = 1f;
    GameObject planet;
    Mesh planetMesh;
    Vector3[] planetVertices;
    int[] planetTriangles;
    MeshRenderer planetMeshRenderer;
    MeshFilter planetMeshFilter;
    MeshCollider planetMeshCollider;

    public void Start()
    {
        CreatePlanet();
    }

    public void CreatePlanet()
    {
        CreatePlanetGameObject();
        //do whatever else you need to do with the sphere mesh
        RecalculateMesh();
    }

    void CreatePlanetGameObject()
    {
        planet = new GameObject();
        planetMeshFilter = planet.AddComponent<MeshFilter>();
        planetMesh = planetMeshFilter.mesh;
        planetMeshRenderer = planet.AddComponent<MeshRenderer>();
        //need to set the material up top
        planetMeshRenderer.material = planetMaterial;
        planet.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
        IcoSphere.Create(planet);
    }

    void RecalculateMesh()
    {
        planetMesh.RecalculateBounds();
        planetMesh.RecalculateTangents();
        planetMesh.RecalculateNormals();
    }
}
