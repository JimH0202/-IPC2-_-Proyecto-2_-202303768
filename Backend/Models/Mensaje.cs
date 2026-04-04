using Backend.TDAs;
using System.Text;

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

        public string Decodificar(SistemaDrones sistema)
        {
            var sb = new StringBuilder();
            foreach (var instruccion in Instrucciones.ObtenerTodos())
            {
                string? letra = sistema.ObtenerLetra(instruccion.NombreDron, instruccion.Altura);
                sb.Append(letra ?? "?");
            }
            return sb.ToString();
        }
    }
}