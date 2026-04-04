namespace Backend.Models
{
    public class InstruccionActiva
    {
        public string Dron;
        public int Altura;
        public bool Completada;

        public InstruccionActiva(string dron, int altura)
        {
            Dron = dron;
            Altura = altura;
            Completada = false;
        }
    }
}