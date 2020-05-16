using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSVFiles;

namespace IWDPacker
{
    public class SoundAliasCSVFile : CSVFile
    {
        string _searchDir;
        public string FileName { get; private set; }

        public SoundAliasCSVFile(string searchDir, string fileName)
            : base(Path.Combine(searchDir, fileName + ".csv"))
        {
            _searchDir = searchDir;
            FileName = fileName;
        }

        public List<string> GetSoundFileList()
        {
            List<string> files = new List<string>();

            int nameColumnI = -1;
            int fileColumnI = -1;
            foreach (List<string> lineColumns in GetItems())
            {
                if (lineColumns.Count > 0)
                {
                    if (nameColumnI == -1)
                    { 
                        for (int i = 0; i < lineColumns.Count; i++)
                        {
                            if (lineColumns[i] == "name")
                                nameColumnI = i;
                            else if (lineColumns[i] == "file")
                                fileColumnI = i;
                        }
                        if (nameColumnI == -1 || fileColumnI == -1)
                            throw new ApplicationException("Could not read column-specify line in soundalias '" + FileName + "'");
                    }
                    else
                        files.Add(CODTraps.ReplacePathSeps(lineColumns[fileColumnI]));
                }
            }

            return files;
        }
    }
}
