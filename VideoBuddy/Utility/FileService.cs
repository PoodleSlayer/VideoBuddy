using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoBuddy.IoC;
using VideoBuddy.Models;

namespace VideoBuddy.Utility
{
    public class FileService : IFileService
    {
		public void SaveSettings(SettingsModel settings)
		{
			// write the current settings to a JSON file in the appdata directory
			string filePath = GetAppSettingsFolder();
			string settingsFile = filePath + @"settings.json";
			string jsonText = JsonConvert.SerializeObject(settings, Formatting.Indented);
			File.WriteAllText(settingsFile, jsonText);
		}

		public SettingsModel LoadSettings()
		{
			string filePath = GetAppSettingsFolder();
			string settingsFile = filePath + @"settings.json";
			if (!File.Exists(settingsFile))
			{
				return new SettingsModel();
			}

			string jsonText = File.ReadAllText(settingsFile);
			if (String.IsNullOrEmpty(jsonText))
			{
				return new SettingsModel();
			}

			SettingsModel settings = JsonConvert.DeserializeObject<SettingsModel>(jsonText);
			return settings;
		}

		private string GetAppSettingsFolder()
		{
			string appFolder = AppDomain.CurrentDomain.BaseDirectory;
			string settingsFolder = appFolder + @"settings\";
			if (!Directory.Exists(settingsFolder))
			{
				DirectoryInfo dirInfo = Directory.CreateDirectory(settingsFolder);
			}
			return settingsFolder;
		}
    }
}
