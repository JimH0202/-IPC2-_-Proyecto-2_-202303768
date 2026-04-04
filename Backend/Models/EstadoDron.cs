namespace Backend.Models
{
    public class EstadoDron
    {
        public string Nombre;
        public int AlturaActual;
        public bool ListoParaEmitir;

        public EstadoDron(string nombre)
        {
            Nombre = nombre;
            AlturaActual = 0;
            ListoParaEmitir = false;
        }
    }
}