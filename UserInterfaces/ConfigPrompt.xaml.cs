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
using System.Windows.Shapes;

namespace UserInterfaces
{
	/// <summary>
	/// Interaction logic for ConfigPrompt.xaml
	/// </summary>
	public partial class ConfigPrompt : Window
	{
		/// <summary>
		/// This is the constructor of the ConfigPrompt class
		/// </summary>
		public ConfigPrompt()
		{
			InitializeComponent();
			ConfigEditor.SetDC(DisplayClass.Assign( new List<object>()
			{
				//new SignalDescriptorBase()
			}));


		}

		/// <summary>
		/// This is the constructor of the ConfigPrompt class
		/// </summary>
		public ConfigPrompt(object o)
		{
			InitializeComponent();
			ConfigEditor.SetDC(DisplayClass.Assign(o));

		}

		private void cmdOk(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void cmdCancel(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{

			// TODO: To check if profile was modified! We have to ask only if the profile have been modified!
			if (null == this.DialogResult)
			{
				var result = MessageBox.Show(this, "Are you sure you want to cancel?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					base.OnClosing(e);
					//App.ExitLog();
				}
				else
				{
					e.Cancel = true;

				}

			}




		}
	}
}
