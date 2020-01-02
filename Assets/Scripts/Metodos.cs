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

        for(int x = 0; x <= mapa.GetUpperBound(0); x++)
        {
            for(int y = 0; y <= mapa.GetUpperBound(1); y++)
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
        for(int x = 0; x <= mapa.GetUpperBound(0); x++)
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
                for(int x = posicionAnterior.x; x < posicionActual.x && x <= mapa.GetUpperBound(0); x++)
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
        for(int x = 0; x <= mapa.GetUpperBound(0); x++)
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
        for(int x = 0; x <= mapa.GetUpperBound(0); x++)
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

        for(int x = 0; x <= mapa.GetUpperBound(0); x++)
        {
            for(int y = 0; y <= mapa.GetUpperBound(1); y++)
            {
                if(losBordesSonMuros && (x == 0 || y == 0 || x == mapa.GetUpperBound(0) || y == mapa.GetUpperBound(1)))
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

    public static int [,] RandomWalkCueva(int[,] mapa, float semilla, float porcentajeSueloEliminar, bool losBordesSonMuros = true, bool movimientoEnDiagonal = false)
    {
        //La semilla de nuestro random
        Random.InitState(semilla.GetHashCode());

        int valorMinimo = 0;
        int valorMaximoX = mapa.GetUpperBound(0);
        int valorMaximoY = mapa.GetUpperBound(1);
        int ancho = mapa.GetUpperBound(0) + 1;
        int alto = mapa.GetUpperBound(1) + 1;

        if(losBordesSonMuros)
        {
            valorMinimo++;
            valorMaximoX--;
            valorMaximoY--;
            ancho -= 2;
            alto -= 2;
        }

        //Definir la posicion de inicio en X y Y
        int posicionX = Random.Range(valorMinimo, valorMaximoX);
        int posicionY = Random.Range(valorMinimo, valorMaximoY);

        //Cantidad de losetas a eliminar
        int cantidadDeLosrtasAEliminar = Mathf.FloorToInt(ancho * alto * porcentajeSueloEliminar);

        //Cantidad de losetas eliminadas
        int losetasEliminadas = 0;

        while(losetasEliminadas < cantidadDeLosrtasAEliminar)
        {
            if(mapa[posicionX, posicionY] == 1)
            {
                mapa[posicionX, posicionY] = 0;
                losetasEliminadas++;
            }

            if(movimientoEnDiagonal)
            {
                int direccionAleatoriaX = Random.Range(-1, 2);
                int direccionAleatoriaY = Random.Range(-1, 2);
                posicionX += direccionAleatoriaX;
                posicionY += direccionAleatoriaY;
            }
            else
            {
                int direccionAleatoria = Random.Range(0, 4);
                switch(direccionAleatoria)
                {
                    case 0:
                        posicionY++;
                        break;
                    case 1:
                        posicionY--;
                        break;
                    case 2:
                        posicionX--;
                        break;
                    case 3:
                        posicionX++;
                        break;
                }
            }
            
            posicionX = Mathf.Clamp(posicionX, valorMinimo, valorMaximoX);
            posicionY = Mathf.Clamp(posicionY, valorMinimo, valorMaximoY);

        }

        return mapa;

    }

    public static int[,] TunelDireccional(int[,] mapa, float semilla, int anchoMinimo, int anchoMaximo, float aspereza, int desplazamientoMaximo, float desplazamiento)
    {
        int anchoTunel = 1;

        int x = mapa.GetUpperBound(0) / 2;

        Random.InitState(semilla.GetHashCode());

        for(int y = 0; y <= mapa.GetUpperBound(1); y++)
        {
            for(int i = -anchoTunel; i <= anchoTunel; i++)
            {
                mapa[x + i, y] = 0; 
            }
            if(Random.value < aspereza)
            {
                int cambioAncho = Random.Range(-anchoMaximo, anchoMaximo);
                anchoTunel += cambioAncho;

                anchoTunel = Mathf.Clamp(anchoTunel, anchoMinimo, anchoMaximo);
            }

            if(Random.value < desplazamiento)
            {
                int cambioEnX = Random.Range(-desplazamientoMaximo, desplazamientoMaximo);
                x += cambioEnX;
                x = Mathf.Clamp(x, anchoMaximo + 1, mapa.GetUpperBound(0) - anchoMaximo);
            }
        }

        return mapa;
    }

    public static int[,] GenerarMapaAleatorio(int ancho, int alto, float semilla, float porcentajeDeRelleno, bool losBordesSonMuros)
    {
        Random.InitState(semilla.GetHashCode());

        int[,] mapa = new int[ancho, alto];

        for(int x = 0; x <= mapa.GetUpperBound(0); x++)
        {
            for(int y = 0; y <= mapa.GetUpperBound(1); y++)
            {
                if(losBordesSonMuros && (x == 0 || x == mapa.GetUpperBound(0) || y == 0 || y == mapa.GetUpperBound(1)))
                {
                    mapa[x, y] = 1;
                }
                else
                {
                    mapa[x, y] = (Random.value < porcentajeDeRelleno) ? 1 : 0;
                }
            }
        }
        return mapa;
    }

    public static int CantidadLosetasVecinas(int[,] mapa, int x, int y, bool incluirDiagonales)
    {
        int totalLosetas = 0;
        for(int vecinoX = x - 1; vecinoX <= x + 1; vecinoX++)
        {
            for(int vecinoY = y - 1; vecinoY <= y + 1; vecinoY++)
            {
                if(vecinoX >= 0 && vecinoX <= mapa.GetUpperBound(0) && vecinoY >= 0 && vecinoY <= mapa.GetUpperBound(1))
                {
                    if(!(vecinoX == x && vecinoY == y) && (incluirDiagonales || (vecinoX == x || vecinoY == y)))
                    {
                        totalLosetas += mapa[vecinoX, vecinoY];
                    }
                }
            }
        }

        return totalLosetas;
    }

    public static int[,] AutomataCelularMoore(int[,] mapa, int totalDePasadas, bool losBordesSonMuros)
    {
        for(int i = 0; i < totalDePasadas; i++)
        {
            for(int x = 0; x <= mapa.GetUpperBound(0); x++)
            {
                for(int y = 0; y <= mapa.GetUpperBound(1); y++)
                {
                    int losetasVecinas = CantidadLosetasVecinas(mapa, x, y, true);

                    if(losBordesSonMuros && (x == 0 || x == mapa.GetUpperBound(0) || y == 0 || y == mapa.GetUpperBound(1)))
                    {
                        mapa[x, y] = 1;
                    }
                    else if(losetasVecinas > 4)
                    {
                        mapa[x, y] = 1;
                    }
                    else if(losetasVecinas < 4)
                    {
                        mapa[x, y] = 0;
                    }
                }
            }
        }
        return mapa;
    }

    public static int[,] AutomataCelularVonNeumann(int[,] mapa, int totalDePasadas, bool losBordesSonMuros)
    {
        for(int i = 0; i < totalDePasadas; i++)
        {
            for(int x = 0; x <= mapa.GetUpperBound(0); x++)
            {
                for(int y = 0; y <= mapa.GetUpperBound(1); y++)
                {
                    int losetasVecinas = CantidadLosetasVecinas(mapa, x, y, false);

                    if(losBordesSonMuros && (x == 0 || x == mapa.GetUpperBound(0) || y == 0 || y == mapa.GetUpperBound(1)))
                    {
                        mapa[x, y] = 1;
                    }
                    else if(losetasVecinas > 2)
                    {
                        mapa[x, y] = 1;
                    }
                    else if(losetasVecinas < 2)
                    {
                        mapa[x, y] = 0;
                    }
                }
            }
        }
        return mapa;
    }
}
