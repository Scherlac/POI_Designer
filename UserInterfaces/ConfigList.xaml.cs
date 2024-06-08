using System;
using System.Collections;
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

namespace UserInterfaces
{
	/// <summary>
	/// Interaction logic for ConfigListEditor.xaml
	/// </summary>
	public partial class ConfigList : UserControl
	{
		public ConfigList()
		{
			InitializeComponent();
			UpdateCmds();
		}


		protected virtual void UpdateCmds()
		{
			var cmd_list = new CommandTypeList()
			{
				new CommandType("", "Add a new item to the list.", "Images/112_Plus_Green_16x16_72.png", (c) => 
				{
					AddItem(1);
				}),
				new CommandType("", "Remove the selected item from the list.", "Images/112_Minus_Orange_16x16_72.png", (c) => 
				{
					RemoveItem();
				}),
				new CommandType("", "Move up the selected item.", "Images/112_UpArrowShort_Blue_16x16_72.png", c => 
				{ 
					MoveItem(-1); 
				}),
				new CommandType("", "Move down the selected item.", "Images/112_DownArrowShort_Blue_16x16_72.png", c => 
				{ 
					MoveItem(+1); 
				}),

				null			
			};
			UserCmds.DataContext = cmd_list;
		}

		private void AddItem(int after)
		{
			//var tb = this.UserText;
			var lb = this.UserList;
			var dc = (IDisplayList)this.DataContext;
			try
			{
				//BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
				//be.UpdateSource();

				dc.Add((after!=0));

				//be = lb.GetBindingExpression(ListView.SelectedIndexProperty);
				//be.UpdateTarget();

				//tb.SelectAll();
				//tb.Focus();

			}
			catch (Exception e)
			{
				//App.CollectExceptions(new Exception(String.Format("Unable to process the command Add! textbox: {0}; index: {1}; after {2}", tb.Text, lb.SelectedIndex, after), e));
			}

		}

		private void UpdateItem()
		{
			//var tb = this.UserText;
			var lb = this.UserList;
			var dc = (IDisplayList)this.DataContext;

			try
			{
				//BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
				//be.UpdateSource();

				//be = lb.GetBindingExpression(ListView.SelectedIndexProperty);
				//be.UpdateSource();

				dc.Update();

				//be.UpdateTarget();

				//tb.SelectAll();
				//tb.Focus();
			}
			catch (Exception e)
			{
				//App.CollectExceptions(new Exception(String.Format("Unable to process the command Update! textbox: {0}; index: {1}", tb.Text, lb.SelectedIndex), e));
			}

		}

		private void RemoveItem()
		{
			//var tb = this.UserText;
			var lb = this.UserList;
			var dc = (IDisplayList)this.DataContext;

			try
			{
				dc.Remove();

				var be = lb.GetBindingExpression(ListView.SelectedIndexProperty);
				be.UpdateTarget();
			}
			catch (Exception e)
			{
				//App.CollectExceptions(new Exception(String.Format("Unable to process the command Remove! textbox: {0}; index: {1}", tb.Text, lb.SelectedIndex), e));
			}

		}

		private void MoveItem(int offset)
		{
			//var tb = this.UserText;
			var lb = this.UserList;
			var dc = (IDisplayList)this.DataContext;

			try
			{
				var be = lb.GetBindingExpression(ListView.SelectedIndexProperty);
				be.UpdateSource();

				dc.Move(offset);

				be.UpdateTarget();

			}
			catch (Exception e)
			{
				//App.CollectExceptions(new Exception(String.Format("Unable to process the command Remove! textbox: {0}; index: {1}", tb.Text, lb.SelectedIndex), e));
			}

		}

		private void UserList_MouseDoubleClick(object sender, MouseButtonEventArgs ev)
		{
			//var tb = this.UserText;
			var lb = this.UserList;
			var dc = (IDisplayList)this.DataContext;
			try
			{
				dc.Select();
			}
			catch (Exception e)
			{
				//App.CollectExceptions(new Exception(String.Format("Unable to process the command DoubleClick! textbox: {0}; index: {1}", tb.Text, lb.SelectedIndex), e));
			}


		}

		/// <summary>
		/// Mouse click event for the given control item.
		/// </summary>
		/// <param name="sender">Reference to the control</param>
		/// <param name="ev">Provides data for mouse button related events.</param>
		//private void listBoxTEMP_MouseDoubleClick(object sender, MouseButtonEventArgs ev)
		//{
		//	ListBoxMouseDoubleClick(this.textBoxTEMP, sender);
		//}

		private void UserList_KeyDown(object sender, KeyEventArgs ev)
		{
			//var tb = this.UserText;
			var lb = this.UserList;
			var dc = (IDisplayList)this.DataContext;

			try
			{
				if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
				{
					if (ev.Key == Key.Up)
					{
						if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
						{
							if (lb.SelectedIndex > 0)
							{
								MoveItem(-1);
							}
						}

					}
					else if (ev.Key == Key.Down)
					{
						if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
						{
							if ((lb.SelectedIndex + 1) < lb.Items.Count)
							{
								MoveItem(+1);
							}

						}

					}
				}
				else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				{
				}
				else
				{
					if (ev.Key == Key.Enter)
					{
						dc.Select();
						//tb.SelectAll();
						//tb.Focus();

					}
					else if (ev.Key == Key.Escape)
					{
						//tb.SelectAll();
						//tb.Focus();

					}
					else if (ev.Key == Key.Delete)
					{
						RemoveItem();

						ListViewItem item = lb.ItemContainerGenerator.ContainerFromIndex(lb.SelectedIndex) as ListViewItem;
						Keyboard.Focus(item);

					}
				}
			}
			catch (Exception e)
			{
				// TODO:
				//App.CollectExceptions(new Exception(String.Format("Unable to process the command DoubleClick! textbox: {0}; index: {1}", tb.Text, lb.SelectedIndex), e));
			}
		}


		private void UserText_KeyDown(object sender, KeyEventArgs ev)
		{
			//var tb = this.UserText;
			var lb = this.UserList;
			var dc = (IDisplayList)this.DataContext;

			try
			{
				if (ev.Key == Key.Enter)
				{
					if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
					{
						UpdateItem();
					}
					else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
					{
						AddItem(0);
					}
					else
					{
						AddItem(1);

					}
				}
				else if (ev.Key == Key.Escape)
				{
					//BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
					//be.UpdateTarget();
					//tb.SelectAll();

				}
				else if (ev.Key == Key.PageDown)
				{
					if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
					{
						ListViewItem item = lb.ItemContainerGenerator.ContainerFromIndex(lb.SelectedIndex) as ListViewItem;
						Keyboard.Focus(item);
						//item.Focus();
						//lb.Focus();
					}

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
