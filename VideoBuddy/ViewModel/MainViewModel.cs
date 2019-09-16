using Autofac;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoBuddy.IoC;
using VideoBuddy.Models;

namespace VideoBuddy.ViewModel
{
    public class MainViewModel : VBViewModel
    {
		public RelayCommand DownloadCommand { get; set; }

		private IFileService fileHelper;
		private SettingsModel settings;

		public MainViewModel() : base()
		{
			// load settings...?
			fileHelper = AppContainer.Container.Resolve<IFileService>();
			settings = fileHelper.LoadSettings();
		}

		public override void SetupViewModel()
		{
			SetupCommands();
		}

		private void SetupCommands()
		{
			DownloadCommand = new RelayCommand(() =>
			{
				Download();
			});
		}

		private void Download()
		{
			if (String.IsNullOrEmpty(downloadURL))
			{
				return;
			}


		}


		private string downloadURL;
		public string DownloadURL
		{
			get => downloadURL;
			set
			{
				if (value != downloadURL)
				{
					downloadURL = value;
					RaisePropertyChanged("DownloadURL");
				}
			}
		}
	}
}