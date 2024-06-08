using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UserInterfaces
{
	/// <summary>
	/// The CommandType class is used to assign dynamic commands to user interface.
	/// </summary>
	public class CommandType
	{
		/// <summary>
		/// This text is the label to display.
		/// </summary>
		public String Label { get; set; }
		/// <summary>
		/// This text is used to display tooltip information.
		/// </summary>
		public String ToolTip { get; set; }

		private String imagepath;
		/// <summary>
		/// This is the path to a icon.
		/// </summary>
		public String ImagePath { 
			get
			{
				return imagepath;
			}
			set
			{
				imagepath = value;
			} 
		}
		/// <summary>
		/// This visibility property is used to select whether the labels is visible or not.
		/// </summary>
		public System.Windows.Visibility IsLabelVisible
		{
			get
			{
				return String.IsNullOrEmpty(Label) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
			}
		}
		/// <summary>
		/// This property is used to select whether the icon is visible or not.
		/// </summary>
		public bool IsIconVisible { get { return !String.IsNullOrEmpty(ImagePath); } }
		private ICommand m_command = null;
		/// <summary>
		/// This is the primary command. This is executed on Click event.
		/// </summary>
		public ICommand Command
		{
			get
			{

				if (null == m_command)
				{
					m_command = new RelayCommand(p => { }); // NOP
				}
				return m_command;
			}
		}
		private ICommand m_shift_command = null;
		/// <summary>
		/// This is the primary command. This is executed on Shift-Click event.
		/// </summary>
		public ICommand ShiftCommand
		{
			get
			{

				if (null == m_shift_command)
				{
					m_shift_command = new RelayCommand(p => { }); // NOP
				}
				return m_shift_command;
			}
		}

		/// <summary>
		/// This is a constructor of the CommandType class. See parameter list for details.
		/// </summary>
		/// <param name="label">This text is the label to display.</param>
		/// <param name="tooltip">This text is used to display tooltip information.</param>
		/// <param name="imagepath">This is the path to a icon.</param>
		/// <param name="execute">This is the primary command. This is executed on Click event.</param>
		/// <param name="canExecute"></param>
		public CommandType(string label, string tooltip, string imagepath, Action<object> execute, Predicate<object> canExecute)
		{
			this.Label = label;
			this.ToolTip = tooltip;
			this.ImagePath = imagepath;
			this.m_command = new RelayCommand(execute, canExecute);
		}

		/// <summary>
		/// This is a constructor of the CommandType class. See parameter list for details.
		/// </summary>
		/// <param name="label">This text is the label to display.</param>
		/// <param name="tooltip">This text is used to display tooltip information.</param>
		/// <param name="imagepath">This is the path to a icon.</param>
		/// <param name="execute">This is the primary command. This is executed on Click event.</param>
		public CommandType(string label, string tooltip, string imagepath, Action<object> execute)
		{
			this.Label = label;
			this.ToolTip = tooltip;
			this.ImagePath = imagepath;
			this.m_command = new RelayCommand(execute);
		}

		/// <summary>
		/// This is a constructor of the CommandType class. See parameter list for details.
		/// </summary>
		/// <param name="label">This text is the label to display.</param>
		/// <param name="tooltip">This text is used to display tooltip information.</param>
		/// <param name="imagepath">This is the path to a icon.</param>
		/// <param name="execute">This is the primary command. This is executed on Click event.</param>
		/// <param name="shift_execute">This is the primary command. This is executed on Shift-Click event.</param>
		public CommandType(string label, string tooltip, string imagepath, Action<object> execute, Action<object> shift_execute)
		{
			this.Label = label;
			this.ToolTip = tooltip;
			this.ImagePath = imagepath;
			this.m_command = new RelayCommand(execute);
			this.m_shift_command = new RelayCommand(shift_execute);
		}

	}
}
