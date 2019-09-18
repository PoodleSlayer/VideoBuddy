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

// NOTE --------------------------------------------------------------------
// This class is based on code found here https://stackoverflow.com/a/661224
// which I have seen used across various websites, so give them credit!

namespace VideoBuddy.Controls
{
	/// <summary>
	/// Custom TextBox that selects all text when it takes focus.
	/// </summary>
	public class VBSelectTextBox : TextBox
	{
		public VBSelectTextBox()
		{
			AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(TakeFocus), true);
			AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);
			AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText), true);
		}

		private static void TakeFocus(object sender, MouseButtonEventArgs e)
		{
			DependencyObject parent = e.OriginalSource as UIElement;
			while (parent != null && !(parent is TextBox))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}

			if (parent != null)
			{
				var textBox = (TextBox)parent;
				if (!textBox.IsKeyboardFocusWithin)
				{
					textBox.Focus();
					e.Handled = true;
				}
			}
		}

		private static void SelectAllText(object sender, RoutedEventArgs e)
		{
			var textBox = e.OriginalSource as TextBox;
			if (textBox != null)
			{
				textBox.SelectAll();
			}
		}

		// what is this for?
		//static VBSelectTextBox()
		//{
		//	DefaultStyleKeyProperty.OverrideMetadata(typeof(VBSelectTextBox), new FrameworkPropertyMetadata(typeof(VBSelectTextBox)));
		//}
	}
}
