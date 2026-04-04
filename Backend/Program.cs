using Backend.Services;

var rutas = args.Length > 0 ? args : new[] { "entrada.xml" };
var lector = new XMLReader();
lector.CargarXMLs(rutas);

Console.WriteLine($"Drones leídos: {lector.listaDrones.Contar()}");
Console.WriteLine($"Sistemas leídos: {lector.listaSistemas.Contar()}");
Console.WriteLine($"Mensajes leídos: {lector.listaMensajes.Contar()}");

foreach (var sistema in lector.listaSistemas.ObtenerTodos())
{
    Console.WriteLine($"Sistema: {sistema.Nombre} (drones={sistema.CantidadDrones}, alturaMax={sistema.AlturaMaxima})");
    foreach (var alturaLetra in sistema.Tabla.ObtenerTodos())
    {
        Console.WriteLine($"  {alturaLetra.Dron}@{alturaLetra.Altura} => {alturaLetra.Letra}");
    }
}

foreach (var mensaje in lector.listaMensajes.ObtenerTodos())
{
    var sistema = lector.listaSistemas.ObtenerTodos().FirstOrDefault(s => s.Nombre == mensaje.SistemaDrones);
    var texto = sistema != null ? mensaje.Decodificar(sistema) : "(sistema no encontrado)";
    Console.WriteLine($"Mensaje {mensaje.Nombre}: {texto}");
}
