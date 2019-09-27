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
using VideoBuddy.Utility;

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

			BothRadioValue = true;
		}

		/// <summary>
		/// This method is called from the MainPage's Loaded event
		/// and can be used as a corresponding life cycle event
		/// </summary>
		public void PageLoaded()
		{
			
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
			string downloadPath = settings.DownloadLocation;
			string newFileName = String.IsNullOrEmpty(fileName) ? @"%(title)s.%(ext)s" : fileName + @".%(ext)s";
			downloadPath += @"\" + newFileName;
			string videoFormat = "";
			if (VideoRadioValue)
			{
				videoFormat = "bestvideo";
			}
			else if (AudioRadioValue)
			{
				videoFormat = "bestaudio";
			}
			else  // BothRadioValue
			{
				videoFormat = "best";
			}

			string processArguments = "-f " + videoFormat + " -o \"" + downloadPath + "\" \"" + downloadURL + "\"";

			Process ytdlProcess = ProcessRunner.CreateProcess(ytdlPath, processArguments);

			ytdlProcess.OutputDataReceived += OutputReceived;

			ytdlProcess.Start();
			ytdlProcess.BeginOutputReadLine();

			// example cmd line usage:
			// youtube-dl -f 137 -o "your\download\location\%(title)s.%(ext)s" "https://www.youtube.com/watch?v=5RsOkhR6KBc"
		}

		/// <summary>
		/// This method will run a Process instance async
		/// </summary>
		/// <returns>The TaskCompletionSource for the Process</returns>
		private Task<int> RunProcessAsync()
		{
			var tcs = new TaskCompletionSource<int>();

			string ytdlPath = settings.YtdlLocation;
			ytdlPath += @"\youtube-dl.exe";
			string downloadPath = settings.DownloadLocation;
			string newFileName = String.IsNullOrEmpty(fileName) ? @"%(title)s.%(ext)s" : fileName + @".%(ext)s";
			downloadPath += @"\" + newFileName;
			string videoFormat = "";
			if (VideoRadioValue)
			{
				videoFormat = "bestvideo";
			}
			else if (AudioRadioValue)
			{
				videoFormat = "bestaudio";
			}
			else  // BothRadioValue
			{
				videoFormat = "best";
			}

			var ytdlProcess = new Process
			{
				StartInfo =
				{
					FileName = ytdlPath,
					Arguments = "-f " + videoFormat + " -o \"" + downloadPath + "\" \"" + downloadURL + "\"",
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
				},
				EnableRaisingEvents = true
			};

			ytdlProcess.Exited += (sender, args) =>
			{
				tcs.SetResult(ytdlProcess.ExitCode);
				ytdlProcess.Dispose();
			};

			ytdlProcess.OutputDataReceived += OutputReceived;
			ytdlProcess.Start();

			return tcs.Task;
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

		#region Properties

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

		private bool bothRadioValue;
		public bool BothRadioValue
		{
			get => bothRadioValue;
			set
			{
				if (value != bothRadioValue)
				{
					bothRadioValue = value;
					RaisePropertyChanged("BothRadioValue");
				}
			}
		}

		private bool videoRadioValue;
		public bool VideoRadioValue
		{
			get => videoRadioValue;
			set
			{
				if (value != videoRadioValue)
				{
					videoRadioValue = value;
					RaisePropertyChanged("VideoRadioValue");
				}
			}
		}

		private bool audioRadioValue;
		public bool AudioRadioValue
		{
			get => audioRadioValue;
			set
			{
				if (value != audioRadioValue)
				{
					audioRadioValue = value;
					RaisePropertyChanged("AudioRadioValue");
				}
			}
		}

		#endregion Properties
	}
}