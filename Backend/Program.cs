using Backend.Services;

var rutas = args.Length > 0 ? args : new[] { "entrada.xml" };
var lector = new XMLReader();
lector.CargarXMLs(rutas);
var decoder = new DecoderService();
var simulator = new SimulatorService();
var optimizer = new OptimizerService();
var writer = new XMLWriter();

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

        int tiempo = simulator.SimularTiempo(mensaje);
        Console.WriteLine($"Tiempo simulado: {tiempo}");

        int tiempoOptimo = optimizer.SimularTiempoReal(mensaje);
        Console.WriteLine($"Tiempo óptimo: {tiempoOptimo}");

        var timeline = optimizer.SimularConTimeline(mensaje);
        writer.GenerarXML($"salida_{mensaje.Nombre}.xml", mensaje, sistema, tiempoOptimo, texto, timeline);
        Console.WriteLine($"XML generado: salida_{mensaje.Nombre}.xml");
    }
    else
    {
        Console.WriteLine($"Mensaje {mensaje.Nombre}: (sistema no encontrado)");
    }
}
