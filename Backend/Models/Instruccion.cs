namespace Backend.Models
{
    public class Instruccion
    {
        public string NombreDron;
        public int Altura;

        public Instruccion(string nombreDron, int altura)
        {
            NombreDron = nombreDron;
            Altura = altura;
        }
    }
}