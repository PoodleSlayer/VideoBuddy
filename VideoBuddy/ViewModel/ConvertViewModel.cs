using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoBuddy.IoC;
using VideoBuddy.Models;
using GalaSoft.MvvmLight.Command;
using Autofac;
using VideoBuddy.Utility;
using System.Text.RegularExpressions;

namespace VideoBuddy.ViewModel
{
	public class ConvertViewModel :VBViewModel
	{
		public RelayCommand ConvertCommand { get; set; }
		public RelayCommand CancelCommand { get; set; }

		private IFileService fileHelper;
		private SettingsModel settings;
		private Process convertProcess;
		private string ffmpegPath;
		private long fileSize = 0;
		private double fileSizeKB = 0;
		private double progress = 0;
		//private int outputCount = 0;
		//private int outputSize = 0;

		public ConvertViewModel()
		{
			fileHelper = AppContainer.Container.Resolve<IFileService>();
			settings = fileHelper.LoadSettings();
			ffmpegPath = settings.FfmpegLocation + "\\ffmpeg.exe";
		}

		public override void SetupViewModel()
		{
			SetupCommands();
		}

		private void SetupCommands()
		{
			ConvertCommand = new RelayCommand(async() =>
			{
				await ConvertAsync();
			});
			CancelCommand = new RelayCommand(() =>
			{

			});
		}

		private async Task ConvertAsync()
		{
			string destinationPath = Path.ChangeExtension(ConvertPath, ".mp4");

			if (File.Exists(destinationPath))
			{
				// file already exists, warn the user somehow and bail from converting
				return;
			}

			string processArgs = $"-i \"{ConvertPath}\" -codec copy \"{destinationPath}\"";
			convertProcess = ProcessRunner.CreateProcess(ffmpegPath, processArgs);

			ConvertPct = 0;
			ConvertInProgress = true;
			ConvertEnabled = false;

			fileSize = new FileInfo(ConvertPath).Length;
			fileSizeKB = fileSize / 1024;
			DebugSize = fileSizeKB.ToString();

			convertProcess.OutputDataReceived += ConvertProcess_OutputDataReceived;
			convertProcess.ErrorDataReceived += ConvertProcess_ErrorDataReceived;
			convertProcess.Exited += ConvertProcess_Exited;

			try
			{
				convertProcess.Start();
				convertProcess.BeginOutputReadLine();
				convertProcess.BeginErrorReadLine();
				PollForUpdate();
			}
			catch (Exception e)
			{
				// how to log these...
				;
			}
		}

		private void ConvertProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null && !String.IsNullOrEmpty(e.Data))
			{
				// evidently ffmpeg doesn't even use stdout, just the error output stream
				;
			}
		}

		private void ConvertProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			// looks like ffmpeg's output all goes to the error output stream?
			if (e.Data != null && !String.IsNullOrEmpty(e.Data))
			{
				if (e.Data.StartsWith("frame="))
				{
					//outputCount++;
					string convertedSize = Regex.Match(e.Data, @"\b[a-zA-Z]?size= *[0-9]*kB\b").Value;
					var convertSplit = convertedSize.Split('=');
					if (convertSplit.Length < 2)
					{
						return;
					}
					convertedSize = convertSplit[1];
					convertedSize = convertedSize.Trim();
					convertedSize = convertedSize.Replace("kB", "");
					if (!String.IsNullOrEmpty(convertedSize))
					{
						//outputSize++;
						float.TryParse(convertedSize, out float cSize);
						progress = (cSize / fileSizeKB) * 100;
					}
				}
				else if (e.Data.StartsWith("File exists"))
				{
					// file already exists, just exit the process
					;
				}
			}
		}

		private void ConvertProcess_Exited(object sender, EventArgs e)
		{
			// ytdl process successfully exited
			var senderProc = sender as Process;
			if (senderProc.ExitCode == 0)
			{
				// process finished successfully!
				ConvertPct = 100;
			}
			else if (senderProc.ExitCode == -1)
			{
				// process was canceled manually
				ConvertPct = 0;
			}
			convertProcess.Dispose();
			ConvertInProgress = false;
			ConvertEnabled = true;
		}

		private async void PollForUpdate()
		{
			while (ConvertInProgress)
			{
				DebugConverted = progress.ToString();
				ConvertPct = (float)progress;
				await Task.Delay(500);
			}
			;
			//outputCount = 0;
			//outputSize = 0;
		}

		// Properties

		private string convertPath;
		public string ConvertPath
		{
			get => convertPath;
			set
			{
				convertPath = value;
				RaisePropertyChanged("ConvertPath");
			}
		}

		private bool convertEnabled = true;
		public bool ConvertEnabled
		{
			get => convertEnabled;
			set
			{
				if (convertEnabled != value)
				{
					convertEnabled = value;
					RaisePropertyChanged("ConvertEnabled");
				}
			}
		}

		private float convertPct = 0;
		public float ConvertPct
		{
			get => convertPct;
			set
			{
				convertPct = value;
				RaisePropertyChanged("ConvertPct");
			}
		}

		private bool convertInProgress = false;
		public bool ConvertInProgress
		{
			get => convertInProgress;
			set
			{
				if (convertInProgress != value)
				{
					convertInProgress = value;
					RaisePropertyChanged("ConvertInProgress");
				}
			}
		}

		private string debugConverted;
		public string DebugConverted
		{
			get => debugConverted;
			set
			{
				debugConverted = value;
				RaisePropertyChanged("DebugConverted");
			}
		}

		private string debugSize;
		public string DebugSize
		{
			get => debugSize;
			set
			{
				debugSize = value;
				RaisePropertyChanged("DebugSize");
			}
		}

		private string debugPct;
		public string DebugPct
		{
			get => debugPct;
			set
			{
				debugPct = value;
				RaisePropertyChanged("DebugPct");
			}
		}

	}
}
