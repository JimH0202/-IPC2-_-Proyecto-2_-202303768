using Backend.TDAs;

namespace Backend.TDAs
{
    public class TiempoAccion
    {
        public int Tiempo;
        public ListaAcciones Acciones;

        public TiempoAccion(int tiempo)
        {
            Tiempo = tiempo;
            Acciones = new ListaAcciones();
        }
    }
}