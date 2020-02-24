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
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();
			DataContext = AppContainer.Container.Resolve<MainViewModel>();

			Loaded += MainPage_Loaded;

			SettingsBtn.Click += SettingsBtn_Click;
		}

		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			ViewModel.PageLoaded();
		}

		private void SettingsBtn_Click(object sender, RoutedEventArgs e)
		{
			//NavigationService.Navigate(AppContainer.Container.Resolve<SettingsPage>());
			MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
			mainWindow.MainDisplay.Content = AppContainer.Container.Resolve<SettingsPage>();
		}

		private MainViewModel ViewModel
		{
			get => DataContext as MainViewModel;
		}
	}
}
