using System;
using System.IO;
using System.Net.Http;
using NLog.Fluent;

namespace Bookmaker.Logic;

public static class GetWebsiteIcon
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    [Obsolete("Obsolete")]
    public static void GetIconOld(string name, string url)
    {
        
        var client = new System.Net.WebClient();

        client.DownloadFile(@"http://www.google.com/s2/favicons?domain_url=" + url,
                            name + ".ico");
    }

    public static async void GetIcon(string name, string url)
    {
        HttpClient client = new();
        try
        {
            var uri = new Uri(@"http://www.google.com/s2/favicons?domain_url=" + url);
            var response = await client.GetAsync(uri);
            await using var stream = await response.Content.ReadAsStreamAsync();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bookmaker", "Icons", name + ".ico");
            await using var fileStream = File.Create(path);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while downloading icon");
        }
        finally
        {
            client.Dispose();
        }

    }
}