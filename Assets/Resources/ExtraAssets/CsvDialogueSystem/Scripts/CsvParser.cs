using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utils for parsing comma-separated values (CSV) files
/// </summary>
public class CsvParser : MonoBehaviour
{
    public static CsvParser Instance;                                           // Singleton

    private char lineSeparator = '\n';                                          // Table line separate character
    private char fieldSeparator = ';';                                          // Table field separate chracter
    private string pageSign = "__Page_";                                        // Dialog page separator
    private string lineSign = "__Line_";                                        // Dialog line separator
    private string variableSign = "_";                                          // Dialog variables separator
    private string[] wasteSymbols = { "\r" };                                   // Symbols will be removed during page parsing

    void Awake()
    {
        Instance = this;                                                        // Make singleton Instance
    }

    /// <summary>
    /// Get page by name from CSV file with dialog descriptor
    /// </summary>
    /// <param name="pageName"> Name of page </param>
    /// <param name="desc"> CSV file with dialog descriptor </param>
    /// <returns> Dialog page with name "pageName" </returns>
    public string GetPage(string pageName, TextAsset desc)
    {
        if (pageName == null || desc == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        string res = null;
        int idx;
        idx = desc.text.IndexOf(pageSign + pageName);                           // Find name of page
        if (idx == -1)
        {
            Debug.Log("No such page");
            return null;
        }
        res = desc.text.Substring(idx);                                         // Cut
        foreach (string waste in wasteSymbols)                                  // Remove waste symbols
        {
            res = res.Replace(waste, "");
        }
        idx = res.IndexOf(lineSign);                                            // Find first line in page
        if (idx == -1)
        {
            Debug.Log("Line data error");
            return null;
        }
        res = res.Substring(idx);                                               // Cut
        idx = res.IndexOf(pageSign);                                            // Find end of page (start of new page)
        if (idx == -1)
        {
            return res;                                                         // This is last page
        }
        res = res.Remove(idx);                                                  // Cut
        res = pageSign + pageName + lineSeparator + res;                        // Add page name into start of page
        return res;
    }

    /// <summary>
    /// Get name of page
    /// </summary>
    /// <param name="page"> Page from CSV dialog descriptor </param>
    /// <returns> Name of page </returns>
    public string GetPageName(string page)
    {
        if (page == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        string res = null;
        int idx;
        idx = page.IndexOf(pageSign);                                           // Find start of page
        if (idx == -1)
        {
            Debug.Log("Page data error");
            return null;
        }
        res = page.Substring(idx);                                              // Cut
        idx = res.IndexOf(lineSeparator);                                       // Find end of page name
        if (idx == -1)
        {
            Debug.Log("Page data error");
            return null;
        }
        res = res.Remove(idx);                                                  // Cut
        res = res.Replace(pageSign, "");                                        // Remove pageSign from page name
        return res;
    }

    /// <summary>
    /// Get line by name from page
    /// </summary>
    /// <param name="lineName"> Name of line </param>
    /// <param name="page"> Page from CSV dialog descriptor </param>
    /// <returns> Line with name "lineName" </returns>
    public string GetLine(string lineName, string page)
    {
        string res = null;
        if (lineName == null || page == null)
        {
            Debug.Log("Wrong input data");
            return res;
        }
        int idx;
        idx = page.IndexOf(lineSign + lineName);                                // Find name of line
        if (idx == -1)
        {
            Debug.Log("Line data error");
            return null;
        }
        res = page.Substring(idx);                                              // Cut
        idx = res.IndexOf(lineSeparator);                                       // Find end of line
        if (idx == -1)
        {
            return res;                                                         // This is last line
        }
        res = res.Remove(idx);                                                  // Cut
        return res;
    }

    /// <summary>
    /// Get line by name from CSV file with dialog descriptor
    /// </summary>
    /// <param name="lineName"> Name of line in page </param>
    /// <param name="pageName"> Name of page </param>
    /// <param name="desc"> CSV file with dialog descriptor </param>
    /// <returns> Line with name "lineName" from page with name "pageName" </returns>
    public string GetLine(string lineName, string pageName, TextAsset desc)
    {
        string res = null;
        if (lineName == null || pageName == null || desc == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        res = GetPage(pageName, desc);                                          // Find page
        if (res != null)
        {
            res = GetLine(lineName, res);                                       // Find line
        }
        return res;
    }

    /// <summary>
    /// Get all lines from page
    /// </summary>
    /// <param name="page"> Page from CSV dialog descriptor </param>
    /// <returns> List of lines contain "lineSign" </returns>
    public List<string> GetLines(string page)
    {
        if (page == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        List<string> res = new List<string>();
        string[] lines = page.Split(lineSeparator);                             // Split by lineSeparator
        foreach (string line in lines)
        {
            if (line.Contains(lineSign) == true)
            {
                res.Add(line);
            }
        }
        return res;
    }

    /// <summary>
    /// Get all lines from page in CSV file with dialog descriptor
    /// </summary>
    /// <param name="pageName"> Name of page </param>
    /// <param name="desc"> CSV dialog descriptor </param>
    /// <returns> List of lines contain "lineSign" in page with name "pageName" </returns>
    public List<string> GetLines(string pageName, TextAsset desc)
    {
        if (pageName == null || desc == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        string parse = GetPage(pageName, desc);                                 // Find page
        return GetLines(parse);                                                 // Get all lines
    }

    /// <summary>
    /// Get name of line
    /// </summary>
    /// <param name="line"> Line from CSV dialog descriptor </param>
    /// <returns> Name of line </returns>
    public string GetLineName(string line)
    {
        if (line == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        string res = null;
        int idx;
        idx = line.IndexOf(lineSign);                                           // Find start of line
        if (idx == -1)
        {
            Debug.Log("Line data error");
            return null;
        }
        res = line.Substring(idx);                                              // Cut
        idx = res.IndexOf(fieldSeparator);                                      // Find end of line name
        if (idx == -1)
        {
            Debug.Log("Line data error");
            return null;
        }
        res = res.Remove(idx);                                                  // Cut
        res = res.Replace(lineSign, "");                                        // Remove lineSign from line name
        return res;
    }

    /// <summary>
    /// Get lines with "lineName" from page
    /// </summary>
    /// <param name="lineName"> Name of lines </param>
    /// <param name="page"> Page from CSV dialog descriptor </param>
    /// <returns> List of lines with name "lineName" </returns>
    public List<string> GetLines(string lineName, string page)
    {
        if (lineName == null || page == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        List<string> res = new List<string>();
        string[] lines = page.Split(lineSeparator);                             // Split page by lineSeparator
        foreach (string line in lines)
        {
            if (line.Contains(lineSign + lineName))                             // Find line with name "lineName"
            {
                res.Add(line);
            }
        }
        return res;
    }

    /// <summary>
    /// Get lines with "lineName" from page with name "pageName" in CSV file with dialog descriptor
    /// </summary>
    /// <param name="lineName"> Name of lines </param>
    /// <param name="pageName"> Name of page </param>
    /// <param name="desc"> CSV dialog descriptor </param>
    /// <returns> List of lines with name "lineName" from page with name "pageName" </returns>
    public List<string> GetLines(string lineName, string pageName, TextAsset desc)
    {
        if (lineName == null || pageName == null || desc == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        string parse = GetPage(pageName, desc);                                 // Find page
        return GetLines(lineName, parse);                                       // Get lines with name "lineName"
    }

    /// <summary>
    /// Get value from next field after "variableName" field (inclusive line name)
    /// </summary>
    /// <param name="outData"> Output value </param>
    /// <param name="variableName"> Name of variable </param>
    /// <param name="line"> Line from CSV dialog descriptor </param>
    /// <returns> true - success, false - no data </returns>
    private bool GetValue(out string outData, string variableName, string line)
    {
        bool res = false;
        outData = null;
        if (variableName == null || line == null)
        {
            Debug.Log("Wrong input data");
            return res;
        }
        int idx = 0;
        string[] fields = line.Split(fieldSeparator);                           // Split the line
        foreach (string field in fields)
        {
            idx++;
            if (    field == (lineSign + variableName)
                ||  field == (variableSign + variableName))                     // Search for "variableName" (inclusive line name)
            {
                break;
            }
        }
        if (idx < fields.Length)                                                // Field finded
        {
            outData = fields[idx];
            res = true;
        }
        return res;
    }

    /// <summary>
    /// Get text from next field after "variableName" field (inclusive line name)
    /// </summary>
    /// <param name="outData"> Output text </param>
    /// <param name="variableName"> Name of variable </param>
    /// <param name="line"> Line from CSV dialog descriptor </param>
    /// <returns> true - success, false - no data </returns>
    public bool GetTextValue(out string outData, string variableName, string line)
    {
        bool res = false;
        outData = null;
        if (variableName == null || line == null)
        {
            Debug.Log("Wrong input data");
            return res;
        }
        string value;
        if (GetValue(out value, variableName, line) == true)                    // Get value with name "variableName"
        {
            value = value.Replace(variableSign, "");                            // Remove variableSign from text if it is
            outData = value;
            res = true;
        }
        return res;
    }

    /// <summary>
    /// Get number from next field after "variableName" field (inclusive line name)
    /// </summary>
    /// <param name="outData"> Output number </param>
    /// <param name="variableName"> Name of variable </param>
    /// <param name="line"> Line from CSV dialog descriptor </param>
    /// <returns> true - success, false - no data </returns>
    public bool GetNumValue(out int outData, string variableName, string line)
    {
        bool res = false;
        outData = 0;
        if (variableName == null || line == null)
        {
            Debug.Log("Wrong input data");
            return res;
        }
        string value;
        if (GetValue(out value, variableName, line) == true)                    // Get value with name "variableName"
        {
            if (int.TryParse(value, out outData) == false)                      // Convert to number
            {
                Debug.Log("Wrong data in field");
                return res;
            }
            res = true;
        }
        return res;
    }

    /// <summary>
    /// Split line into lines list by variable with "variableName"
    /// </summary>
    /// <param name="variableName"> Name of variable </param>
    /// <param name="line"> Line from CSV dialog descriptor </param>
    /// <returns> List of splited lines. All lines have name equals "variableName" </returns>
    public List<string> SplitLineByValue(string variableName, string line)
    {
        if (line == null)
        {
            Debug.Log("Wrong input data");
            return null;
        }
        List<string> res = new List<string>();
        string[] fields = line.Split(fieldSeparator);                           // Split line by fieldSeparator
        bool hit = false;
        string splitedLine = null;
        foreach (string field in fields)
        {
            if (field == variableSign + variableName)
            {
                hit = true;                                                     // Value finded
                if (splitedLine != null)                                        // If output line compiled
                {
                                                                                // Add name equals "variableName" into start of line
                    splitedLine = lineSign + variableName + fieldSeparator + splitedLine;
                    res.Add(splitedLine);
                    splitedLine = null;
                }
            }
            else
            {
                if (hit == true)                                                // If value was finded
                {
                    splitedLine += field + fieldSeparator;                      // Add field to output line
                }
            }
        }
        if ((hit == true) && (splitedLine != null))                             // At the end of line. If value was finded
        {
                                                                                // Add name equals "variableName" into start of line
            splitedLine = lineSign + variableName + fieldSeparator + splitedLine;
            res.Add(splitedLine);
        }
        return res;
    }
}
