using Backend.Models;

namespace Backend.TDAs
{
    public class ListaDrones
    {
        private Nodo cabeza;

        public void Insertar(Dron dron)
        {
            Nodo nuevo = new Nodo(dron);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }
    }
}