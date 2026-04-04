using Backend.Models;
using System.Collections.Generic;

namespace Backend.TDAs
{
    public class ListaSistemas
    {
        private Nodo? cabeza;

        public void Insertar(SistemaDrones sistema)
        {
            Nodo nuevo = new Nodo(sistema);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public IEnumerable<SistemaDrones> ObtenerTodos()
        {
            Nodo? actual = cabeza;
            while (actual != null)
            {
                yield return (SistemaDrones)actual.Dato;
                actual = actual.Siguiente;
            }
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