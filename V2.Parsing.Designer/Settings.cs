using System;
using System.IO;
using System.Xml.Linq;

namespace V2.Parsing.Designer
{
    class Settings
    {
        public string DirPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parsing");

        public string SettingsFilePath => Path.Combine(DirPath, "Settings.xml");

        public void Load()
        {
            if (File.Exists(SettingsFilePath))
            {
                var xDocument = XDocument.Load(SettingsFilePath);

                GrammarDefFilePath = xDocument.Root.Attribute("grammarDefFilePath").Value;
            }
        }

        public void Save()
        {
            XDocument xDocument = new XDocument(new XElement("root", new XAttribute("grammarDefFilePath", GrammarDefFilePath)));

            xDocument.Save(SettingsFilePath);
        }

        public string GrammarDefFilePath { get; set; }
    }
}