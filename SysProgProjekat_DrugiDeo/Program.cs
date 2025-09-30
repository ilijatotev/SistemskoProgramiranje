using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        var rootDir = Path.Combine(projectDir, "web_files");

        var logger = new Logger();

        logger.Log($"Root direktorijum za fajlove: {rootDir}");

        if (!Directory.Exists(rootDir))
        {
            try
            {
                Directory.CreateDirectory(rootDir);
                logger.Log("Root direktorijum je kreiran. Ubacite .csv fajlove u njega.");
            }
            catch (Exception ex)
            {
                logger.Log($"[GRESKA] Neuspesno kreiranje root direktorijuma: {ex.Message}");
                return;
            }
        }

        string serverUrl = "http://localhost:5050/";

        var cacheManager = new CacheManager();
        var requestHandler = new RequestHandler(cacheManager, logger, rootDir);
        var simpleWebServer = new SimpleWebServer(serverUrl, requestHandler, logger);

        try
        {
            await simpleWebServer.Start();
        }
        catch (Exception ex)
        {
            logger.Log($"[GRESKA] Neuspesno pokretanje servera: {ex.Message}");
        }
    }
}