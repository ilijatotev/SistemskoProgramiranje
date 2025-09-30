using System;
using System.IO;
using System.Threading;
//using System.Configuration; zaboravio sam da sam definisao url vec u appconfig

class Program
{
    static void Main(string[] args)
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
                logger.Log("Root direktorijum je kreiran.");
            }
            catch (Exception ex)
            {
                logger.Log($"[GREŠKA] Neuspešno kreiranje root direktorijuma: {ex.Message}");
                return;
            }
        }

        string serverUrl = "http://localhost:5050/"; //u principu ne mora da bude hardkodiran jer sam zaboravio da sam definisao vec u appconfig
        //string serverUrl = System.ConfigurationManager.AppSettings["ServerUrl"];

        var cacheManager = new CacheManager();
        var requestHandler = new RequestHandler(cacheManager, logger, rootDir);
        var simpleWebServer = new SimpleWebServer(serverUrl, requestHandler, logger);

        try
        {
            simpleWebServer.Start();
        }
        catch (Exception ex)
        {
            logger.Log($"[GREŠKA] Neuspešno pokretanje servera: {ex.Message}");
        }
    }
}