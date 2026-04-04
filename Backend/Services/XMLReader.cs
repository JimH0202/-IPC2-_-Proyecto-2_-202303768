using System.Xml;
using System.IO;
using Backend.Models;
using Backend.TDAs;

namespace Backend.Services
{
    public class XMLReader
    {
        public ListaDrones listaDrones = new ListaDrones();
        public ListaSistemas listaSistemas = new ListaSistemas();
        public ListaMensajes listaMensajes = new ListaMensajes();

        public void CargarXML(string ruta)
        {
            if (!File.Exists(ruta))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(ruta);

            LeerDrones(doc);
            LeerSistemas(doc);
            LeerMensajes(doc);
        }

        public void CargarXMLs(IEnumerable<string> rutas)
        {
            foreach (var ruta in rutas)
            {
                CargarXML(ruta);
            }
        }

        private void LeerDrones(XmlDocument doc)
        {
            XmlNodeList? drones = doc.SelectNodes("//listaDrones/dron");
            if (drones == null)
                return;

            foreach (XmlNode dron in drones)
            {
                string nombre = dron.InnerText.Trim();
                if (string.IsNullOrEmpty(nombre) || listaDrones.Contiene(nombre))
                    continue;

                listaDrones.Insertar(new Dron(nombre));
            }
        }

        private void LeerSistemas(XmlDocument doc)
        {
            XmlNodeList? sistemas = doc.SelectNodes("//listaSistemasDrones/sistemaDrones") ?? doc.SelectNodes("//sistemaDrones");
            if (sistemas == null)
                return;

            foreach (XmlNode sistema in sistemas)
            {
                string nombre = sistema.Attributes?["nombre"]?.Value?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(nombre))
                    continue;

                SistemaDrones? s = listaSistemas.ObtenerPorNombre(nombre) ?? new SistemaDrones(nombre);

                var alturaMaximaNode = sistema.SelectSingleNode("./alturaMaxima");
                var cantidadDronesNode = sistema.SelectSingleNode("./cantidadDrones");

                if (alturaMaximaNode == null || cantidadDronesNode == null)
                    continue;

                if (!int.TryParse(alturaMaximaNode.InnerText.Trim(), out int alturaMaxima))
                    continue;

                if (!int.TryParse(cantidadDronesNode.InnerText.Trim(), out int cantidadDrones))
                    continue;

                s.AlturaMaxima = Math.Max(s.AlturaMaxima, alturaMaxima);
                s.CantidadDrones = Math.Max(s.CantidadDrones, cantidadDrones);

                XmlNodeList? contenidos = sistema.SelectNodes("./contenido");
                if (contenidos == null)
                {
                    if (listaSistemas.ObtenerPorNombre(nombre) == null)
                        listaSistemas.Insertar(s);
                    continue;
                }

                foreach (XmlNode contenido in contenidos)
                {
                    var dronNode = contenido.SelectSingleNode("./dron");
                    if (dronNode == null)
                        continue;

                    string dron = dronNode.InnerText.Trim();
                    if (!string.IsNullOrEmpty(dron) && !s.Drones.Contiene(dron))
                        s.Drones.Insertar(new Dron(dron));

                    XmlNodeList? alturas = contenido.SelectNodes("./alturas/altura") ?? contenido.SelectNodes("./altura");
                    if (alturas == null)
                        continue;

                    foreach (XmlNode altura in alturas)
                    {
                        int valor = 0;
                        var valorAttr = altura.Attributes?["valor"]?.Value;
                        if (string.IsNullOrEmpty(valorAttr) || !int.TryParse(valorAttr.Trim(), out valor))
                            continue;

                        string letra = altura.InnerText.Trim();
                        if (s.Tabla.BuscarLetra(dron, valor) != null)
                            continue;

                        s.Tabla.Insertar(new AlturaLetra(dron, valor, letra));
                    }
                }

                if (listaSistemas.ObtenerPorNombre(nombre) == null)
                    listaSistemas.Insertar(s);
            }
        }

        private void LeerMensajes(XmlDocument doc)
        {
            XmlNodeList? mensajes = doc.SelectNodes("//listaMensajes/Mensaje") ?? doc.SelectNodes("//Mensaje");
            if (mensajes == null)
                return;

            foreach (XmlNode mensaje in mensajes)
            {
                string nombre = mensaje.Attributes?["nombre"]?.Value?.Trim() ?? string.Empty;
                var sistemaNode = mensaje.SelectSingleNode("./sistemaDrones");
                string sistema = sistemaNode?.InnerText.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(sistema) || listaMensajes.Contiene(nombre))
                    continue;

                Mensaje m = new Mensaje(nombre, sistema);

                XmlNodeList? instrucciones = mensaje.SelectNodes("./instrucciones/instruccion") ?? mensaje.SelectNodes(".//instruccion");
                if (instrucciones == null)
                {
                    listaMensajes.Insertar(m);
                    continue;
                }

                foreach (XmlNode inst in instrucciones)
                {
                    string dron = inst.Attributes?["dron"]?.Value?.Trim() ?? string.Empty;
                    if (string.IsNullOrEmpty(dron))
                        continue;

                    if (!int.TryParse(inst.InnerText.Trim(), out int altura))
                        continue;

                    m.Instrucciones.Insertar(new Instruccion(dron, altura));
                }

                listaMensajes.Insertar(m);
            }
        }
    }
}