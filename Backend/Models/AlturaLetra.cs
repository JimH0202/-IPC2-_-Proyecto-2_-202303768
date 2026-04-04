namespace Backend.Models
{
    public class AlturaLetra
    {
        public string Dron;
        public int Altura;
        public string Letra;

        public AlturaLetra(string dron, int altura, string letra)
        {
            Dron = dron;
            Altura = altura;
            Letra = letra;
        }
    }
}