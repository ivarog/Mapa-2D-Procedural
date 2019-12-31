using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Algoritmo
{
    PerlinNoise,
    PerlinNoiseSuavizado
}

public class Generador : MonoBehaviour
{

    // private void Start() 
    // {
    //     GenerarMapa();
    // }

    // private void Update() 
    // {
    //     if(Input.GetKeyDown(KeyCode.G) || Input.GetMouseButtonDown(0))
    //     {
    //         GenerarMapa();
    //     }    
    //     if(Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(1))
    //     {
    //         LimpiarMapa();
    //     }
    // }

    [Header("Referencias")]
    public Tilemap mapaDeLosetas;
    public TileBase loseta;

    [Header("Dimensiones")]
    public int ancho = 60;
    public int alto = 34;

    [Header("Semilla")]
    public float semilla = 0;
    public bool semillaAleatoria = true;

    [Header("Algoritmo")]
    public Algoritmo algoritmo = Algoritmo.PerlinNoise;

    [Header("Perlin Noise Suavizado")]
    public int intervalo = 2;


    public void GenerarMapa()
    {
        mapaDeLosetas.ClearAllTiles();
        int[,] mapa = null; 

        if(semillaAleatoria)
        {
            semilla = Random.Range(0f, 1000f);
        }

        switch(algoritmo)
        {
            case Algoritmo.PerlinNoise:
                mapa = Metodos.GenerarArray(ancho, alto, true);
                mapa = Metodos.PerlinNoise(mapa, semilla);
                break;
            case Algoritmo.PerlinNoiseSuavizado:
                mapa = Metodos.GenerarArray(ancho, alto, true);
                mapa = Metodos.PerlinNoiseSuavizado(mapa, semilla, intervalo);
                break;

        }

        Metodos.GenerarMapa(mapa, mapaDeLosetas, loseta);

        // = Metodos.GenerarArray(ancho, alto, false);
        // Metodos.GenerarMapa(mapa, mapaDeLosetas, loseta);
    }

    public void LimpiarMapa()
    {
        Debug.Log("Limpiar Mapa");
        mapaDeLosetas.ClearAllTiles();
    }
}
