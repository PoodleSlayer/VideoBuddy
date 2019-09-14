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
	public partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();
			DataContext = AppContainer.Container.Resolve<MainViewModel>();

			SettingsBtn.Click += SettingsBtn_Click;
		}

		private void SettingsBtn_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(AppContainer.Container.Resolve<SettingsPage>());
		}
	}
}
