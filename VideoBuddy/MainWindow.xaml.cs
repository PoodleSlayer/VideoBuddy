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
using VideoBuddy.Views;
using Autofac;
using GalaSoft.MvvmLight.Messaging;

namespace VideoBuddy
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool onDownload = true;
		private bool onRecord = false;
		private bool onConvert = false;
		private bool onSettings = false;

		public MainWindow()
		{
			InitializeComponent();

			//MainFrame.Navigate(AppContainer.Container.Resolve<MainPage>());
			MainDisplay.Content = AppContainer.Container.Resolve<DownloadPage>();
			DownloadBtn.IsEnabled = false;

			DownloadBtn.Click += DownloadBtn_Click;
			RecordBtn.Click += RecordBtn_Click;
			ConvertBtn.Click += ConvertBtn_Click;
			SettingsBtn.Click += SettingsBtn_Click;

			Closing += MainWindow_Closing;

			Messenger.Default.Register<NotificationMessage>(this, ReceiveMessage);
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Messenger.Default.Unregister<NotificationMessage>(this, ReceiveMessage);
		}

		private void SettingsBtn_Click(object sender, RoutedEventArgs e)
		{
			MainDisplay.Content = AppContainer.Container.Resolve<SettingsPage>();
			DownloadBtn.IsEnabled = RecordBtn.IsEnabled = ConvertBtn.IsEnabled = true;
			SettingsBtn.IsEnabled = false;
		}

		private void ConvertBtn_Click(object sender, RoutedEventArgs e)
		{
			MainDisplay.Content = AppContainer.Container.Resolve<ConvertPage>();
			DownloadBtn.IsEnabled = RecordBtn.IsEnabled = SettingsBtn.IsEnabled = true;
			ConvertBtn.IsEnabled = false;
		}

		private void RecordBtn_Click(object sender, RoutedEventArgs e)
		{
			MainDisplay.Content = AppContainer.Container.Resolve<RecordPage>();
			DownloadBtn.IsEnabled = ConvertBtn.IsEnabled = SettingsBtn.IsEnabled = true;
			RecordBtn.IsEnabled = false;
		}

		private void DownloadBtn_Click(object sender, RoutedEventArgs e)
		{
			MainDisplay.Content = AppContainer.Container.Resolve<DownloadPage>();
			RecordBtn.IsEnabled = ConvertBtn.IsEnabled = SettingsBtn.IsEnabled = true;
			DownloadBtn.IsEnabled = false;
		}

		private void ViewModel_SettingsWarning(object sender, EventArgs e)
		{
			// highlight the Settings button so the user knows what to fill out
			ColorAnimation animation;
			animation = new ColorAnimation();
			animation.From = Colors.Green;
			animation.To = (Color)ColorConverter.ConvertFromString("#239CFF");
			animation.Duration = new Duration(TimeSpan.FromSeconds(1));
			SettingsBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#239CFF"));
			SettingsBtn.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		}

		private async void ReceiveMessage(NotificationMessage msg)
		{

		}

		public void NavigateTo(string newPage)
		{

		}

		
	}
}
