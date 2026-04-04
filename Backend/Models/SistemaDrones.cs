using Backend.TDAs;

namespace Backend.Models
{
    public class SistemaDrones
    {
        public string Nombre;
        public int AlturaMaxima;
        public int CantidadDrones;

        public ListaDrones Drones;
        public ListaAlturas Tabla; // dron + altura → letra

        public SistemaDrones(string nombre)
        {
            Nombre = nombre;
            Drones = new ListaDrones();
            Tabla = new ListaAlturas();
        }
    }
}