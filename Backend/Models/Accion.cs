namespace Backend.Models
{
    public class Accion
    {
        public string NombreDron;
        public string Tipo;

        public Accion(string dron, string tipo)
        {
            NombreDron = dron;
            Tipo = tipo;
        }
    }
}
