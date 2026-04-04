using Backend.Models;
using System.Collections.Generic;

namespace Backend.TDAs
{
    public class ListaDrones
    {
        private Nodo? cabeza;

        public void Insertar(Dron dron)
        {
            Nodo nuevo = new Nodo(dron);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public IEnumerable<Dron> ObtenerTodos()
        {
            Nodo? actual = cabeza;
            while (actual != null)
            {
                yield return (Dron)actual.Dato;
                actual = actual.Siguiente;
            }
        }

        public bool Contiene(string nombre)
        {
            Nodo? actual = cabeza;
            while (actual != null)
            {
                if (((Dron)actual.Dato).Nombre == nombre)
                    return true;
                actual = actual.Siguiente;
            }
            return false;
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