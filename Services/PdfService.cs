using BibliotecaProyectoIntegrado.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Diagnostics;
using iText.Kernel.Font;
using iText.IO.Font.Constants;


namespace BibliotecaProyectoIntegrado.Services
{
    public static class PdfService
    {
        public static async Task ExportarInventarioAsync(List<InventarioExtendido> inventario)
        {
            var fileName = $"Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            doc.Add(new Paragraph("Inventario de la Biblioteca")
                .SetFontSize(20)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetMarginBottom(20));

            // Crear tabla
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 2, 2, 2 }))
                .UseAllAvailableWidth();

            // Cabeceras
            table.AddHeaderCell("Título");
            table.AddHeaderCell("Autor");
            table.AddHeaderCell("ISBN");
            table.AddHeaderCell("Estado");

            // Datos
            foreach (var item in inventario)
            {
                table.AddCell(item.Libro.Titulo);
                table.AddCell(item.Libro.Autor);
                table.AddCell(item.Libro.ISBN);
                table.AddCell(item.Status);
            }

            doc.Add(table);
            doc.Close();

            await Application.Current.MainPage.DisplayAlert("PDF generado", $"Archivo guardado en:\n{filePath}", "OK");

            try
            {
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"No se pudo abrir el PDF: {ex.Message}");
            }

        }
    }
}
