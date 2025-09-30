using System;
using System.IO;
using System.Net;
using System.Threading;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class RequestHandler
{
    private readonly CacheManager cacheManager;
    private readonly Logger logger;
    private readonly string rootDir;

    public RequestHandler(CacheManager cacheManager, Logger logger, string rootDir)
    {
        this.cacheManager = cacheManager;
        this.logger = logger;
        this.rootDir = rootDir;
    }

    public void HandleRequest(object state)
    {
        HttpListenerContext context = (HttpListenerContext)state;
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        int threadId = Thread.CurrentThread.ManagedThreadId;
        string requestedFileName = Path.GetFileName(request.Url.LocalPath);

        logger.Log($"[LOG][Nit {threadId}] Primljen zahtev za: {requestedFileName} | {DateTime.Now}");

        byte[] excelContent;

        if (cacheManager.TryGetValue(requestedFileName, out excelContent))
        {
            logger.Log($"[INFO][Nit {threadId}] Fajl {requestedFileName} pronađen u kešu. Vraćam keširanu verziju.");
        }
        else
        {
            try
            {
                string csvFilePath = Path.Combine(rootDir, requestedFileName);

                if (!File.Exists(csvFilePath))
                {
                    string errorMessage = $"Fajl '{requestedFileName}' nije pronađen.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(errorMessage);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    logger.Log($"[GREŠKA][Nit {threadId}] Fajl {requestedFileName} nije pronađen.");
                    response.OutputStream.Close();
                    return;
                }

                excelContent = ConvertCsvToExcel(csvFilePath);
                cacheManager.Add(requestedFileName, excelContent);
                logger.Log($"[INFO][Nit {threadId}] Fajl {requestedFileName} uspešno konvertovan i dodat u keš.");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Došlo je do greške na serveru: {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(errorMessage);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                logger.Log($"[GREŠKA][Nit {threadId}] Greška pri obradi zahteva za {requestedFileName}: {ex.Message}");
                response.OutputStream.Close();
                return;
            }
        }

        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        response.AddHeader("Content-Disposition", $"attachment; filename={Path.GetFileNameWithoutExtension(requestedFileName)}.xlsx");
        response.ContentLength64 = excelContent.Length;
        response.OutputStream.Write(excelContent, 0, excelContent.Length);
        response.OutputStream.Close();

        logger.Log($"[LOG][Nit {threadId}] Zahtev za {requestedFileName} uspešno obrađen.");
    }

    private byte[] ConvertCsvToExcel(string csvFilePath)
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("Podaci");

        using (var reader = new StreamReader(csvFilePath))
        {
            string line;
            int rowCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                IRow row = sheet.CreateRow(rowCount++);
                string[] values = line.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(values[i]);
                }
            }
        }

        using (var stream = new MemoryStream())
        {
            workbook.Write(stream);
            return stream.ToArray();
        }
    }
}