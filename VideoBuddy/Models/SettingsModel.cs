using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoBuddy.IoC;

namespace VideoBuddy.Models
{
    public class SettingsModel
    {
		public string YtdlLocation { get; set; }
		public string DownloadLocation { get; set; }
    }
}
