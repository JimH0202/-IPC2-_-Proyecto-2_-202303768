using Backend.Models;

namespace Backend.TDAs
{
    public class ListaEstados
    {
        private Nodo? cabeza;

        public EstadoDron ObtenerEstado(string nombre)
        {
            Nodo? actual = cabeza;

            while (actual != null)
            {
                EstadoDron estado = (EstadoDron)actual.Dato;

                if (estado.Nombre == nombre)
                    return estado;

                actual = actual.Siguiente;
            }

            EstadoDron nuevo = new EstadoDron(nombre);
            Insertar(nuevo);
            return nuevo;
        }

        private void Insertar(EstadoDron estado)
        {
            Nodo nuevo = new Nodo(estado);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }
    }
}
