using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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


namespace UserInterfaces
{
	/// <summary>
	/// Interaction logic for ConfigList.xaml
	/// </summary>
	public partial class ConfigEditor : UserControl
	{
		/// <summary>
		/// This is the constructor of the ConfigEditor class.
		/// </summary>
		public ConfigEditor()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Use this method to define the data context for the control.
		/// </summary>
		/// <param name="o">The list of the classes to display.</param>
		public void SetDC(object o)
		{
			MainPanel.DataContext = o;
		}

		private void ListViewKeyDown(object sender, KeyEventArgs ev)
		{
			//var tb = textBox as TextBox; ;
			var lv = sender as ListView;
			try
			{
				if (ev.Key == Key.Enter)
				{
					if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
					{
						//UpdateItem(textBox, listBox);
					}
					else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
					{
						//AddItem(textBox, listBox, 0);
					}
					else
					{
						//AddItem(textBox, listBox, 1);

					}
				}
				if (ev.Key == Key.Escape)
				{
					lv.SelectedIndex = -1;
					//lv
				}

			}
			catch (Exception e)
			{
				// TODO:
				//App.CollectExceptions(new Exception(String.Format("Unable to process the command DoubleClick! textbox: {0}; index: {1}", tb.Text, lb.SelectedIndex), e));
			}


		}

	}
}
