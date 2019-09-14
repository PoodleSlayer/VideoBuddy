using System;
using System.Collections.Generic;
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
			string filePath = GetAppFolder();
		}

		private string GetAppFolder()
		{
			string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			return "";
		}
    }
}
