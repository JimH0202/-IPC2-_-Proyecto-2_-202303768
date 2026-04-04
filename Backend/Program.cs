using Backend.Services;

var rutas = args.Length > 0 ? args : new[] { "entrada.xml" };
var lector = new XMLReader();
lector.CargarXMLs(rutas);
var decoder = new DecoderService();

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
    var sistema = lector.listaSistemas.ObtenerPorNombre(mensaje.SistemaDrones);
    if (sistema != null)
    {
        var texto = decoder.DecodificarMensaje(mensaje, sistema);
        Console.WriteLine($"Mensaje {mensaje.Nombre}: {texto}");
    }
    else
    {
        Console.WriteLine($"Mensaje {mensaje.Nombre}: (sistema no encontrado)");
    }
}
