using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.Services
{
    public class BugReporter
    {
        private static BugReporter _instance;
        private readonly string _logFilePath;

        public static BugReporter Instance => _instance ??= new BugReporter();

        private BugReporter()
        {
            // Guardar en la carpeta del ejecutable
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            _logFilePath = Path.Combine(appPath, "BugReports.txt");
        }

        /// <summary>
        /// Reporta un bug manualmente con todos los detalles
        /// </summary>
        public void ReportBug(string title, string description, string steps = "", string additionalInfo = "")
        {
            try
            {
                var report = new StringBuilder();
                report.AppendLine(new string('=', 70));
                report.AppendLine("REPORTE DE BUG");
                report.AppendLine(new string('=', 70));
                report.AppendLine($"Fecha y Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                report.AppendLine($"Título: {title}");
                report.AppendLine();
                report.AppendLine("DESCRIPCIÓN:");
                report.AppendLine(description);

                if (!string.IsNullOrWhiteSpace(steps))
                {
                    report.AppendLine();
                    report.AppendLine("PASOS PARA REPRODUCIR:");
                    report.AppendLine(steps);
                }

                if (!string.IsNullOrWhiteSpace(additionalInfo))
                {
                    report.AppendLine();
                    report.AppendLine("INFORMACIÓN ADICIONAL:");
                    report.AppendLine(additionalInfo);
                }

                report.AppendLine(new string('=', 70));
                report.AppendLine();

                File.AppendAllText(_logFilePath, report.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al guardar reporte: {ex.Message}");
            }
        }

        /// <summary>
        /// Reporta una excepción automáticamente
        /// </summary>
        public void ReportException(Exception exception, string context = null)
        {
            try
            {
                var report = new StringBuilder();
                report.AppendLine(new string('=', 70));
                report.AppendLine("EXCEPCIÓN CAPTURADA");
                report.AppendLine(new string('=', 70));
                report.AppendLine($"Fecha y Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                report.AppendLine($"Tipo de Excepción: {exception.GetType().Name}");
                report.AppendLine($"Mensaje: {exception.Message}");

                if (!string.IsNullOrEmpty(context))
                {
                    report.AppendLine($"Contexto: {context}");
                }

                report.AppendLine();
                report.AppendLine("STACK TRACE:");
                report.AppendLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    report.AppendLine();
                    report.AppendLine("INNER EXCEPTION:");
                    report.AppendLine($"Tipo: {exception.InnerException.GetType().Name}");
                    report.AppendLine($"Mensaje: {exception.InnerException.Message}");
                    report.AppendLine($"Stack Trace: {exception.InnerException.StackTrace}");
                }

                report.AppendLine(new string('=', 70));
                report.AppendLine();

                File.AppendAllText(_logFilePath, report.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al guardar excepción: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la ruta del archivo de logs
        /// </summary>
        public string GetLogFilePath() => _logFilePath;

        /// <summary>
        /// Abre el archivo de logs con el editor de texto predeterminado
        /// </summary>
        public void OpenLogFile()
        {
            if (File.Exists(_logFilePath))
            {
                Process.Start(new ProcessStartInfo(_logFilePath) { UseShellExecute = true });
            }
        }

        /// <summary>
        /// Limpia todos los logs
        /// </summary>
        public void ClearLogs()
        {
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }

        /// <summary>
        /// Verifica si existen reportes
        /// </summary>
        public bool HasReports() => File.Exists(_logFilePath) && new FileInfo(_logFilePath).Length > 0;
    }
}