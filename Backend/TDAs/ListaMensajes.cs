using Backend.Models;
using System.Collections.Generic;

namespace Backend.TDAs
{
    public class ListaMensajes
    {
        private Nodo? cabeza;

        public void Insertar(Mensaje mensaje)
        {
            Nodo nuevo = new Nodo(mensaje);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public IEnumerable<Mensaje> ObtenerTodos()
        {
            Nodo? actual = cabeza;
            while (actual != null)
            {
                yield return (Mensaje)actual.Dato;
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