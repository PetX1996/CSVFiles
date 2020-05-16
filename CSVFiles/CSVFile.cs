using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CSVFiles
{
    public class CSVFile
    {
        public string FilePath { get; private set; }

        List<List<string>> _items = new List<List<string>>();

        public CSVFile(string filePath)
        {
            FilePath = filePath;
            //Read(filePath);
        }

        public void Read()
        {
            string[] lines = File.ReadAllLines(FilePath);

            for (int lineI = 0; lineI < lines.Length; lineI++)
            {
                List<string> columns = new List<string>();
                bool isQuote = false;
                int ch;
                int nextCh;
                int realIndex = 0;

                StringBuilder columnSB = new StringBuilder();

                bool isComment = false;

                for (int i = 0; i < lines[lineI].Length; i++)
                {
                    ch = lines[lineI][i];
                    nextCh = (i + 1) < lines[lineI].Length ? lines[lineI][i + 1] : -1;

                    if (realIndex == 0 && (ch == ' ' || ch == '\t'))
                        continue;
                    else if (ch == '"')
                    {
                        isQuote = !isQuote;
                        continue;
                    }
                    else if (ch == ',' && !isQuote)
                    {
                        if (realIndex == 0) // first column is empty => comment
                        {
                            isComment = true;
                            break;
                        }

                        columns.Add(columnSB.ToString());
                        columnSB = new StringBuilder();
                        continue;
                    }
                    else if (realIndex == 0 && (ch == '#' || (ch == '/' && nextCh == '/'))) // comment
                    {
                        isComment = true;
                        break;
                    }

                    columnSB.Append((char)ch);
                    realIndex++;
                }

                if (!isComment)
                {
                    if (isQuote)
                        throw new ApplicationException("Found unclosed quotes in csv, line " + (lineI + 1) + ", file " + FilePath);

                    if (columnSB.Length != 0)
                        columns.Add(columnSB.ToString());

                    _items.Add(columns);
                }
            }
        }

        public List<List<string>> GetItems()
        {
            return _items;
        }

        public string GetValueByColumn(int columnIndex, string columnValue, int returnIndex)
        {
            foreach (List<string> lineColumns in _items)
            {
                if (lineColumns.Count <= columnIndex)
                    continue;

                if (lineColumns[columnIndex] == columnValue)
                    return lineColumns[returnIndex];
            }
            return null;
        }

        public string GetValueByLine(int lineIndex, int returnIndex)
        {
            if (lineIndex >= _items.Count || returnIndex >= _items[lineIndex].Count)
                return null;

            return _items[lineIndex][returnIndex];
        }
    }
}
