using Backend.Models;

namespace Backend.TDAs
{
    public class ListaInstrucciones
    {
        private Nodo cabeza;

        public void Insertar(Instruccion instruccion)
        {
            Nodo nuevo = new Nodo(instruccion);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }
    }
}