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

    public static int[,] RandomWalk(int[,] mapa, float semilla)
    {
        //La semilla de nuestro random
        Random.InitState(semilla.GetHashCode());

        //Establecemos la altura inicial
        int ultimaAltura = Random.Range(0, mapa.GetUpperBound(1));

        //Recorremos todo el mapa a lo ancho
        for(int x = 0; x < mapa.GetUpperBound(0); x++)
        {
            //0 sube 1 baja 2 igual
            int siguienteMovimiento = Random.Range(0, 3);

            if(siguienteMovimiento == 0 && ultimaAltura < mapa.GetUpperBound(1))
            {
                ultimaAltura++;
            }
            else if(siguienteMovimiento == 1 && ultimaAltura > 0)
            {
                ultimaAltura--;
            }

            for(int y = ultimaAltura; y >= 0; y--)
            {
                mapa[x, y] = 1;
            }
        }

        return mapa;

    }

    public static int[,] RandomWalkSuavizado(int[,] mapa, float semilla, int minimoAnchoSeccion)
    {
         //La semilla de nuestro random
        Random.InitState(semilla.GetHashCode());

        //Establecemos la altura inicial
        int ultimaAltura = Random.Range(0, mapa.GetUpperBound(1));

        //Para llevar la cuenta
        int anchoSeccion = 0;

        //Recorremos todo el mapa a lo ancho
        for(int x = 0; x < mapa.GetUpperBound(0); x++)
        {
            if(anchoSeccion >= minimoAnchoSeccion)
            {
                //0 sube 1 baja 2 igual
                int siguienteMovimiento = Random.Range(0, 3);

                if(siguienteMovimiento == 0 && ultimaAltura < mapa.GetUpperBound(1))
                {
                    ultimaAltura++;
                }
                else if(siguienteMovimiento == 1 && ultimaAltura > 0)
                {
                    ultimaAltura--;
                }

                anchoSeccion = 0;
            }

            anchoSeccion++;

            //Relleno del suelo
            for(int y = ultimaAltura; y >= 0; y--)
            {
                mapa[x, y] = 1;
            }
        }

        return mapa;
    }

    public static int[,] PerlinNoiseCueva(int[,] mapa, float modificador, bool losBordesSonMuros, float offsetX = 0.0f, float offfsetY =0.0f, float semilla = 0.0f)
    {
        int nuevoPunto;

        for(int x = 0; x < mapa.GetUpperBound(0); x++)
        {
            for(int y = 0; y < mapa.GetUpperBound(1); y++)
            {
                if(losBordesSonMuros && (x == 0 || y == 0 || x == mapa.GetUpperBound(0) - 1 || y == mapa.GetUpperBound(1) - 1))
                {
                    mapa[x, y] = 1;
                }
                else
                {
                    nuevoPunto = Mathf.RoundToInt(Mathf.PerlinNoise(x * modificador + offsetX + semilla, y * modificador + offfsetY + semilla));
                    mapa[x, y] = nuevoPunto;
                }
            }
        }

        return mapa;
    }

}
