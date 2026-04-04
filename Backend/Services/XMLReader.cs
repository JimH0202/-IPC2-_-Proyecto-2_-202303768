using System.Xml;
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
            XmlDocument doc = new XmlDocument();
            doc.Load(ruta);

            LeerDrones(doc);
            LeerSistemas(doc);
            LeerMensajes(doc);
        }

        private void LeerDrones(XmlDocument doc)
        {
            XmlNodeList drones = doc.SelectNodes("//listaDrones/dron");

            foreach (XmlNode dron in drones)
            {
                string nombre = dron.InnerText.Trim();
                listaDrones.Insertar(new Dron(nombre));
            }
        }

        private void LeerSistemas(XmlDocument doc)
        {
            XmlNodeList sistemas = doc.SelectNodes("//sistemaDrones");

            foreach (XmlNode sistema in sistemas)
            {
                string nombre = sistema.Attributes["nombre"].Value;
                SistemaDrones s = new SistemaDrones(nombre);

                s.AlturaMaxima = int.Parse(sistema["alturaMaxima"].InnerText);
                s.CantidadDrones = int.Parse(sistema["cantidadDrones"].InnerText);

                XmlNodeList contenidos = sistema.SelectNodes(".//contenido");

                foreach (XmlNode contenido in contenidos)
                {
                    string dron = contenido["dron"].InnerText;

                    XmlNodeList alturas = contenido.SelectNodes(".//altura");

                    foreach (XmlNode altura in alturas)
                    {
                        int valor = int.Parse(altura.Attributes["valor"].Value);
                        string letra = altura.InnerText;

                        s.Tabla.Insertar(new AlturaLetra(dron, valor, letra));
                    }
                }

                listaSistemas.Insertar(s);
            }
        }

        private void LeerMensajes(XmlDocument doc)
        {
            XmlNodeList mensajes = doc.SelectNodes("//Mensaje");

            foreach (XmlNode mensaje in mensajes)
            {
                string nombre = mensaje.Attributes["nombre"].Value;
                string sistema = mensaje["sistemaDrones"].InnerText;

                Mensaje m = new Mensaje(nombre, sistema);

                XmlNodeList instrucciones = mensaje.SelectNodes(".//instruccion");

                foreach (XmlNode inst in instrucciones)
                {
                    string dron = inst.Attributes["dron"].Value;
                    int altura = int.Parse(inst.InnerText);

                    m.Instrucciones.Insertar(new Instruccion(dron, altura));
                }

                listaMensajes.Insertar(m);
            }
        }
    }
}