using Backend.Models;

namespace Backend.TDAs
{
    public class ListaAlturas
    {
        private Nodo cabeza;

        public void Insertar(AlturaLetra dato)
        {
            Nodo nuevo = new Nodo(dato);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }
    }
}