using Backend.Models;
using System.Collections.Generic;

namespace Backend.TDAs
{
    public class ListaInstrucciones
    {
        private Nodo? cabeza;

        public void Insertar(Instruccion instruccion)
        {
            Nodo nuevo = new Nodo(instruccion);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public IEnumerable<Instruccion> ObtenerTodos()
        {
            Nodo? actual = cabeza;
            while (actual != null)
            {
                yield return (Instruccion)actual.Dato;
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

        public Nodo? GetCabeza() => cabeza;
    }
}