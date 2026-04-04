using Backend.Models;

namespace Backend.TDAs
{
    public class ListaInstruccionesActivas
    {
        private Nodo? cabeza;

        public void Insertar(InstruccionActiva inst)
        {
            Nodo nuevo = new Nodo(inst);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public Nodo? GetCabeza()
        {
            return cabeza;
        }

        public bool TodasCompletadas()
        {
            Nodo? actual = cabeza;

            while (actual != null)
            {
                InstruccionActiva inst = (InstruccionActiva)actual.Dato!;

                if (!inst.Completada)
                    return false;

                actual = actual.Siguiente;
            }

            return true;
        }
    }
}