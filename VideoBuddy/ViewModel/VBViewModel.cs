using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoBuddy.ViewModel
{
	public abstract class VBViewModel : ViewModelBase
	{
		public VBViewModel()
		{
			SetupViewModel();
		}

		public abstract void SetupViewModel();
	}
}
