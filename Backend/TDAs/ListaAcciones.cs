using Backend.Models;
using Backend.TDAs;

namespace Backend.TDAs
{
    public class ListaAcciones
    {
        private Nodo? cabeza;

        public void Insertar(Accion accion)
        {
            Nodo nuevo = new Nodo(accion);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public Nodo? GetCabeza()
        {
            return cabeza;
        }
    }
}