using Autofac;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
			DownloadCommand = new RelayCommand(async () =>
			{
				YtdlOutput = "began downloading " + downloadURL + "...";
				await DownloadAsync();
			});
		}

		private async Task DownloadAsync()
		{
			// OKAY how do we make this thing actually run async

			if (String.IsNullOrEmpty(downloadURL))
			{
				return;
			}

			if (settings == null)
			{
				// what the heck? Settings not loaded
				YtdlOutput = "Settings failed to load D:";
				return;
			}

			if (String.IsNullOrEmpty(settings.DownloadLocation))
			{
				// prompt user to specify a download location

				// maybe a popup that allows them to proceed and default
				// to saving in the youtube-dl folder?
				YtdlOutput = "Please specify a Download Location in Settings!";
				return;
			}

			if (String.IsNullOrEmpty(settings.YtdlLocation))
			{
				// prompt user to specify a youtube-dl location
				YtdlOutput = "Please specify a Youtube-dl Location in Settings!";
				return;
			}

			string ytdlPath = settings.YtdlLocation;
			ytdlPath += @"\youtube-dl.exe";
			//string ytdlPath = @"your\path\YoutubeDL\youtube-dl.exe";
			string downloadPath = settings.DownloadLocation;
			string newFileName = String.IsNullOrEmpty(fileName) ? @"%(title)s.%(ext)s" : fileName + @".%(ext)s";
			downloadPath += @"\" + newFileName;
			//string downloadPath = @"your\path\Videos\%(title)s.%(ext)s";

			Process ytdlProcess = new Process();
			ytdlProcess.StartInfo.FileName = ytdlPath;
			ytdlProcess.StartInfo.Arguments = @"-f 137 -o """ + downloadPath + @""" """ + downloadURL + @"""";
			ytdlProcess.StartInfo.UseShellExecute = false;
			ytdlProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			ytdlProcess.StartInfo.CreateNoWindow = true;
			ytdlProcess.StartInfo.RedirectStandardOutput = true;

			ytdlProcess.OutputDataReceived += OutputReceived;

			ytdlProcess.Start();
			ytdlProcess.BeginOutputReadLine();
			//var ytdlOutput = ytdlProcess.StandardOutput.ReadToEnd();

			//ytdlProcess.WaitForExit();

			// example cmd line usage:
			// youtube-dl -f 137 -o "your\download\location\%(title)s.%(ext)s" "https://www.youtube.com/watch?v=5RsOkhR6KBc"
		}

		private void OutputReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null && !String.IsNullOrEmpty(e.Data))
			{
				// Top-down logging would need to auto-scroll. maybe something like:
				//	public class ScrollingTextBox : TextBox 
				//  {
				//		protected override void OnInitialized(EventArgs e)
				//		{
				//			base.OnInitialized(e);
				//			VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
				//			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
				//		}
				//		protected override void OnTextChanged(TextChangedEventArgs e)
				//		{
				//			base.OnTextChanged(e);
				//			CaretIndex = Text.Length;
				//			ScrollToEnd();
				//		}
				//	}
			
				//YtdlOutput += "\n" + e.Data;
				// maybe try bottom-up logging...?
				YtdlOutput = e.Data + "\n" + YtdlOutput;
			}
		}

		private string downloadURL;
		public string DownloadURL
		{
			get
			{
				if (String.IsNullOrEmpty(downloadURL))
				{
					return "URL to download...";
				}
				return downloadURL;
			}
			set
			{
				if (value != downloadURL)
				{
					downloadURL = value;
					RaisePropertyChanged("DownloadURL");
				}
			}
		}

		private string fileName;
		public string FileName
		{
			get
			{
				if (String.IsNullOrEmpty(fileName))
				{
					return "If empty, video title will be used...";
				}
				return fileName;
			}
			set
			{
				if (value != fileName)
				{
					fileName = value;
					RaisePropertyChanged("FileName");
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
	}
}