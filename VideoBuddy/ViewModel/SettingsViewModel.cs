using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoBuddy.ViewModel
{
	public class SettingsViewModel : VBViewModel
	{
		public RelayCommand GoBackCommand { get; set; }

		public override void SetupViewModel()
		{
			SetupCommands();
		}

		private void SetupCommands()
		{
			GoBackCommand = new RelayCommand(() =>
			{
				// actually maybe just do this in the UI
			});
		}
	}
}
