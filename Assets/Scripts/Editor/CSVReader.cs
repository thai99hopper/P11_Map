using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public static class CSVReader
{
    public static List<Dictionary<string, string>> Read(string filePath)
    {
        var list = new List<Dictionary<string, string>>();
        
        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV file not found at: {filePath}");
            return list;
        }
        
        string fileContent = File.ReadAllText(filePath);
        
        // Parse CSV properly handling multi-line quoted fields
        var rows = ParseCSVContent(fileContent);
        
        if (rows.Count <= 1) return list;
        
        string[] headers = rows[0];
        
        // Clean headers: remove quotes and newlines, normalize naming
        for (int i = 0; i < headers.Length; i++)
        {
            headers[i] = CleanHeaderName(headers[i]);
        }
        
        Debug.Log($"Headers found: {string.Join(", ", headers)}");
        
        for (int i = 1; i < rows.Count; i++)
        {
            string[] values = rows[i];
            if (values.Length == 0 || string.IsNullOrEmpty(values[0].Trim())) continue;
            
            var entry = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                string cleanValue = values[j].Replace("\"", "").Trim();
                entry[headers[j]] = cleanValue;
            }
            list.Add(entry);
        }
        
        return list;
    }
    
    private static List<string[]> ParseCSVContent(string content)
    {
        var result = new List<string[]>();
        var lines = content.Split('\n');
        
        List<string> currentRow = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool inQuotes = false;
        bool fieldStarted = false;
        
        foreach (string line in lines)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (c == '"')
                {
                    if (!fieldStarted)
                    {
                        inQuotes = true;
                        fieldStarted = true;
                    }
                    else if (inQuotes)
                    {
                        // Check for escaped quote
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            currentField.Append('"');
                            i++; // Skip next quote
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    currentRow.Add(currentField.ToString());
                    currentField.Clear();
                    fieldStarted = false;
                }
                else if (c == '\r')
                {
                    // Skip carriage return
                    continue;
                }
                else
                {
                    currentField.Append(c);
                    if (!fieldStarted) fieldStarted = true;
                }
            }
            
            // If we're in quotes, add newline and continue to next line
            if (inQuotes)
            {
                currentField.Append('\n');
            }
            else
            {
                // End of row
                currentRow.Add(currentField.ToString());
                result.Add(currentRow.ToArray());
                
                currentRow.Clear();
                currentField.Clear();
                fieldStarted = false;
            }
        }
        
        // Add last row if not empty
        if (currentRow.Count > 0 || currentField.Length > 0)
        {
            if (currentField.Length > 0)
            {
                currentRow.Add(currentField.ToString());
            }
            if (currentRow.Count > 0)
            {
                result.Add(currentRow.ToArray());
            }
        }
        
        return result;
    }
    
    private static string CleanHeaderName(string header)
    {
        return header.Replace("\"", "")
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace("_", "")
                    .Trim()
                    .ToLower();
    }
}
