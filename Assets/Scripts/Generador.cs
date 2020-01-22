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
    PerlinNoiseCueva,
    RandomWalkCueva,
    TunelDireccional,
    MapaAleatorio,
    AutomataCelularMoore,
    AutomataCelularVonNeumann,
    Combinado
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

    [Header("Random Walk Prueba")]
    [Range(0, 1)]
    public float porcentajeAEliminar = 0.25f;
    public bool movimientoEnDiagonal = false;

    [Header("Tunel direccional cueva")]
    public int anchoMaximo = 4;
    public int anchoMinimo = 1;
    [Range(0, 1)]
    public float aspereza = 0.75f;
    public int desplazamientoMaximo = 2;
    public float desplazamiento = 0.75f; 

    [Header("Automata Celular")]
    [Range(0, 1)]
    public float porcentajeDeRelleno = 0.45f;
    public int totalDePasadas = 3;

    [Header("Combinado")]
    public int altoSuperficie = 20;

    public void GenerarMapa()
    {
        mapaDeLosetas.ClearAllTiles();
        int[,] mapa = null; 
        int[,] mapaSuperficie = null; 
        int[,] mapaGeneral = null;

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
            case Algoritmo.RandomWalkCueva:
                mapa = Metodos.GenerarArray(ancho, alto, false);
                mapa = Metodos.RandomWalkCueva(mapa, semilla, porcentajeAEliminar, losBordesSonMuros, movimientoEnDiagonal);
                break;
            case Algoritmo.TunelDireccional:
                mapa = Metodos.GenerarArray(ancho, alto, false);
                mapa = Metodos.TunelDireccional(mapa, semilla, anchoMinimo, anchoMaximo, aspereza, desplazamientoMaximo, desplazamiento);
                break;
            case Algoritmo.MapaAleatorio:
                mapa = Metodos.GenerarMapaAleatorio(ancho, alto, semilla, porcentajeDeRelleno, losBordesSonMuros);
                break;
            case Algoritmo.AutomataCelularMoore:
                mapa = Metodos.GenerarMapaAleatorio(ancho, alto, semilla, porcentajeDeRelleno, losBordesSonMuros);
                mapa = Metodos.AutomataCelularMoore(mapa, totalDePasadas, losBordesSonMuros);
                break;
            case Algoritmo.AutomataCelularVonNeumann:
                mapa = Metodos.GenerarMapaAleatorio(ancho, alto, semilla, porcentajeDeRelleno, losBordesSonMuros);
                mapa = Metodos.AutomataCelularVonNeumann(mapa, totalDePasadas, losBordesSonMuros);
                break;
            case Algoritmo.Combinado:
                mapa = Metodos.GenerarMapaAleatorio(ancho, alto, semilla, porcentajeDeRelleno, losBordesSonMuros);
                mapaSuperficie = Metodos.GenerarMapaAleatorio(ancho, altoSuperficie, semilla, 0.0f, false);
                mapa = Metodos.PerlinNoiseCueva(mapa, modificador, losBordesSonMuros, offsetX, offsetY, semilla);
                mapaSuperficie = Metodos.PerlinNoiseSuavizado(mapaSuperficie, semilla, intervalo);
                mapaGeneral = Metodos.JuntarArreglos(mapa, mapaSuperficie);
                mapaGeneral = Metodos.TunelDireccional(mapaGeneral, semilla, anchoMinimo, anchoMaximo, aspereza, desplazamientoMaximo, desplazamiento);
                Metodos.GenerarMapa(mapaGeneral, mapaDeLosetas, loseta, 0);
                break;
        }

                Metodos.GenerarMapa(mapa, mapaDeLosetas, loseta, 0);

        // = Metodos.GenerarArray(ancho, alto, false);
        // Metodos.GenerarMapa(mapa, mapaDeLosetas, loseta);
    }

    public void LimpiarMapa()
    {
        Debug.Log("Limpiar Mapa");
        mapaDeLosetas.ClearAllTiles();
    }
}
