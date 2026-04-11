using Backend.Models;
using Backend.TDAs;

namespace Backend.Services
{
    public class OptimizerService
    {
        public int SimularTiempoReal(Mensaje mensaje)
        {
            ListaEstados estados = new ListaEstados();
            ListaInstruccionesActivas activas = new ListaInstruccionesActivas();

            // cargar instrucciones
            Nodo? actual = mensaje.Instrucciones.GetCabeza();
            while (actual != null)
            {
                Instruccion inst = (Instruccion)actual.Dato!;
                activas.Insertar(new InstruccionActiva(inst.NombreDron, inst.Altura));
                actual = actual.Siguiente;
            }

            int tiempo = 0;

            while (!activas.TodasCompletadas() && tiempo < 1000) // límite para evitar loop infinito
            {
                Nodo? act = activas.GetCabeza();

                // 1. MOVER TODOS LOS DRONES
                while (act != null)
                {
                    InstruccionActiva inst = (InstruccionActiva)act.Dato!;
                    EstadoDron estado = estados.ObtenerEstado(inst.Dron);

                    if (!inst.Completada)
                    {
                        if (estado.AlturaActual < inst.Altura)
                            estado.AlturaActual++;
                        else if (estado.AlturaActual > inst.Altura)
                            estado.AlturaActual--;

                        // verificar si llegó
                        if (estado.AlturaActual == inst.Altura)
                            estado.ListoParaEmitir = true;
                    }

                    act = act.Siguiente;
                }

                // 2. EMITIR (solo uno)
                act = activas.GetCabeza();
                while (act != null)
                {
                    InstruccionActiva inst = (InstruccionActiva)act.Dato!;
                    EstadoDron estado = estados.ObtenerEstado(inst.Dron);

                    if (!inst.Completada && estado.ListoParaEmitir)
                    {
                        inst.Completada = true;
                        estado.ListoParaEmitir = false;
                        break; // SOLO UNO EMITE
                    }

                    act = act.Siguiente;
                }

                tiempo++;
            }

            return tiempo;
        }

        public ListaTiempos SimularConTimeline(Mensaje mensaje)
        {
            ListaEstados estados = new ListaEstados();
            ListaInstruccionesActivas activas = new ListaInstruccionesActivas();
            ListaTiempos timeline = new ListaTiempos();

            Nodo? actual = mensaje.Instrucciones.GetCabeza();

            while (actual != null)
            {
                Instruccion inst = (Instruccion)actual.Dato!;
                activas.Insertar(new InstruccionActiva(inst.NombreDron, inst.Altura));
                actual = actual.Siguiente;
            }

            int tiempo = 1;

            while (!activas.TodasCompletadas())
            {
                TiempoAccion t = new TiempoAccion(tiempo);

                Nodo? act = activas.GetCabeza();

                // MOVER
                while (act != null)
                {
                    InstruccionActiva inst = (InstruccionActiva)act.Dato!;
                    EstadoDron estado = estados.ObtenerEstado(inst.Dron);

                    if (!inst.Completada)
                    {
                        if (estado.AlturaActual < inst.Altura)
                        {
                            estado.AlturaActual++;
                            t.Acciones.Insertar(new Accion(inst.Dron, "Subir"));
                        }
                        else if (estado.AlturaActual > inst.Altura)
                        {
                            estado.AlturaActual--;
                            t.Acciones.Insertar(new Accion(inst.Dron, "Bajar"));
                        }
                        else
                        {
                            estado.ListoParaEmitir = true;
                        }
                    }

                    act = act.Siguiente;
                }

                // EMITIR
                act = activas.GetCabeza();
                while (act != null)
                {
                    InstruccionActiva inst = (InstruccionActiva)act.Dato!;
                    EstadoDron estado = estados.ObtenerEstado(inst.Dron);

                    if (!inst.Completada && estado.ListoParaEmitir)
                    {
                        inst.Completada = true;
                        estado.ListoParaEmitir = false;

                        t.Acciones.Insertar(new Accion(inst.Dron, "Emitir"));
                        break;
                    }

                    act = act.Siguiente;
                }

                timeline.Insertar(tiempo, t);
                tiempo++;
            }

            return timeline;
        }
    }
}