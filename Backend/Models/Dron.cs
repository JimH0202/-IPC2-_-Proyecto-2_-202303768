namespace Backend.Models
{
    public class Dron
    {
        public string Nombre;
        public int AlturaActual;

        public Dron(string nombre)
        {
            Nombre = nombre;
            AlturaActual = 0;
        }
    }
}