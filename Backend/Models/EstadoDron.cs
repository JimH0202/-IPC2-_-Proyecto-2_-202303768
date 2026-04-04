namespace Backend.Models
{
    public class EstadoDron
    {
        public string Nombre;
        public int AlturaActual;

        public EstadoDron(string nombre)
        {
            Nombre = nombre;
            AlturaActual = 0;
        }
    }
}
