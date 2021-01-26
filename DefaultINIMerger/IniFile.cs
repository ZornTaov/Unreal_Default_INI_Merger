using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DefaultINIMerger
{
    public class IniFile
    {
        //                section            item    values
        public Dictionary<string, Dictionary<string, List<string>>> SectionsMap { get; set; } = new Dictionary<string, Dictionary<string, List<string>>>();

        /**
         * Load data from target file
         * @param path target file. It should be in ini format
         */
        public void ReadFile(string path)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(path);
                string line = null;
                //         item    values
                Dictionary<string, List<string>> itemsMap = new Dictionary<string, List<string>>();

                string currentSection = "";
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if ("".Equals(line))
                        continue;
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        // Ends last section
                        if (itemsMap.Count > 0 && !"".Equals(currentSection.Trim()))
                        {
                            SectionsMap.Add(currentSection, itemsMap);
                        }
                        currentSection = "";
                        itemsMap = null;

                        // Start new section initial
                        currentSection = line.Substring(1, line.Length - 2);
                        itemsMap = new Dictionary<string, List<string>>();
                    }
                    else
                    {
                        int index = line.IndexOf("=");
                        if (index != -1)
                        {
                            if (line.StartsWith("+") || line.StartsWith("."))
                            {
                                string key = line.Substring(1, index - 1);
                                string value = line.Substring(index + 1, line.Length - 1 - index);
                                if (!itemsMap.ContainsKey(key))
                                {
                                    itemsMap.Add(key, new List<string>());
                                }
                                itemsMap[key].Add(value);
                            }
                            else if (line.StartsWith("-") || line.StartsWith("!"))
                            {
                                string key = line.Substring(1, index - 1);
                                string value = line.Substring(index + 1, line.Length - 1 - index);
                                if (itemsMap.ContainsKey(key) && itemsMap[key].Contains(value))
                                {
                                    if(itemsMap[key].Remove(value))
                                        Console.WriteLine("Removed " + value + " from " + key);
                                }
                            }
                            else
                            {
                                string key = line.Substring(0, index);
                                string value = line.Substring(index + 1, line.Length - 1 - index);
                                if (!itemsMap.ContainsKey(key))
                                {
                                    itemsMap.Add(key, new List<string>());
                                }
                                itemsMap[key].Add(value);
                            }
                        }
                    }
                }

                // Ends last section
                if (itemsMap.Count != 0 && !"".Equals(currentSection.Trim()))
                {
                    SectionsMap.Add(currentSection, itemsMap);
                }
                reader.Close();
            }
            finally
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (IOException e1)
                    {
                        Console.WriteLine(e1.StackTrace);
                        throw e1;
                    }
                }
            }
        }

        /**
         * Write contents of SectionsMap to path
         */
        public void WriteFile(string path)
        {
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(path);
                foreach (string section in SectionsMap.Keys.ToList())
                {
                    writer.WriteLine("[" + section + "]");
                    foreach (string key in SectionsMap[section].Keys.ToList())
                    {
                        if (SectionsMap[section][key].Count == 1)
                        {
                            writer.WriteLine(key + "=" + SectionsMap[section][key][0]);
                        }
                        else
                        {
                            foreach (string value in SectionsMap[section][key].ToList())
                            {
                                writer.WriteLine("+" + key + "=" + value);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (writer != null)
                {
                    try
                    {
                        writer.Close();
                    }
                    catch (IOException e1)
                    {
                        Console.WriteLine(e1.StackTrace);
                        throw e1;
                    }
                }
            }
        }

        /**
         * Dirty way of merging two IniFile's together
         */
        public void Merge(IniFile other)
        {
            foreach (string section in other.SectionsMap.Keys.ToList())
            {
                if (!SectionsMap.ContainsKey(section))
                {
                    SectionsMap.Add(section, other.SectionsMap[section]);
                }
                else
                {
                    foreach (string key in other.SectionsMap[section].Keys.ToList())
                    {
                        if (!SectionsMap[section].ContainsKey(key))
                        {
                            SectionsMap[section].Add(key, other.SectionsMap[section][key]);
                        }
                        else
                        {
                            foreach (string value in other.SectionsMap[section][key].ToList())
                            {
                                if (!SectionsMap[section][key].Contains(value))
                                {
                                    SectionsMap[section][key].Add(value);
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
         * The following methods are for further utility of class, goal of project was to just merge two ini files
         */

        /**
         * Attempts to get values by Section and Key, null otherwise
         */
        public List<string> GetValues(string section, string item)
        {
            SectionsMap.TryGetValue(section, out Dictionary<string, List<string>> map);
            if (map == null)
            {
                Console.WriteLine("No such section:" + section);
                return null;
            }
            map.TryGetValue(item, out List<string> value);
            if (value == null)
            {
                Console.WriteLine("No such item:" + item);
                return null;
            }
            return value;
        }

        /**
         * Returns list of section names.
         */
        public List<string> GetSectionNames()
        {
            List<string> list = new List<string>();
            foreach (string it in SectionsMap.Keys)
            {
                list.Add(it);
            }
            return list;
        }

        /**
         * Returns all items of a section
         */
        public Dictionary<string, List<string>> GetItemsBySectionName(string section)
        {
            SectionsMap.TryGetValue(section, out Dictionary<string, List<string>> output);
            return output;
        }
    }
}
