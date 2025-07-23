using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

string url =
    "https://docs.google.com/document/d/e/2PACX-1vRMx5YQlZNa3ra8dYYxmv-QIQ3YJe8tbI3kqcuC7lQiZm-CSEznKfN_HYNSpoXcZIV3Y_O3YoUB1ecq/pub";
string assesUrl =
    "https://docs.google.com/document/d/e/2PACX-1vSZ1vDD85PCR1d5QC2XwbXClC1Kuh3a4u0y3VbTvTFQI53erafhUkGot24ulET8ZRqFSzYoi3pLTGwM/pub";

Console.OutputEncoding = Encoding.UTF8;
RenderGridFromPublishedDoc(assesUrl);
void RenderGridFromPublishedDoc(string publishedDocUrl)
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
    var content = txt.Replace("x-coordinateCharactery-coordinate", "");
    string pattern = @"(\d{1,2})([^\d]+?)(\d{1})";
    MatchCollection matches = Regex.Matches(content, pattern);

    int size = matches.Count;
    char[,] grid = new char[size + 1, size + 1];
    for (int i = 0; i < size + 1; i++)
    {
        for (int j = 0; j < size + 1; j++)
        {
            grid[i, j] = ' ';
        }
    }

    foreach (Match match in matches)
    {
        if (match.Groups.Count != 4) continue;

        string firstIndexStr = match.Groups[1].Value;
        string middleText = match.Groups[2].Value;
        string lastIndexStr = match.Groups[3].Value;

        if (!int.TryParse(firstIndexStr, out int firstIndex))
            continue;

        if (!int.TryParse(lastIndexStr, out int lastIndex))
            continue;

        int posX = firstIndex;
        int posY = lastIndex;
        grid[posY, posX] = middleText[0];
    }

    int row = grid.GetLength(0);
    int col = grid.GetLength(1);
    for (int i = 0; i < row; i++)
    {
        for (int j = 0; j < col; j++)
            Console.Write(grid[row - i - 1, j] + " ");

        Console.WriteLine();
    }
}