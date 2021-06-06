using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoBuddy.IoC;
using VideoBuddy.Models;
using VideoBuddy.Utility;
using Autofac;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using VideoBuddy.Utility.Messages;

namespace VideoBuddy.ViewModel
{
    public class DownloadViewModel : VBViewModel
    {
		public RelayCommand DownloadCommand { get; set; }
		public RelayCommand CancelCommand { get; set; }
		public event EventHandler SettingsWarning;
		public event EventHandler URLWarning;

		private IFileService fileHelper;
		private SettingsModel settings;
		private Process dlProcess;

		public DownloadViewModel() : base()
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
				await DownloadAsync();
			});
			CancelCommand = new RelayCommand(() =>
			{
				CancelDownload();
			});
		}

		private async Task DownloadAsync()
		{
			// OKAY how do we make this thing actually run async
			if (settings == null)
			{
				// what the heck? Settings not loaded
				YtdlOutput = "Settings failed to load D:";
				return;
			}

			if (String.IsNullOrEmpty(App.Settings.YtdlLocation))
			{
				// prompt user to specify a youtube-dl location
				YtdlOutput = "Please specify a Youtube-dl Location in Settings!";
				NotifySettingsError();
				return;
			}

			if (String.IsNullOrEmpty(App.Settings.DownloadLocation))
			{
				// prompt user to specify a download location

				// maybe a popup that allows them to proceed and default
				// to saving in the youtube-dl folder?
				YtdlOutput = "Please specify a Download Location in Settings!";
				NotifySettingsError();
				return;
			}

			if (String.IsNullOrEmpty(downloadURL))
			{
				YtdlOutput = "Please enter a URL to download!";
				URLWarning?.Invoke(this, null);
				return;
			}

			YtdlOutput = "began downloading " + downloadURL + "...";

			DownloadPct = 0;
			DlInProgress = true;
			DlEnabled = false;

			string ytdlPath = App.Settings.YtdlLocation;
			ytdlPath += @"\youtube-dl.exe";
			string downloadPath = App.Settings.DownloadLocation;
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

			//Process ytdlProcess = ProcessRunner.CreateProcess(ytdlPath, processArguments);
			dlProcess = ProcessRunner.CreateProcess(ytdlPath, processArguments);

			//ytdlProcess.OutputDataReceived += OutputReceived;
			//ytdlProcess.Exited += YtdlProcess_Exited;

			//ytdlProcess.Start();
			//ytdlProcess.BeginOutputReadLine();

			dlProcess.OutputDataReceived += OutputReceived;
			dlProcess.ErrorDataReceived += ErrorReceived;
			dlProcess.Exited += YtdlProcess_Exited;
			
			try
			{
				dlProcess.Start();
				dlProcess.BeginOutputReadLine();
				dlProcess.BeginErrorReadLine();
			}
			catch(Exception e)
			{
				YtdlOutput = e.Message;
			}

			// example cmd line usage:
			// youtube-dl -f 137 -o "your\download\location\%(title)s.%(ext)s" "https://www.youtube.com/watch?v=5RsOkhR6KBc"
		}

		private void YtdlProcess_Exited(object sender, EventArgs e)
		{
			// ytdl process successfully exited
			var senderProc = sender as Process;
			if (senderProc.ExitCode == 0)
			{
				// process finished successfully!
			}
			else if (senderProc.ExitCode == -1)
			{
				// process was canceled manually
				DownloadPct = 0;
			}
			dlProcess.Dispose();
			DlInProgress = false;
			DlEnabled = true;
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
			ytdlProcess.ErrorDataReceived += ErrorReceived;
			ytdlProcess.Start();

			return tcs.Task;
		}

		private void CancelDownload()
		{
			if (dlProcess != null)
			{
				YtdlOutput = "***Download canceled***" + "\n" + YtdlOutput;
				dlProcess.Kill();
				//dlProcess.Dispose();
			}
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
				if (e.Data.StartsWith("[download]"))
				{
					//string percentage = Regex.Match(e.Data, @"(\d+)%").Value;
					string percentage = Regex.Match(e.Data, @"(\d+(\.\d+))%").Value;
					percentage = percentage.Replace("%", "");
					if (!string.IsNullOrEmpty(percentage))
					{
						float.TryParse(percentage, out float percent);
						if (percent > DownloadPct)
						{
							DownloadPct = percent;
						}
					}
					else
					{
						YtdlOutput = e.Data + "\n" + YtdlOutput;
					}
				}
				else
				{
					YtdlOutput = e.Data + "\n" + YtdlOutput;
				}
			}
		}

		private void ErrorReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null && !String.IsNullOrEmpty(e.Data))
			{
				YtdlOutput = e.Data + "\n" + YtdlOutput;
			}
		}

		// Properties

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

		private float downloadPct = 0;
		public float DownloadPct
		{
			get => downloadPct;
			set
			{
				if (value != downloadPct)
				{
					downloadPct = value;
					RaisePropertyChanged("DownloadPct");
				}
			}
		}

		private bool dlInProgress = false;
		public bool DlInProgress
		{
			get => dlInProgress;
			set
			{
				if (dlInProgress != value)
				{
					dlInProgress = value;
					RaisePropertyChanged("DlInProgress");
				}
			}
		}

		private bool dlEnabled = true;
		public bool DlEnabled
		{
			get => dlEnabled;
			set
			{
				if (dlEnabled != value)
				{
					dlEnabled = value;
					RaisePropertyChanged("DlEnabled");
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

	}
}