namespace Avanade.Platform.Services.Web.Configuration.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using NLog;

    internal sealed class IniFileProcessor
    {
        #region Fields

        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Loads the specified file info for the external config file.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        public static IDictionary<string, IConfigSettingCollection> Load(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return null;
            }

            return Load(fileInfo.OpenRead());
        }

        /// <summary>
        /// Loads the configuration from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the configuration.</param>
        public static IDictionary<string, IConfigSettingCollection> Load(Stream stream)
        {
            //track line numbers for exceptions
            int lineNumber = 0;

            //groups found
            var groups = new List<ConfigSettingCollection>();

            //current group information
            string currentGroupName = null;
            List<IConfigSetting> settings = null;

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        string line = reader.ReadLine();

                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        lineNumber++;

                        //strip out comments
                        if (line.Contains("#"))
                        {
                            if (line.IndexOf("#") == 0)
                            {
                                continue;
                            }
                            line = line.Substring(0, line.IndexOf("#"));
                        }

                        //trim off any extra whitespace
                        line = line.Trim();

                        //try to match a group name
                        var match = Regex.Match(line, "\\[[a-zA-Z\\d\\s]+\\]");

                        //found group name

                        if (match.Success)
                        {
                            //if we have a current group we're on, we save it
                            settings = DoProcessGroup(match, settings, ref currentGroupName, groups);
                        }

                        //no group name, check for setting with equals sign
                        else if (line.Contains("="))
                        {
                            DoProcessLineItem(line, settings);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Error("Problem encountered parsing the file at line number " + lineNumber, exception);
                    }
                }
            }

            //make sure we save off the last group
            if (settings != null && currentGroupName != null)
                groups.Add(new ConfigSettingCollection(currentGroupName, settings));

            //create our new group dictionary
            IDictionary<string, IConfigSettingCollection> settingGroups = new Dictionary<string, IConfigSettingCollection>();

            //add each group to the dictionary
            foreach (ConfigSettingCollection group in groups)
            {
                settingGroups.Add(group.Name, group);
            }

            return settingGroups;
        }

        private static void DoProcessArrayedItem(string[] parts)
        {
            string[] pieces = parts[1].Split(',');

            //need to build a new string
            var builder = new StringBuilder();

            for (int i = 0; i < pieces.Length; i++)
            {
                //trim off whitespace
                string s = pieces[i].Trim();

                //convert to lower case
                string t = s.ToLower();

                //check for any of the true values
                if (t == "on" || t == "yes" || t == "true")
                    s = "true";

                    //check for any of the false values
                else if (t == "off" || t == "no" || t == "false")
                    s = "false";

                //append the value
                builder.Append(s);

                //if we are not on the last value, add a comma
                if (i < pieces.Length - 1)
                    builder.Append(",");
            }

            //save the built string as the value
            parts[1] = builder.ToString();
        }

        private static List<IConfigSetting> DoProcessGroup(Capture match, List<IConfigSetting> settings, ref string currentGroupName, ICollection<ConfigSettingCollection> groups)
        {
            if (settings != null && currentGroupName != null)
            {
                groups.Add(new ConfigSettingCollection(currentGroupName, settings));
            }

            //set our current group information
            currentGroupName = match.Value.Substring(1, match.Length - 2);
            settings = new List<IConfigSetting>();
            return settings;
        }

        private static void DoProcessLineItem(string line, ICollection<IConfigSetting> settings)
        {
            string[] parts = new string[2];

            int index = line.IndexOf('=');
            string part1 = line.Substring(0, index);
            string part2 = line.Substring(index + 1);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("First: {0}, Second: {1}", part1, part2);
            }

            parts[0] = part1.Trim();
            parts[1] = part2.Trim();

            //figure out if we have an array or not
            bool isArray = false;
            bool inString = false;

            //go through the characters
            foreach (char c in parts[1])
            {
                //any comma not in a string makes us creating an array
                if (c == ',' && !inString)
                    isArray = true;

                    //flip the inString value each time we hit a quote
                else if (c == '"')
                    inString = !inString;
            }

            //if we have an array, we have to trim off whitespace for each item and
            //do some checking for boolean values.
            if (isArray)
            {
                //split our value array
                DoProcessArrayedItem(parts);
            }
            else
            {
                //make sure we are not working with a string value
                DoProcessNonArrayedItem(parts);
            }

            //add the setting to our list making sure, once again, we have stripped
            //off the whitespace
            if (settings != null)
            {
                settings.Add(new Setting(parts[0].Trim(), parts[1].Trim(), isArray));
            }
        }

        private static void DoProcessNonArrayedItem(string[] parts)
        {
            if (!parts[1].StartsWith("\""))
            {
                //convert to lower
                string t = parts[1].ToLower();

                //check for any of the true values
                if (t == "on" || t == "yes" || t == "true")
                    parts[1] = "true";

                    //check for any of the false values
                else if (t == "off" || t == "no" || t == "false")
                    parts[1] = "false";
            }
        }

        #endregion Methods
    }
}