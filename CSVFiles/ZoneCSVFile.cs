using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using CSVFiles;

namespace IWDPacker
{
    public class ZoneCSVFile : CSVFile
    {
        string _searchDir;
        public string FileName { get; private set; }

        List<ZoneCSVFile> _includes = new List<ZoneCSVFile>();
        List<ZoneCSVFile> _ignores = new List<ZoneCSVFile>();

        public ZoneCSVFile(string searchDir, string fileName)
            : base(Path.Combine(searchDir, fileName + ".csv"))
        {
            _searchDir = searchDir;
            FileName = fileName;
        }

        public new void Read()
        {
            base.Read();

            List<string> includes = GetFileList(ZoneCSVFileType.Include);
            includes.AddRange(GetFileList(ZoneCSVFileType.IncludePC));
            foreach (string inc in includes)
            {
                ZoneCSVFile include = new ZoneCSVFile(_searchDir, inc);
                include.Read();
                _includes.Add(include);
            }

            List<string> ignores = GetFileList(ZoneCSVFileType.Ignore);
            foreach (string ign in ignores)
            {
                ZoneCSVFile ignore = new ZoneCSVFile(_searchDir, ign);
                ignore.Read();
                _ignores.Add(ignore);
            }
        }

        public List<string> GetFileList(ZoneCSVFileType type)
        {
            List<string> files = new List<string>();
            List<string> ignoreFiles = new List<string>();

            foreach (ZoneCSVFile inc in _includes)
                files.AddRange(inc.GetFileList(type));

            foreach (List<string> lineColumns in GetItems())
            {
                if (lineColumns.Count > 0 && lineColumns[0] == type.Type)
                {
                    string file = ZoneCSVFileType.GetType(type).FileGetFunc(lineColumns);
                    if (!String.IsNullOrEmpty(file))
                        files.Add(file);
                }
            }

            foreach (ZoneCSVFile ign in _ignores)
                 ignoreFiles.AddRange(ign.GetFileList(type));

            return files.Except(ignoreFiles).ToList();
        }
    }

    public class ZoneCSVFileType
    {
        public string Type { get; private set; }
        public Func<List<string>, string> FileGetFunc { get; private set; }

        public ZoneCSVFileType(string type, Func<List<string>, string> func)
        {
            Type = type;
            FileGetFunc = func;
        }

        static List<ZoneCSVFileType> _types;

        public static ZoneCSVFileType GetType(ZoneCSVFileType type)
        {
            if (_types == null)
            {
                _types = new List<ZoneCSVFileType>();

                foreach (FieldInfo f in typeof(ZoneCSVFileType).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (f.FieldType == typeof(ZoneCSVFileType))
                        _types.Add((ZoneCSVFileType)f.GetValue(null));
                }
            }

            ZoneCSVFileType foundType = _types.Find(a => a == type);
            if (foundType != null)
                return foundType;

            throw new ApplicationException("Unsupported zone file type '" + type + "'");
        }

        public static ZoneCSVFileType Include = new ZoneCSVFileType("include", ZoneCSVFileType.ProcessInclude);
        public static ZoneCSVFileType IncludePC = new ZoneCSVFileType("include_pc", ZoneCSVFileType.ProcessInclude);
        public static string ProcessInclude(List<string> columns)
        {
            if (columns.Count > 1)
                return columns[1];

            return null;
        }

        public static ZoneCSVFileType Ignore = new ZoneCSVFileType("ignore", ZoneCSVFileType.ProcessIgnore);
        public static string ProcessIgnore(List<string> columns)
        {
            if (columns.Count > 1)
                return columns[1];

            return null;
        }

        public static ZoneCSVFileType Image = new ZoneCSVFileType("image", ZoneCSVFileType.ProcessImage);
        public static string ProcessImage(List<string> columns)
        {
            if (columns.Count > 1)
                return columns[1];

            return null;
        }

        public static ZoneCSVFileType Weapon = new ZoneCSVFileType("weapon", ZoneCSVFileType.ProcessWeapon);
        public static string ProcessWeapon(List<string> columns)
        {
            if (columns.Count > 1)
                return CODTraps.ReplacePathSeps(columns[1]);

            return null;
        }

        public static ZoneCSVFileType Material = new ZoneCSVFileType("material", ZoneCSVFileType.ProcessMaterial);
        public static string ProcessMaterial(List<string> columns)
        {
            if (columns.Count > 1)
                return columns[1];

            return null;
        }

        public static ZoneCSVFileType LoadedSound = new ZoneCSVFileType("loaded_sound", ZoneCSVFileType.ProcessLoadedSound);
        public static string ProcessLoadedSound(List<string> columns)
        {
            if (columns.Count > 1)
                return CODTraps.ReplacePathSeps(columns[1]);

            return null;
        }

        public static ZoneCSVFileType Sound = new ZoneCSVFileType("sound", ZoneCSVFileType.ProcessSound);
        public static string ProcessSound(List<string> columns)
        {
            if (columns.Count >= 4
                && (columns[3] == "all_sp" || columns[3] == "all_mp"))
                return columns[1];

            return null;
        }
    }
}
