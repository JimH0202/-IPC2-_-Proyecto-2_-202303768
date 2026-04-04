namespace Backend.TDAs
{
    public class Nodo
    {
        public object Dato;
        public Nodo? Siguiente;

        public Nodo(object dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}