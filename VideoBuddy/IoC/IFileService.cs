using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoBuddy.Models;

namespace VideoBuddy.IoC
{
    public interface IFileService
    {
		void SaveSettings(SettingsModel settings);
    }
}
