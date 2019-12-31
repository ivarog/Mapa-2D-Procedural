using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Metodos 
{
    public static int[,] GenerarArray(int ancho, int alto, bool vacio)
    {
        int[,] mapa = new int[ancho, alto];
        for(int x = 0; x < ancho; x++)
        {
            for(int y = 0; y < alto; y++)
            {
                mapa[x, y] = vacio ? 0 : 1;
            }
        }
        return mapa;
    }

    public static void GenerarMapa(int[,] mapa, Tilemap mapaDeLosetas, TileBase loseta)
    {
        mapaDeLosetas.ClearAllTiles();

        for(int x = 0; x < mapa.GetUpperBound(0); x++)
        {
            for(int y = 0; y < mapa.GetUpperBound(1); y++)
            {
                if(mapa[x, y] == 1)
                {
                    mapaDeLosetas.SetTile(new Vector3Int(x, y, 0), loseta);
                }
            }
        }
    }

    public static int [,] PerlinNoise(int [,] mapa, float semilla)
    {
        int nuevoPunto;

        //Como Mathf.PerlinNoise devuelve enrte 0 o 1 le restamos esta variable para que el valor final sea  entre -0.5 y 0.5
        float reduccion = 0.5f;
        //Crear PrlinNoise
        for(int x = 0; x < mapa.GetUpperBound(0); x++)
        {
            nuevoPunto = Mathf.FloorToInt((Mathf.PerlinNoise(x, semilla) - reduccion) * mapa.GetUpperBound(1));
            nuevoPunto += (mapa.GetUpperBound(1) / 2);
            for(int y = nuevoPunto; y >= 0; y--)
            {
                mapa[x, y] = 1;
            }
        }

        return mapa;
    }

    public static int[,] PerlinNoiseSuavizado(int[,] mapa, float semilla, int intervalo)
    {
        if(intervalo > 1)
        {
            Vector2Int posicionActual, posicionAnterior;
            List<int> ruidoX = new List<int>();
            List<int> ruidoY = new List<int>();

            int nuevoPunto, puntos;

            //Genera el ruido
            for(int x = 0; x < mapa.GetUpperBound(0) + intervalo; x += intervalo)
            {
                nuevoPunto = Mathf.FloorToInt(Mathf.PerlinNoise(x, semilla) * mapa.GetUpperBound(1));
                ruidoY.Add(nuevoPunto);
                ruidoX.Add(x);
            }
            puntos = ruidoY.Count;

            for(int i = 1; i < puntos; i++)
            {
                //Obtenemos la posicion actual
                posicionActual = new Vector2Int(ruidoX[i], ruidoY[i]);
                //Obtenemos la posicion anterior
                posicionAnterior = new Vector2Int(ruidoX[i -1], ruidoY[i -1]);

                //Calculamos la diferencia entre las dos
                Vector2 diferencia = posicionActual - posicionAnterior;
                //Calculamos el cambio de altura
                float cambioEnAltura = diferencia.y / intervalo;
                //Guardamos la altura actual
                float alturaActual = posicionAnterior.y;

                //Genneramos los bloques de denro del intervalo desde la x anterioir hasta la x actual
                for(int x = posicionAnterior.x; x < posicionActual.x && x < mapa.GetUpperBound(0); x++)
                {
                    //Empezamos desde la altura actual
                    for(int y = Mathf.FloorToInt(alturaActual); y >= 0; y--)
                    {
                        mapa[x, y] = 1;
                    }

                    alturaActual += cambioEnAltura;
                }
            }
        }
        else
        {
            mapa = PerlinNoise(mapa, semilla);
        }

        return mapa;
    }
}
