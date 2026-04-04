using Backend.Models;
using System.Collections.Generic;

namespace Backend.TDAs
{
    public class ListaAlturas
    {
        private Nodo? cabeza;

        public void Insertar(AlturaLetra dato)
        {
            Nodo nuevo = new Nodo(dato);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public IEnumerable<AlturaLetra> ObtenerTodos()
        {
            Nodo? actual = cabeza;
            while (actual != null)
            {
                yield return (AlturaLetra)actual.Dato;
                actual = actual.Siguiente;
            }
        }

        public string? BuscarLetra(string dron, int altura)
        {
            Nodo? actual = cabeza;
            while (actual != null)
            {
                AlturaLetra item = (AlturaLetra)actual.Dato;
                if (item.Dron == dron && item.Altura == altura)
                {
                    return item.Letra;
                }
                actual = actual.Siguiente;
            }
            return null;
        }

        public int Contar()
        {
            int count = 0;
            Nodo? actual = cabeza;
            while (actual != null)
            {
                count++;
                actual = actual.Siguiente;
            }
            return count;
        }
    }
}