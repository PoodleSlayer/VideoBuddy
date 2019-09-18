using Autofac;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoBuddy.IoC;
using VideoBuddy.Models;
using VideoBuddy.Utility;

namespace VideoBuddy.ViewModel
{
	public class SettingsViewModel : VBViewModel
	{
		public RelayCommand GoBackCommand { get; set; }
		public RelayCommand SaveCommand { get; set; }

		private SettingsModel settings;
		private IFileService fileService;

		public override void SetupViewModel()
		{
			SetupCommands();
			fileService = AppContainer.Container.Resolve<IFileService>();
			settings = fileService.LoadSettings();
			if (!String.IsNullOrEmpty(settings.DownloadLocation))
			{
				downloadLocation = settings.DownloadLocation;
			}
			if (!String.IsNullOrEmpty(settings.YtdlLocation))
			{
				ytdlLocation = settings.YtdlLocation;
			}
		}

		private void SetupCommands()
		{
			GoBackCommand = new RelayCommand(() =>
			{
				// actually maybe just do this in the UI
			});
			SaveCommand = new RelayCommand(() =>
			{
				settings.DownloadLocation = downloadLocation;
				settings.YtdlLocation = ytdlLocation;
				fileService.SaveSettings(settings);
			});
		}

		private string ytdlLocation;
		public string YtdlLocation
		{
			get
			{
				if (String.IsNullOrEmpty(ytdlLocation))
				{
					return "Specify the youtube-dl location...";
				}
				return ytdlLocation;
			}
			set
			{
				if (value != ytdlLocation)
				{
					ytdlLocation = value;
					RaisePropertyChanged("YtdlLocation");
				}
			}
		}

		private string downloadLocation;
		public string DownloadLocation
		{
			get
			{
				if (String.IsNullOrEmpty(downloadLocation))
				{
					return "Specify the download location...";
				}
				return downloadLocation;
			}
			set
			{
				if (value != downloadLocation)
				{
					downloadLocation = value;
					RaisePropertyChanged("DownloadLocation");
				}
			}
		}
	}
}
