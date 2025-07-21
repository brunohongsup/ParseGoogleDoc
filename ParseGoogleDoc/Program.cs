using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;

string url = "https://docs.google.com/document/d/e/2PACX-1vRMx5YQlZNa3ra8dYYxmv-QIQ3YJe8tbI3kqcuC7lQiZm-CSEznKfN_HYNSpoXcZIV3Y_O3YoUB1ecq/pub";

UnicodeGridRenderer.RenderGridFromPublishedDoc(url);

public class UnicodeGridRenderer
{
    public static void RenderGridFromPublishedDoc(string publishedDocUrl)
    {
        // 1. Download HTML as bytes and decode to UTF-8
        var http = new HttpClient();
        byte[] htmlBytes = http.GetByteArrayAsync(publishedDocUrl).GetAwaiter().GetResult();
        string html = Encoding.UTF8.GetString(htmlBytes);

        // 2. Load into HtmlAgilityPack
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // 3. Locate the table rows containing coordinates and characters
        //    Assumes the table is the first <table> under div#contents
        var main = doc.GetElementbyId("contents");
        var table = main.SelectSingleNode(".//table");
        if (table == null)
        {
            Console.Error.WriteLine("No table found in published doc.");
            return;
        }
        
        var txt = table.InnerText;
        int len = txt.Length / 3;
        int sizeX = 0;
        int sizeY = 0;
        for (int i = 0; i < len; i++)
        {
            int val = Convert.ToInt32(txt[i * 3]);
            if(val > sizeX) 
                sizeX = val;
            
            val = Convert.ToInt32(txt[i * 3 + 2]);
            if(val > sizeY) 
                sizeY = val;
        }
        
        char [, ] grid = new char[sizeX, sizeY];
        
    }
}
