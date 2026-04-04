using Backend.Models;
using Backend.TDAs;

namespace Backend.Services
{
    public class SimulatorService
    {
        public int SimularTiempo(Mensaje mensaje)
        {
            int tiempoTotal = 0;

            ListaEstados estados = new ListaEstados();

            foreach (var inst in mensaje.Instrucciones.ObtenerTodos())
            {
                EstadoDron estado = estados.ObtenerEstado(inst.NombreDron);

                int alturaActual = estado.AlturaActual;
                int alturaDestino = inst.Altura;

                int movimiento = Math.Abs(alturaDestino - alturaActual);

                tiempoTotal += movimiento; // subir/bajar
                tiempoTotal += 1; // emitir luz

                // actualizar posición
                estado.AlturaActual = alturaDestino;
            }

            return tiempoTotal;
        }
    }
}