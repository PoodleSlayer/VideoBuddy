using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoBuddy.IoC;
using VideoBuddy.ViewModel;

namespace VideoBuddy.Views
{
	/// <summary>
	/// Interaction logic for MainPage.xaml
	/// </summary>
	public partial class DownloadPage : UserControl
	{
		public DownloadPage()
		{
			InitializeComponent();
			DataContext = AppContainer.Container.Resolve<DownloadViewModel>();

			Loaded += DownloadPage_Loaded;
			Unloaded += DownloadPage_Unloaded;

			ViewModel.SettingsWarning += ViewModel_SettingsWarning;
			ViewModel.URLWarning += ViewModel_URLWarning;
		}

		// move this to MainWindow and use messaging
		private void ViewModel_SettingsWarning(object sender, EventArgs e)
		{
			// highlight the Settings button so the user knows what to fill out
			ColorAnimation animation;
			animation = new ColorAnimation();
			animation.From = Colors.Green;
			animation.To = (Color)ColorConverter.ConvertFromString("#239CFF");
			animation.Duration = new Duration(TimeSpan.FromSeconds(1));
			//SettingsBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#239CFF"));
			//SettingsBtn.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		}

		private void ViewModel_URLWarning(object sender, EventArgs e)
		{
			// highlight the URL field so the user knows what to click
			ColorAnimation animation;
			animation = new ColorAnimation();
			animation.From = Colors.Green;
			animation.To = Colors.White;
			animation.Duration = new Duration(TimeSpan.FromSeconds(1));
			URLBox.Background = new SolidColorBrush(Colors.White);
			URLBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		}

		// called first time the page is created
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		// called each time we navigate to this page
		private void DownloadPage_Loaded(object sender, RoutedEventArgs e)
		{
			ViewModel.PageLoaded();
		}

		// called each time we leave this page
		private void DownloadPage_Unloaded(object sender, RoutedEventArgs e)
		{
			// do some stuff
		}

		private DownloadViewModel ViewModel
		{
			get => DataContext as DownloadViewModel;
		}
	}
}
