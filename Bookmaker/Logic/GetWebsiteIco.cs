namespace Bookmaker.Logic;

public static class GetWebsiteIco
{
    public static void GetIcon(string name, string url)
    {
        
        var client = new System.Net.WebClient();

        client.DownloadFile(@"http://www.google.com/s2/favicons?domain_url=" + url,
                            name + ".ico");
    }
}