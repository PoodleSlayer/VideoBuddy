using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using VideoBuddy.Utility.Messages;

namespace VideoBuddy.ViewModel
{
	public abstract class VBViewModel : ViewModelBase
	{
		public VBViewModel()
		{
			SetupViewModel();
		}

		protected void NotifySettingsError()
		{
			ButtonWarningMessage msg = new ButtonWarningMessage()
			{
				ButtonToHighlight = "Settings"
			};
			Messenger.Default.Send(msg);
		}

		public abstract void SetupViewModel();
	}
}
