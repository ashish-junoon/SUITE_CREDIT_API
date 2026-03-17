using iText.Html2pdf;
using LoggerLibrary;
using Microsoft.Playwright;
using System.Reflection;

namespace CIC.Helper
{
    public class HtmlToPdfService
    {
        public static string ConvertHtmlToPdfAndGetUrl(string html, string webRootPath, ILoggerManager _logger)
        {
            string pdfUrl = string.Empty;
            try
            {
                // 1️⃣ File name
                var fileName = $"report_{Guid.NewGuid()}.pdf";

                // 2️⃣ Folder path
                // var folderPath = System.IO.Path.Combine(webRootPath, "pdfs");
                if (!Directory.Exists(webRootPath))
                    Directory.CreateDirectory(webRootPath);

                // 3️⃣ File path
                var filePath = System.IO.Path.Combine(webRootPath, fileName);

                // 4️⃣ Convert HTML to PDF safely
                var props = new ConverterProperties();
                using (var pdfStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    HtmlConverter.ConvertToPdf(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)), pdfStream, props);
                }

                // 5️⃣ Return public URL
                pdfUrl = $"{webRootPath}/{fileName}";
                _logger.LogInfo($"MethodName: {MethodBase.GetCurrentMethod().Name}, Result : {pdfUrl}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"MethodName: {MethodBase.GetCurrentMethod().Name}, Error Message : {ex.Message}");
            }

            return pdfUrl;
        }

        public static async Task<string> GeneratePdfAndGetUrlAsync(string html, string webRootPath, ILoggerManager _logger)
        {
            string pdfUrl = string.Empty;
            try
            {
                _logger.LogInfo($"MethodName: {MethodBase.GetCurrentMethod().Name}, Starting PDF generation. Environment : {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", "0");
                }
                else
                {
                    var path = @"C:\playwright-browsers";
                    if (Directory.Exists(path))
                    {
                        Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", path);
                    }
                }
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(
                    new BrowserTypeLaunchOptions { Headless = true });

                var page = await browser.NewPageAsync();
                await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.NetworkIdle });

                var pdfBytes = await page.PdfAsync(new PagePdfOptions
                {
                    Format = "A4",
                    PrintBackground = true
                });

                // 1️⃣ file name
                var fileName = $"report_{Guid.NewGuid()}.pdf";

                var filePath = System.IO.Path.Combine(webRootPath, fileName);

                // 3️⃣ save pdf
                try
                {
                    await File.WriteAllBytesAsync(filePath, pdfBytes);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"MethodName: {MethodBase.GetCurrentMethod().Name}, Error Writing file on disk : {ex.Message}");
                }


                // 4️⃣ public url
                pdfUrl = $"{webRootPath}/{fileName}";
                _logger.LogInfo($"MethodName: {MethodBase.GetCurrentMethod().Name}, Result : {pdfUrl}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"MethodName: {MethodBase.GetCurrentMethod().Name}, Error Message : {ex.Message}");
            }
            return pdfUrl;
        }

    }
}
