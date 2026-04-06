using System;
using System.Diagnostics;
using Backend.Services;
using Backend.Models;
using Backend.TDAs;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Linq;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DroneSystemController : ControllerBase
    {
        private static XMLReader? _reader;
        private static DecoderService? _decoder;
        private static OptimizerService? _optimizer;

        [HttpPost("cargar-xml")]
        public IActionResult CargarXML(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file provided");

                // Guardar archivo temporalmente
                var rutaTemporal = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = System.IO.File.Create(rutaTemporal))
                {
                    file.CopyTo(stream);
                }

                // Cargar con la clase XMLReader
                _reader = new XMLReader();
                _reader.CargarXML(rutaTemporal);
                _decoder = new DecoderService();
                _optimizer = new OptimizerService();

                return Ok(new
                {
                    drones = _reader.listaDrones.Contar(),
                    sistemas = _reader.listaSistemas.Contar(),
                    mensajes = _reader.listaMensajes.Contar(),
                    mensaje = "XML cargado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("inicializar")]
        public IActionResult InicializarSistema()
        {
            _reader = new XMLReader();
            _decoder = new DecoderService();
            _optimizer = new OptimizerService();

            return Ok(new
            {
                mensaje = "Sistema inicializado. Carga un archivo XML para continuar.",
                drones = 0,
                sistemas = 0,
                mensajes = 0
            });
        }

        public class DronRequest
        {
            public string Nombre { get; set; } = string.Empty;
        }

        private static string GetProjectRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && dir.FullName != dir.Root.FullName)
            {
                if (System.IO.File.Exists(Path.Combine(dir.FullName, "Backend.csproj")))
                    return dir.FullName;
                dir = dir.Parent;
            }

            dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && dir.FullName != dir.Root.FullName)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "Data")) || Directory.Exists(Path.Combine(dir.FullName, "Graphviz")))
                    return dir.FullName;
                dir = dir.Parent;
            }

            return AppContext.BaseDirectory;
        }

        private static string EnsureProjectSubfolder(string folderName)
        {
            var root = GetProjectRoot();
            var path = Path.Combine(root, folderName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private static string GetSafeFileName(string input)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                input = input.Replace(c, '_');
            return input.Replace(' ', '_');
        }

        private static string SaveDotFile(string name, string dotCode)
        {
            var graphvizDir = EnsureProjectSubfolder("Graphviz");
            var safeName = GetSafeFileName(name);
            var dotPath = Path.Combine(graphvizDir, $"{safeName}.dot");
            System.IO.File.WriteAllText(dotPath, dotCode);
            return dotPath;
        }

        private static string? TryRenderDotToSvg(string dotPath, string svgFileName)
        {
            try
            {
                var dotExe = FindDotExecutable();
                if (string.IsNullOrEmpty(dotExe))
                    return null;

                var svgPath = Path.Combine(EnsureProjectSubfolder("Graphviz"), svgFileName);
                var startInfo = new ProcessStartInfo
                {
                    FileName = dotExe,
                    Arguments = $"-Tsvg -o \"{svgPath}\" \"{dotPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                    return null;

                process.WaitForExit(10000);
                if (process.ExitCode == 0 && System.IO.File.Exists(svgPath))
                    return svgPath;

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string? FindDotExecutable()
        {
            try
            {
                var whereInfo = new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = "dot",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var whereProcess = Process.Start(whereInfo);
                if (whereProcess != null)
                {
                    whereProcess.WaitForExit(5000);
                    if (whereProcess.ExitCode == 0)
                    {
                        var path = whereProcess.StandardOutput.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                            return path;
                    }
                }
            }
            catch
            {
            }

            var candidates = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Graphviz", "bin", "dot.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Graphviz", "bin", "dot.exe")
            };

            return candidates.FirstOrDefault(System.IO.File.Exists);
        }

        [HttpPost("agregar-dron")]
        public IActionResult AgregarDron([FromBody] DronRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest("Nombre de dron requerido");
            if (_reader == null)
                return BadRequest("Primero inicializa el sistema");
            if (_reader.listaDrones.Contiene(request.Nombre))
                return BadRequest("El dron ya existe");

            _reader.listaDrones.Insertar(new Dron(request.Nombre));
            return Ok(new
            {
                nombre = request.Nombre,
                mensaje = "Dron agregado correctamente"
            });
        }

        [HttpGet("drones")]
        public IActionResult GetDrones()
        {
            if (_reader == null)
                return BadRequest("Primero carga un XML");

            var drones = new List<object>();
            var listaDrones = _reader.listaDrones.ObtenerTodos().OrderBy(d => d.Nombre).ToList();

            foreach (var dron in listaDrones)
            {
                string sistemaAsignado = "N/A";
                int alturaMaxima = 100;

                foreach (var sistema in _reader.listaSistemas.ObtenerTodos())
                {
                    bool encontrado = false;
                    foreach (var dronEnSistema in sistema.Drones.ObtenerTodos())
                    {
                        if (dronEnSistema.Nombre == dron.Nombre)
                        {
                            sistemaAsignado = sistema.Nombre;
                            alturaMaxima = sistema.AlturaMaxima;
                            encontrado = true;
                            break;
                        }
                    }
                    if (encontrado) break;
                }

                drones.Add(new
                {
                    nombre = dron.Nombre,
                    alturaActual = dron.AlturaActual,
                    alturaMaxima = alturaMaxima,
                    velocidad = "500 m/s",
                    sistema = sistemaAsignado,
                    estado = "Activo"
                });
            }

            return Ok(drones);
        }

        [HttpGet("sistemas")]
        public IActionResult GetSistemas()
        {
            if (_reader == null)
                return BadRequest("Primero carga un XML");

            var sistemas = new List<object>();
            int index = 1;
            foreach (var sistema in _reader.listaSistemas.ObtenerTodos())
            {
                // Calcular altura mínima de los drones en este sistema
                int alturaMinima = int.MaxValue;
                foreach (var dron in sistema.Drones.ObtenerTodos())
                {
                    if (dron.AlturaActual < alturaMinima)
                        alturaMinima = dron.AlturaActual;
                }
                if (alturaMinima == int.MaxValue) alturaMinima = 0;

                sistemas.Add(new
                {
                    nombre = sistema.Nombre,
                    idSistema = $"SYS-{index:D3}",
                    cantidadDrones = sistema.CantidadDrones,
                    alturaMaxima = sistema.AlturaMaxima,
                    alturaMinima = alturaMinima,
                    frecuencia = "2.4 GHz",
                    estado = "Activo"
                });
                index++;
            }

            return Ok(sistemas);
        }

        [HttpGet("mensajes")]
        public IActionResult GetMensajes()
        {
            if (_reader == null || _decoder == null || _optimizer == null)
                return BadRequest("Primero carga un XML");

            var mensajes = _reader.listaMensajes.ObtenerTodos()
                .OrderBy(m => m.Nombre)
                .Select(mensaje =>
                {
                    var sistema = _reader.listaSistemas.ObtenerPorNombre(mensaje.SistemaDrones);
                    var texto = string.Empty;
                    var tiempoOptimo = 0;
                    if (sistema != null)
                    {
                        texto = _decoder.DecodificarMensaje(mensaje, sistema);
                        tiempoOptimo = _optimizer.SimularTiempoReal(mensaje);
                    }
                    return new
                    {
                        nombre = mensaje.Nombre,
                        sistemaDrones = mensaje.SistemaDrones,
                        textoDecodificado = texto,
                        tiempoOptimo = tiempoOptimo
                    };
                })
                .ToList();

            return Ok(mensajes);
        }

        [HttpGet("mensaje/{nombre}")]
        public IActionResult GetMensaje(string nombre)
        {
            if (_reader == null || _decoder == null || _optimizer == null)
                return BadRequest("Primero carga un XML");

            var mensaje = _reader.listaMensajes.ObtenerTodos()
                .FirstOrDefault(m => m.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

            if (mensaje == null)
                return NotFound("Mensaje no encontrado");

            var sistema = _reader.listaSistemas.ObtenerPorNombre(mensaje.SistemaDrones);
            if (sistema == null)
                return BadRequest("Sistema de drones no encontrado para este mensaje");

            var texto = _decoder.DecodificarMensaje(mensaje, sistema);
            var tiempoOptimo = _optimizer.SimularTiempoReal(mensaje);
            var instrucciones = mensaje.Instrucciones.ObtenerTodos()
                .Select(i => new { dron = i.NombreDron, altura = i.Altura })
                .ToList();

            return Ok(new
            {
                nombre = mensaje.Nombre,
                sistemaDrones = mensaje.SistemaDrones,
                mensajeRecibido = texto,
                tiempoOptimo = tiempoOptimo,
                instrucciones = instrucciones
            });
        }

        [HttpGet("mensaje/{nombre}/graphviz")]
        public IActionResult GetMensajeGraphviz(string nombre)
        {
            if (_reader == null || _decoder == null || _optimizer == null)
                return BadRequest("Primero carga un XML");

            var mensaje = _reader.listaMensajes.ObtenerTodos()
                .FirstOrDefault(m => m.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
            if (mensaje == null)
                return NotFound("Mensaje no encontrado");

            var sistema = _reader.listaSistemas.ObtenerPorNombre(mensaje.SistemaDrones);
            if (sistema == null)
                return BadRequest("Sistema de drones no encontrado para este mensaje");

            var instrucciones = mensaje.Instrucciones.ObtenerTodos().Reverse().ToList();
            var dot = new System.Text.StringBuilder();
            dot.AppendLine("digraph Instrucciones {");
            dot.AppendLine("  rankdir=LR;");
            dot.AppendLine("  node [shape=box, style=rounded, color=darkblue, fontsize=12];");
            dot.AppendLine("  graph [bgcolor=white];");

            for (int i = 0; i < instrucciones.Count; i++)
            {
                var inst = instrucciones[i];
                var nodeId = $"{inst.NombreDron}_{inst.Altura}";
                dot.AppendLine($"  \"{nodeId}\" [label=\"{inst.NombreDron} @ {inst.Altura}m\"];");
                if (i > 0)
                {
                    var prev = instrucciones[i - 1];
                    var prevId = $"{prev.NombreDron}_{prev.Altura}";
                    dot.AppendLine($"  \"{prevId}\" -> \"{nodeId}\";");
                }
            }

            dot.AppendLine("}");

            var dotPath = SaveDotFile($"mensaje_{GetSafeFileName(nombre)}", dot.ToString());
            var svgPath = TryRenderDotToSvg(dotPath, $"mensaje_{GetSafeFileName(nombre)}.svg");
            string? svgContent = null;
            if (!string.IsNullOrEmpty(svgPath) && System.IO.File.Exists(svgPath))
            {
                svgContent = System.IO.File.ReadAllText(svgPath);
            }

            return Ok(new
            {
                dotCode = dot.ToString(),
                dotFile = dotPath,
                svgFile = svgPath,
                svgContent = svgContent
            });
        }

        [HttpGet("graphviz")]
        public IActionResult GetGraphviz()
        {
            if (_reader == null)
                return BadRequest("Primero carga un XML");

            var dotCode = GenerarCodigoDOT();
            var dotPath = SaveDotFile("graphviz_general", dotCode);
            var svgPath = TryRenderDotToSvg(dotPath, "graphviz_general.svg");
            string? svgContent = null;
            if (!string.IsNullOrEmpty(svgPath) && System.IO.File.Exists(svgPath))
            {
                svgContent = System.IO.File.ReadAllText(svgPath);
            }

            return Ok(new
            {
                dotCode = dotCode,
                dotFile = dotPath,
                svgFile = svgPath,
                svgContent = svgContent,
                estadisticas = new
                {
                    totalDrones = _reader.listaDrones.Contar(),
                    totalSistemas = _reader.listaSistemas.Contar(),
                    totalMensajes = _reader.listaMensajes.Contar()
                }
            });
        }

        [HttpGet("exportar-xml")]
        public IActionResult ExportarXML()
        {
            if (_reader == null || _decoder == null || _optimizer == null)
                return BadRequest("Primero carga un XML");

            try
            {
                var dataDir = EnsureProjectSubfolder("Data");
                var rutaSalida = Path.Combine(dataDir, "salida_export.xml");

                var xml = new XmlDocument();
                var root = xml.CreateElement("respuesta");
                xml.AppendChild(root);

                var listaMensajes = xml.CreateElement("listaMensajes");
                root.AppendChild(listaMensajes);

                foreach (var mensaje in _reader.listaMensajes.ObtenerTodos())
                {
                    var sistema = _reader.listaSistemas.ObtenerPorNombre(mensaje.SistemaDrones);
                    if (sistema == null)
                        continue;

                    var texto = _decoder.DecodificarMensaje(mensaje, sistema);
                    var tiempoOptimo = _optimizer.SimularTiempoReal(mensaje);
                    var timeline = _optimizer.SimularConTimeline(mensaje);

                    var mensajeElement = xml.CreateElement("mensaje");
                    mensajeElement.SetAttribute("nombre", mensaje.Nombre);
                    listaMensajes.AppendChild(mensajeElement);

                    var sistemaDrones = xml.CreateElement("sistemaDrones");
                    sistemaDrones.InnerText = sistema.Nombre;
                    mensajeElement.AppendChild(sistemaDrones);

                    var tiempoOptimoElement = xml.CreateElement("tiempoOptimo");
                    tiempoOptimoElement.InnerText = tiempoOptimo.ToString();
                    mensajeElement.AppendChild(tiempoOptimoElement);

                    var mensajeRecibidoElement = xml.CreateElement("mensajeRecibido");
                    mensajeRecibidoElement.InnerText = texto;
                    mensajeElement.AppendChild(mensajeRecibidoElement);

                    var instruccionesElement = xml.CreateElement("instrucciones");
                    mensajeElement.AppendChild(instruccionesElement);

                    var timelineNode = timeline.GetCabeza();
                    while (timelineNode != null)
                    {
                        var tiempo = (TiempoAccion)timelineNode.Dato!;
                        var tiempoElement = xml.CreateElement("tiempo");
                        tiempoElement.SetAttribute("valor", tiempo.Tiempo.ToString());

                        var accionesElement = xml.CreateElement("acciones");
                        var accionNode = tiempo.Acciones.GetCabeza();
                        while (accionNode != null)
                        {
                            var accion = (Accion)accionNode.Dato!;
                            var dronElement = xml.CreateElement("dron");
                            dronElement.SetAttribute("nombre", accion.NombreDron);
                            dronElement.InnerText = accion.Tipo;
                            accionesElement.AppendChild(dronElement);
                            accionNode = accionNode.Siguiente;
                        }

                        tiempoElement.AppendChild(accionesElement);
                        instruccionesElement.AppendChild(tiempoElement);
                        timelineNode = timelineNode.Siguiente;
                    }
                }

                xml.Save(rutaSalida);
                var contenido = System.IO.File.ReadAllText(rutaSalida);

                return Ok(new
                {
                    xml = contenido,
                    ruta = rutaSalida
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        private string GenerarCodigoDOT()
        {
            if (_reader == null)
                return "digraph Drones { }";

            var dot = new System.Text.StringBuilder();
            dot.AppendLine("digraph Drones {");
            dot.AppendLine("  rankdir=LR;");
            dot.AppendLine("  node [shape=box, style=rounded, fontsize=11];");
            dot.AppendLine("  graph [bgcolor=lightblue];");

            int systemIndex = 0;
            foreach (var sistema in _reader.listaSistemas.ObtenerTodos())
            {
                dot.AppendLine($"  subgraph cluster_{systemIndex} {{");
                dot.AppendLine($"    label=\"{sistema.Nombre}\";");
                dot.AppendLine("    style=filled;");
                dot.AppendLine("    color=lightgrey;");
                dot.AppendLine("    node [style=filled, fillcolor=white, color=darkblue];");

                foreach (var dron in sistema.Drones.ObtenerTodos().OrderBy(d => d.Nombre))
                {
                    dot.AppendLine($"    \"{dron.Nombre}\";");
                }

                dot.AppendLine("  }");
                systemIndex++;
            }

            var drones = _reader.listaDrones.ObtenerTodos().OrderBy(d => d.Nombre).ToList();
            for (int i = 0; i < drones.Count - 1; i++)
            {
                dot.AppendLine($"  \"{drones[i].Nombre}\" -> \"{drones[i + 1].Nombre}\" [style=dashed, color=gray];");
            }

            dot.AppendLine("}");
            return dot.ToString();
        }
    }
}
