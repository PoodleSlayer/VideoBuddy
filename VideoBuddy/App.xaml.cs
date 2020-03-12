using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VideoBuddy.IoC;
using VideoBuddy.Models;

namespace VideoBuddy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static SettingsModel Settings = new SettingsModel();

		public App()
		{
			AppContainer.Start();
		}

		protected override void OnActivated(EventArgs e)
		{
			// just in case
			base.OnActivated(e);
		}

		protected override void OnDeactivated(EventArgs e)
		{
			// just in case
			base.OnDeactivated(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			AppContainer.Stop();
		}
	}
}
