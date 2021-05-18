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
using Autofac;
using VideoBuddy.Controls;

namespace VideoBuddy.Views
{
	/// <summary>
	/// Interaction logic for ConvertPage.xaml
	/// </summary>
	public partial class ConvertPage : UserControl
	{
		public ConvertPage()
		{
			InitializeComponent();
			DataContext = AppContainer.Container.Resolve<ConvertViewModel>();

			PathBox.PreviewDragOver += PathBox_PreviewDragOver;
			PathBox.Drop += PathBox_Drop;
		}

		private void PathBox_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (!ViewModel.ConvertInProgress)
			{
				e.Handled = true;
			}
		}

		private void PathBox_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
				if (files != null && files.Length > 0)
				{
					//((VBSelectTextBox)sender).Text = files[0];
					ViewModel.ConvertPath = files[0];
				}
			}
		}

		private ConvertViewModel ViewModel
		{
			get => DataContext as ConvertViewModel;
		}
	}
}
