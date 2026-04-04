using Backend.TDAs;

namespace Backend.Models
{
    public class Mensaje
    {
        public string Nombre;
        public string SistemaDrones;
        public ListaInstrucciones Instrucciones;

        public Mensaje(string nombre, string sistema)
        {
            Nombre = nombre;
            SistemaDrones = sistema;
            Instrucciones = new ListaInstrucciones();
        }
    }
}