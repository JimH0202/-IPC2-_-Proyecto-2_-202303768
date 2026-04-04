using Backend.Models;

namespace Backend.TDAs
{
    public class ListaSistemas
    {
        private Nodo cabeza;

        public void Insertar(SistemaDrones sistema)
        {
            Nodo nuevo = new Nodo(sistema);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }
    }
}