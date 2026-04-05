using System.Text;
using Backend.Models;

namespace Backend.Services
{
    public class DecoderService
    {
        public string DecodificarMensaje(Mensaje mensaje, SistemaDrones sistema)
        {
            var resultado = new StringBuilder();

            foreach (var instruccion in mensaje.Instrucciones.ObtenerTodos())
            {
                string letra = sistema.Tabla.BuscarLetra(instruccion.NombreDron, instruccion.Altura) ?? "?";
                resultado.Append(letra);
            }

            return resultado.ToString();
        }

    }
}