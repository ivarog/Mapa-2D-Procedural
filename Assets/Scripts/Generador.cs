using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Algoritmo
{
    PerlinNoise,
    PerlinNoiseSuavizado,
    RandomWalk,
    RandomWalkSuavizado,
    PerlinNoiseCueva
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

    [Header("Random Walk Suavizado")]
    public int minimoAnchoSeccion = 2;

    [Header("Cuevas")]
    public bool losBordesSonMuros = true;

    [Header("Perlin Noise Cueva")]
    public float modificador = 0.1f;
    public float offsetX = 0.0f;
    public float offsetY = 0.0f;

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
            case Algoritmo.RandomWalk:
                mapa = Metodos.GenerarArray(ancho, alto, true);
                mapa = Metodos.RandomWalk(mapa, semilla);
                break;
            case Algoritmo.RandomWalkSuavizado:
                mapa = Metodos.GenerarArray(ancho, alto, true);
                mapa = Metodos.RandomWalkSuavizado(mapa, semilla, minimoAnchoSeccion);
                break;
            case Algoritmo.PerlinNoiseCueva:
                mapa = Metodos.GenerarArray(ancho, alto, false);
                mapa = Metodos.PerlinNoiseCueva(mapa, modificador, losBordesSonMuros, offsetX, offsetY, semilla);
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
