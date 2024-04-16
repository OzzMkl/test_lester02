using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using test_lester02.Models;

namespace test_lester02.Controllers
{
    public class RfidCodiCaptEmbarqueController : Controller
    {
        private readonly ExamenContext _context;

        public RfidCodiCaptEmbarqueController(ExamenContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerarReporte(string startDate, string endDate, string agrupar)
        {
            
            DateTime startDateTime = DateTime.Parse(startDate);
            DateTime endDateTime = string.IsNullOrEmpty(endDate)
                    ? DateTime.Today.AddDays(1).AddMilliseconds(-1)
                    : DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1);
            List<string> headersList = new List<string>();

            switch (agrupar)
            {
                case "agruparAcronimo":
                        var report = ConsultaAgrupada(startDateTime, endDateTime);

                        headersList.Add("Acronimo");
                        headersList.Add("Viaje");
                        headersList.Add("Cantidad");

                        ViewBag.Headers = headersList;
                        ViewBag.Report = report;
                    break;
                case "agruparViaje":
                        report = ConsultaAgrupadaNoViaje(startDateTime, endDateTime);

                        headersList.Add("Viaje");
                        headersList.Add("Cantidad");
                        headersList.Add("Fecha");

                        ViewBag.Headers = headersList;
                        ViewBag.Report = report;
                    break;
                case "sinAgrupar":
                        report = test(startDateTime, endDateTime);

                        headersList.Add("RFID");
                        headersList.Add("Acrónimo");
                        headersList.Add("Hora de lectura");

                        ViewBag.Headers = headersList;
                        ViewBag.Report = report;
                    break;
            }

            ViewBag.StartDateTime = startDateTime;
            ViewBag.EndDateTime = endDateTime;
            ViewBag.Agrupar = agrupar;

            return View();


        }

        public List<Object> test(DateTime startDate, DateTime endDate)
        {
            var report = _context.TblRfidCodiCaptEmbarques
               .Where(r => r.FechaLectura >= startDate && r.FechaLectura <= endDate)
               .Select(r => new { r.Codebar, r.Acronimo, r.FechaLectura })
               .ToList<Object>();
            return report;
        }

        public List<Object> ConsultaAgrupada(DateTime startDate, DateTime endDate)
        {
            var report = _context.TblRfidCodiCaptEmbarques
                        .Where(r => r.FechaLectura >= startDate && r.FechaLectura <= endDate)
                        .GroupBy(r => new { r.Acronimo, r.Viaje})
                        .Select(g => new
                        {
                            Acronimo = g.Key.Acronimo,
                            Viaje = g.Key.Viaje,
                            Cantidad = g.Count()
                        })
                        .ToList<Object>();
            return report;
        }

        public List<Object> ConsultaAgrupadaNoViaje(DateTime startDate, DateTime endDate)
        {
            var report = _context.TblRfidCodiCaptEmbarques
                        .Where(r => r.FechaLectura >= startDate && r.FechaLectura <= endDate)
                        .GroupBy(r => r.Viaje )
                        .Select(g => new
                        {
                            Viaje = g.Key,
                            Fecha = g.Min(r => r.FechaLectura),
                            Cantidad = g.Count()
                        })
                        .ToList<Object>();
            return report;
        }

        [HttpPost]
        public IActionResult GenerarReporteExcel(string startDate, string endDate)
        {
            DateTime startDateTime = DateTime.Parse(startDate);
            DateTime endDateTime = string.IsNullOrEmpty(endDate)
                    ? DateTime.Today.AddDays(1).AddMilliseconds(-1)
                    : DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1);

            var report = test(startDateTime, endDateTime);

            //Crear archivo de excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("report");

            //Agregar encabezados
            worksheet.Cells[1, 1].Value = "RFID";
            worksheet.Cells[1, 2].Value = "Acronimo";
            worksheet.Cells[1, 3].Value = "Hora de lectura";

            //Agregar datos
            int row = 2;
            foreach (var item in report)
            {
                var properties = item.GetType().GetProperties();
                worksheet.Cells[row, 1].Value = properties.First(p => p.Name == "Codebar").GetValue(item);
                worksheet.Cells[row, 2].Value = properties.First(p => p.Name == "Acronimo").GetValue(item);
                worksheet.Cells[row, 3].Value = properties.First(p => p.Name == "FechaLectura").GetValue(item);
                row++;
            }

            //Guardar archivo
            MemoryStream stream = new MemoryStream();
            excelPackage.SaveAs(stream);
            stream.Position = 0;

            //devolver el archivo excel a un archivo descargable
            string fileName = "Report_unitario.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(stream, contentType, fileName);
        }

        [HttpPost]
        public IActionResult GenerarReporteAgrupadoExcel(string startDate, string endDate)
        {
            DateTime startDateTime = DateTime.Parse(startDate);
            DateTime endDateTime = string.IsNullOrEmpty(endDate)
                    ? DateTime.Today.AddDays(1).AddMilliseconds(-1)
                    : DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1);

            var report = ConsultaAgrupada(startDateTime, endDateTime);

            //Crear archivo de excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("report");

            //Agregar encabezados
            worksheet.Cells[1, 1].Value = "Acronimo";
            worksheet.Cells[1, 2].Value = "Acronimo";
            worksheet.Cells[1, 3].Value = "Cantidad";

            //Agregar datos
            int row = 2;
            foreach (var item in report)
            {
                var properties = item.GetType().GetProperties();
                worksheet.Cells[row, 1].Value = properties.First(p => p.Name == "Acronimo").GetValue(item);
                worksheet.Cells[row, 2].Value = properties.First(p => p.Name == "Viaje").GetValue(item);
                worksheet.Cells[row, 3].Value = properties.First(p => p.Name == "Cantidad").GetValue(item);
                row++;
            }

            //Guardar archivo
            MemoryStream stream = new MemoryStream();
            excelPackage.SaveAs(stream);
            stream.Position = 0;

            //devolver el archivo excel a un archivo descargable
            string fileName = "Reporte_agrupado.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(stream, contentType, fileName);
        }

        [HttpPost]
        public IActionResult GenerarReporteAgrupadoNoViajeExcel(string startDate, string endDate)
        {
            DateTime startDateTime = DateTime.Parse(startDate);
            DateTime endDateTime = string.IsNullOrEmpty(endDate)
                    ? DateTime.Today.AddDays(1).AddMilliseconds(-1)
                    : DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1);

            var report = ConsultaAgrupadaNoViaje(startDateTime, endDateTime);

            //Crear archivo de excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("report");

            //Agregar encabezados
            worksheet.Cells[1, 1].Value = "Viaje";
            worksheet.Cells[1, 2].Value = "Cantidad";
            worksheet.Cells[1, 3].Value = "Fecha";

            //Agregar datos
            int row = 2;
            foreach (var item in report)
            {
                var properties = item.GetType().GetProperties();
                worksheet.Cells[row, 1].Value = properties.First(p => p.Name == "Viaje").GetValue(item);
                worksheet.Cells[row, 2].Value = properties.First(p => p.Name == "Cantidad").GetValue(item);
                worksheet.Cells[row, 3].Value = properties.First(p => p.Name == "Fecha").GetValue(item);
                row++;
            }

            //Guardar archivo
            MemoryStream stream = new MemoryStream();
            excelPackage.SaveAs(stream);
            stream.Position = 0;

            //devolver el archivo excel a un archivo descargable
            string fileName = "Reporte_agrupado_noviaje.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(stream, contentType, fileName);
        }

    }
}
