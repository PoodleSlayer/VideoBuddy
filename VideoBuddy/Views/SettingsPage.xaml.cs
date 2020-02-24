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
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoBuddy.Controls;
using VideoBuddy.IoC;
using VideoBuddy.ViewModel;

namespace VideoBuddy.Views
{
	/// <summary>
	/// Interaction logic for SettingsPage.xaml
	/// </summary>
	public partial class SettingsPage : UserControl
	{
		public SettingsPage()
		{
			InitializeComponent();
			DataContext = AppContainer.Container.Resolve<SettingsViewModel>();

			DestinationBtn.Click += DestinationBtn_Click;
			YtdlBtn.Click += YtdlBtn_Click;
			BackBtn.Click += BackBtn_Click;
			VBSelectTextBox testy = new VBSelectTextBox();
			Loaded += SettingsPage_Loaded;
		}

		private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
		{
			ViewModel.PageLoaded();
		}

		private void YtdlBtn_Click(object sender, RoutedEventArgs e)
		{
			// should probably find a way to run these from the ViewModel but this is fine for now
			using (var folderBrowser = new WinForms.FolderBrowserDialog())
			{
				WinForms.DialogResult result = folderBrowser.ShowDialog();
				if (result == WinForms.DialogResult.OK && !String.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
				{
					ViewModel.YtdlLocation = folderBrowser.SelectedPath;
				}
			}
		}

		private void DestinationBtn_Click(object sender, RoutedEventArgs e)
		{
			using (var folderBrowser = new WinForms.FolderBrowserDialog())
			{
				WinForms.DialogResult result = folderBrowser.ShowDialog();
				if (result == WinForms.DialogResult.OK && !String.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
				{
					ViewModel.DownloadLocation = folderBrowser.SelectedPath;
				}
			}
		}

		private void BackBtn_Click(object sender, RoutedEventArgs e)
		{
			//NavigationService.GoBack();
			// need to make a custom NavigationService/Stack to keep track of things now 
			// that the app uses UserControls instead of Pages. also ideally the navigation
			// would happen from the ViewModels c: good luck future Chris
			MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
			mainWindow.MainDisplay.Content = AppContainer.Container.Resolve<MainPage>();
		}

		private SettingsViewModel ViewModel
		{
			get => DataContext as SettingsViewModel;
		}
	}
}
