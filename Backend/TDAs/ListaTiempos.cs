global using Backend.Models;

namespace Backend.TDAs
{
    public class ListaTiempos
    {
        private Nodo? cabeza;

        public void Insertar(int tiempo, TiempoAccion dato)
        {
            Nodo nuevo = new Nodo(dato);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public Nodo? GetCabeza()
        {
            return cabeza;
        }
    }
}