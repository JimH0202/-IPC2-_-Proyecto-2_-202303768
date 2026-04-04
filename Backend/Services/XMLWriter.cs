using System.Xml;
using Backend.Models;
using Backend.TDAs;

namespace Backend.Services
{
    public class XMLWriter
    {
        public void GenerarXML(string ruta, Mensaje mensaje, SistemaDrones sistema, int tiempoOptimo, string mensajeRecibido, ListaTiempos timeline)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement("respuesta");
            doc.AppendChild(root);

            XmlElement listaMensajes = doc.CreateElement("listaMensajes");
            root.AppendChild(listaMensajes);

            XmlElement mensajeElement = doc.CreateElement("mensaje");
            mensajeElement.SetAttribute("nombre", mensaje.Nombre);
            listaMensajes.AppendChild(mensajeElement);

            // Agregar sistemaDrones
            XmlElement sistemaDrones = doc.CreateElement("sistemaDrones");
            sistemaDrones.InnerText = sistema.Nombre;
            mensajeElement.AppendChild(sistemaDrones);

            // Agregar tiempoOptimo
            XmlElement tiempoOptimoElement = doc.CreateElement("tiempoOptimo");
            tiempoOptimoElement.InnerText = tiempoOptimo.ToString();
            mensajeElement.AppendChild(tiempoOptimoElement);

            // Agregar mensajeRecibido
            XmlElement mensajeRecibidoElement = doc.CreateElement("mensajeRecibido");
            mensajeRecibidoElement.InnerText = mensajeRecibido;
            mensajeElement.AppendChild(mensajeRecibidoElement);

            // instrucciones por tiempo
            XmlElement instrucciones = doc.CreateElement("instrucciones");
            mensajeElement.AppendChild(instrucciones);

            Nodo? tNodo = timeline.GetCabeza();

            while (tNodo != null)
            {
                TiempoAccion t = (TiempoAccion)tNodo.Dato!;

                XmlElement tiempo = doc.CreateElement("tiempo");
                tiempo.SetAttribute("valor", t.Tiempo.ToString());

                XmlElement acciones = doc.CreateElement("acciones");

                Nodo? aNodo = t.Acciones.GetCabeza();

                while (aNodo != null)
                {
                    Accion a = (Accion)aNodo.Dato!;

                    XmlElement dron = doc.CreateElement("dron");
                    dron.SetAttribute("nombre", a.NombreDron);
                    dron.InnerText = a.Tipo;

                    acciones.AppendChild(dron);

                    aNodo = aNodo.Siguiente;
                }

                tiempo.AppendChild(acciones);
                instrucciones.AppendChild(tiempo);

                tNodo = tNodo.Siguiente;
            }

            doc.Save(ruta);
        }
    }
}