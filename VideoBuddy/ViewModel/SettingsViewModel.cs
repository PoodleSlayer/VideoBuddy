using Autofac;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public RelayCommand SaveCommand { get; set; }
		public RelayCommand UpdateCommand { get; set; }

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

		/// <summary>
		/// This method is called from the SettingsPage's Loaded event
		/// and can be used as a corresponding life cycle event
		/// </summary>
		public void PageLoaded()
		{
			CheckYtdlVersion();
		}

		private void SetupCommands()
		{
			SaveCommand = new RelayCommand(() =>
			{
				settings.DownloadLocation = downloadLocation;
				settings.YtdlLocation = ytdlLocation;
				fileService.SaveSettings(settings);
			});
			UpdateCommand = new RelayCommand(() =>
			{
				UpdateYtdl();
			});
		}

		private void UpdateYtdl()
		{
			string ytdlPath = settings.YtdlLocation;
			ytdlPath += @"\youtube-dl.exe";

			Process ytdlProcess = new Process();
			ytdlProcess.StartInfo.FileName = ytdlPath;
			ytdlProcess.StartInfo.Arguments = "--update";
			ytdlProcess.StartInfo.UseShellExecute = false;
			ytdlProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			ytdlProcess.StartInfo.CreateNoWindow = true;
			ytdlProcess.StartInfo.RedirectStandardOutput = true;

			ytdlProcess.OutputDataReceived += OutputReceived;

			ytdlProcess.Start();
			ytdlProcess.BeginOutputReadLine();
		}

		/// <summary>
		/// Checks the current version of youtube-dl
		/// </summary>
		private void CheckYtdlVersion()
		{
			// okay I have three different methods that all just run a Process now,
			// probably need to pull this out into a helper method

			// also, to avoid spamming this every time the Settings Page is loaded,
			// I might add a hidden Settings flag which will keep track of when the
			// user updates their version. Then I can check that before attempting
			// to see if their version even needs to be checked
			string ytdlPath = settings.YtdlLocation;
			ytdlPath += @"\youtube-dl.exe";

			Process ytdlProcess = new Process();
			ytdlProcess.StartInfo.FileName = ytdlPath;
			ytdlProcess.StartInfo.Arguments = "--version";
			ytdlProcess.StartInfo.UseShellExecute = false;
			ytdlProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			ytdlProcess.StartInfo.CreateNoWindow = true;
			ytdlProcess.StartInfo.RedirectStandardOutput = true;

			ytdlProcess.OutputDataReceived += VersionOutputReceived;

			ytdlProcess.Start();
			ytdlProcess.BeginOutputReadLine();
		}

		/// <summary>
		/// Used for logging general youtube-dl output to the TextBox at the bottom
		/// of the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OutputReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null && !String.IsNullOrEmpty(e.Data))
			{
				YtdlOutput = e.Data + "\n" + YtdlOutput;
			}
		}

		/// <summary>
		/// Used specifically for getting the youtube-dl version string
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VersionOutputReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null && !String.IsNullOrEmpty(e.Data))
			{
				YtdlVersion = e.Data;
			}
		}

		#region Properties

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

		private string ytdlOutput;
		public string YtdlOutput
		{
			get
			{
				if (String.IsNullOrEmpty(ytdlOutput))
				{
					return "output from youtube-dl command line";
				}
				return ytdlOutput;
			}
			set
			{
				if (value != ytdlOutput)
				{
					ytdlOutput = value;
					RaisePropertyChanged("YtdlOutput");
				}
			}
		}

		private string ytdlVersion;
		public string YtdlVersion
		{
			get
			{
				if (String.IsNullOrEmpty(ytdlVersion))
				{
					return "0.0";
				}
				return ytdlVersion;
			}
			set
			{
				if (value != ytdlVersion)
				{
					ytdlVersion = value;
					RaisePropertyChanged("YtdlVersion");
				}
			}
		}

		#endregion Properties
	}
}
