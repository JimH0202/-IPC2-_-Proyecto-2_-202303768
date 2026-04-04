using Backend.Models;

namespace Backend.TDAs
{
    public class ListaMensajes
    {
        private Nodo cabeza;

        public void Insertar(Mensaje mensaje)
        {
            Nodo nuevo = new Nodo(mensaje);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }
    }
}