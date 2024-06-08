using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.XPath;

namespace UserInterfaces
{
	/// <summary>
	/// This is the data template selector for the user interface.
	/// </summary>
	public class ConfigSelector : DataTemplateSelector
	{
		/// <summary>
		/// This is an override of the method of the base class, it returns a DataTemplate based on custom logic. 
		/// </summary>
		/// <param name="item">The data object for which to select the template.</param>
		/// <param name="container">The data-bound object.</param>
		/// <returns>Returns a DataTemplate based on custom logic.</returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var sel = base.SelectTemplate(item, container);

			FrameworkElement element = container as FrameworkElement;

			if (null == item)
			{
				return element.FindResource("displayNULL") as DataTemplate;
			}
			var it = item.GetType();

			if (typeof(IDisplayList).IsAssignableFrom(item.GetType()))
			{
				if (typeof(DisplayObjectList).IsAssignableFrom(it))
				{
					//var dt = element.FindResource("displayObjectList") as DataTemplate;
					//return dt;
				}
				else
				{
					var dt = element.FindResource("displayValueList") as DataTemplate;
					return dt;
				}
			}
			else if (typeof(IDisplayObject).IsAssignableFrom(item.GetType()))
			{
				if (typeof(DisplayBool).IsAssignableFrom(it))
				{
					//var dt = element.FindResource("displayBool") as DataTemplate;
					//return dt;
				}
				else if (typeof(DisplayEnum).IsAssignableFrom(it))
				{
					//var dt = element.FindResource("displayEnum") as DataTemplate;
					//return dt;
				}
				else if (typeof(DisplayClass).IsAssignableFrom(it))
				{
					//var dt = element.FindResource("displayClass") as DataTemplate;
					//return dt;
				}
				else
				{
					var dt = element.FindResource("displayValue") as DataTemplate;
					return dt;
				}
			}

			return sel;

		}

	}

	/// <summary>
	/// This class defines the Display attribute used by the user interface.  
	/// </summary>
	[
		AttributeUsage(AttributeTargets.Class | AttributeTargets.Property,
			   AllowMultiple = false,
			   Inherited = false)
	]
	public class DisplayAttribute : Attribute
	{
		/// <summary>
		/// This is the default constructor. 
		/// </summary>
		/// <param name="Label"></param>
		/// <param name="ToopTip"></param>
		/// <param name="Width"></param>
		/// <param name="VPName">This is the name of the property used to control the visibility. (Visibility property name)</param>
		public DisplayAttribute(string Label, string ToopTip, int Width, string VPName = null)
		{
			this.Label = Label;
			this.ToolTip = ToopTip;
			this.Width = Width;
			this.VPName = VPName;
			this.VLName = null;
			this.IsReadonly = false;
		}

		/// <summary>
		/// This is the default constructor. 
		/// </summary>
		/// <param name="Label"></param>
		/// <param name="ToopTip"></param>
		/// <param name="Width"></param>
		/// <param name="VPName">This is the name of the property used to control the visibility. (Visibility property name)</param>
		/// <param name="VLName">This is the name of the property contains the validation list</param>
		/// <param name="IsReadonly"></param>
		public DisplayAttribute(string Label, string ToopTip, int Width, bool IsReadonly, string VPName = null, string VLName = null)
		{
			this.Label = Label;
			this.ToolTip = ToopTip;
			this.Width = Width;
			this.VPName = VPName;
			this.VLName = VLName;
			this.IsReadonly = IsReadonly;
		}

		public string Label { get; set; }
		public string ToolTip { get; set; }
		public int Width { get; set; }
		public string VPName { get; set; }
		public string VLName { get; set; }
		public bool IsReadonly { get; set; }

	}

	/// <summary>
	/// This is the configuration interface.
	/// </summary>
	public interface IConfiguration
	{
		UserInterfaces.CommandTypeList GetCommands(string propertyName);
	}

	/// <summary>
	/// Configurable
	/// </summary>
	interface IConfigurable
	{
		void Configure(object conf);
		object GetConfig();
		void Load(System.Xml.XPath.XPathNavigator nav, string configID = null, string itemID = null);
		void PromptConfig();
		void Save(System.Xml.XPath.XPathNavigator nav, string configID = null, string itemID = null);
	}


	public class ConfigurableBase<TCONFIG> : IConfigurable
		where TCONFIG : class, new()
	{
		protected TCONFIG config;

		public static Type GetConfigType()
		{
			return typeof(TCONFIG);
		}

		public void Load(XPathNavigator nav, string configID = null, string itemID = null)
		{
			config = Common.Load<TCONFIG>(nav, configID, itemID);

		}

		public void Save(XPathNavigator nav, string configID = null, string itemID = null)
		{
			Common.Save(nav, config, configID, itemID);
		}

		public void Configure(object conf)
		{
			if (typeof(TCONFIG).IsAssignableFrom(conf.GetType()))
			{
				config = (TCONFIG)conf;
			}
		}

		public object GetConfig()
		{
			return config;
		}

		public void PromptConfig()
		{
			TCONFIG conf = null;

			if (null == config)
			{
				config = new TCONFIG();
			}

			// FIXME
			// conf = Common.Copy(config);
			conf = config;

			if (Common.ShowConfigPromptDialog(conf))
			{
				config = conf;
			}
		}
	}

	public interface IDisplayObject //<T>
	{
		string Name { set; get; }
		string Category { set; get; }
		string Label { set; get; }
		string ToolTip { set; get; }
		int Width { set; get; }
		//object Value { set; get; }
		Visibility Visibility { get; }
		bool IsEnabled { get; }
		CommandTypeList Commands { set; get; }

	}

	public class DisplayObjectBase<T> : NotifyPropertyChangedBase, IDisplayObject
	{

		public DisplayObjectBase(object o)
		{
			//pInfo = null;
			obj = o;

			if (typeof(INotifyPropertyChanged).IsAssignableFrom(obj.GetType()))
			{
				var oo = obj as INotifyPropertyChanged;
				oo.PropertyChanged += RelayPropertyChanged;
			}

			Name = obj.GetType().Name;

			var dia = obj.GetType().GetCustomAttribute<DisplayAttribute>();
			if (null != dia)
			{
				Label = dia.Label + ":";
				ToolTip = dia.ToolTip;
				Width = dia.Width;

				VPName = dia.VPName;
				IsEnabled = !dia.IsReadonly;

			}
			else
			{
				Label = Name + ":";

				var da = obj.GetType().GetCustomAttribute<DescriptionAttribute>();
				ToolTip = (null != da) ? da.Description : null;

				Width = 120;
				IsEnabled = true;
			}

		}

		public string Category { set; get; }
		public string Label { set; get; }
		public string Name { set; get; }
		public string ToolTip { set; get; }
		public int Width { set; get; }
		public CommandTypeList Commands { set; get; }
		public string VPName { set; get; }
		protected Type Controll;

		//private bool m_IsEnabled;
		//public bool IsEnabled { set { SetField(ref m_IsEnabled, value); } get { return m_IsEnabled; } }
		public bool IsEnabled { set; get; }

		protected virtual void RelayPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		protected override bool NewSubscriber(Type t)
		{
			if (typeof(UIElement).IsAssignableFrom(t))
			{
				Controll = t;
			}
			return base.NewSubscriber(t);
		}
		public T Value
		{
			set
			{
			}
			get
			{
				return default(T);
			}
		}
		public virtual Visibility Visibility
		{
			get
			{
				return Visibility.Visible;
			}
		}

		protected object obj;

	}

	public class DisplayPropertyBase<T> : DisplayObjectBase<T>
	{
		public DisplayPropertyBase(object o)
			: base(o)
		{

		}

		public DisplayPropertyBase(PropertyInfo pi, object o) :
			base(o)
		{
			pInfo = pi;

			Name = pInfo.Name;

			var ca = pInfo.GetCustomAttribute<CategoryAttribute>();
			Category = (null != ca) ? ca.Category : "Default";

			var dia = pInfo.GetCustomAttribute<DisplayAttribute>();
			if (null != dia)
			{
				Label = dia.Label + ":";
				ToolTip = dia.ToolTip;
				Width = dia.Width;

				VPName = dia.VPName;

				if (null != dia.VPName)
				{
					pInfoVP = o.GetType().GetProperty(dia.VPName);
				}

				this.IsEnabled = !dia.IsReadonly;
			}
			else
			{
				Label = Name + ":";

				var da = pInfo.GetCustomAttribute<DescriptionAttribute>();
				ToolTip = (null != da) ? da.Description : null;

				Width = 120;

				this.IsEnabled = null != pInfo.GetSetMethod() ||
					//typeof(IEnumerable).IsAssignableFrom(pInfo.PropertyType) ||
					typeof(INotifyPropertyChanged).IsAssignableFrom(pInfo.PropertyType) ||
					false;
			}

			if (typeof(IConfiguration).IsAssignableFrom(this.obj.GetType()))
			{
				var co = this.obj as IConfiguration;
				this.Commands = co.GetCommands(this.Name);
			}

			this.GenericParameterType = Common.GetGenericParameter(pInfo.PropertyType);



		}

		protected virtual void RelayCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method is an event relay for the hosted property.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event to relay over.</param>
		protected override void RelayPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Name)
			{
				OnPropertyChanged("Value");
			}
			else if (e.PropertyName == VPName)
			{
				OnPropertyChanged("Visibility");
			}
		}

		/// <summary>
		/// This is the main property. The gui access the hosted property over this property.
		/// </summary>
		public new T Value
		{
			set
			{
				if (null != pInfo)
				{
					pInfo.SetValue(obj, value);
				}
			}
			get
			{
				if (null != pInfo)
				{
					return (T)pInfo.GetValue(obj);
				}
				else
				{
					return base.Value;
				}
			}
		}

		/// <summary>
		/// The visibility of the control.
		/// </summary>
		public override Visibility Visibility
		{
			get
			{
				if (null != pInfoVP)
				{
					return (bool)pInfoVP.GetValue(obj) ? Visibility.Visible : Visibility.Collapsed;
				}
				else
				{
					return Visibility.Visible;
				}
			}
		}

		/// <summary>
		/// The type of the first generic parameter. Eg. string for List&lt;string&gt;
		/// </summary>
		public Type GenericParameterType { set; get; }

		protected PropertyInfo pInfoVP;
		protected PropertyInfo pInfo;

	}

	public class DisplayString : DisplayPropertyBase<string>
	{
		public DisplayString(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o)
		{

		}
	}

	public class DisplayDouble : DisplayPropertyBase<double>
	{
		public DisplayDouble(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o)
		{
		}
	}

	public class DisplayInt : DisplayPropertyBase<int>
	{
		public DisplayInt(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o)
		{
		}
	}

	public class DisplayUInt : DisplayPropertyBase<uint>
	{
		public DisplayUInt(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o)
		{
		}
	}

	public class DisplayBool : DisplayPropertyBase<bool>
	{
		public DisplayBool(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o)
		{
		}
	}

	public class DisplayEnum : DisplayPropertyBase<object>
	{
		public DisplayEnum(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o)
		{
		}
		public Array Items
		{
			get
			{
				return Enum.GetValues(pInfo.PropertyType);
			}
		}
	}

	public class CommandTypeList : List<CommandType>
	{
	}

	public interface IDisplayList
	{
		void Add(bool after);
		void Remove();
		void Move(int offset);
		int Count { get; }

		void Select();
		void Update();
	}

	public class DisplayListBase<T> : DisplayPropertyBase<List<T>>, IDisplayList //where T:class
	{
		public DisplayListBase(object o)
			: base(o)
		{
			UpdateCmds();
			UpdateList();
		}

		public DisplayListBase(PropertyInfo pi, object o)
			: base(pi, o)
		{

			if (typeof(INotifyCollectionChanged).IsAssignableFrom(this.pInfo.PropertyType))
			{
				try
				{
					var v = this.Value;
					INotifyCollectionChanged vv = (dynamic)v;
					vv.CollectionChanged += RelayCollectionChanged;
				}
				catch (Exception e)
				{
					ErrorReport.CollectExceptions(e);
				}


			}

			UpdateCmds();
			UpdateList();
		}

		protected virtual void UpdateCmds()
		{
			cmd_list = new CommandTypeList()
			{
				new CommandType("Add", "Add a new item to the list.", "Images/112_Plus_Green_16x16_72.png", (c) => 
				{
					Add(true);
				}, (c) =>
				{
					Add(false);
				}),
				new CommandType("Remove", "Remove the selected item from the list.", "Images/112_Minus_Orange_16x16_72.png", (c) => 
				{
					Remove();
				}),
				null			
			};
		}

		protected virtual void UpdateList()
		{

			// This is ok for string int uint double bool
			//class_list = (List<object>)Value;

			// This is ok for INotifyPropertyChanged
			var v = Value;
			if (null != v)
			{
				class_list = new ObservableCollection<object>(DisplayClass.Assign(v));
			}
			//class_list.CollectionChanged

		}

		protected override void RelayCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			try
			{
				Application.Current.Dispatcher.Invoke((Action)(() =>
				{
					switch (e.Action)
					{
						case NotifyCollectionChangedAction.Add:
							if (this.Value.Count != this.Lists.Count)
							{
								var i = e.NewStartingIndex;
								foreach (var it in DisplayClass.Assign(e.NewItems))
								{
									Lists.Insert(i, it);
									i++;
								}

								Selection = i;
							}
							break;
						case NotifyCollectionChangedAction.Move:
							Lists.Move(e.OldStartingIndex, e.NewStartingIndex);
							break;
						case NotifyCollectionChangedAction.Remove:
							if (this.Value.Count != this.Lists.Count)
							{
								var i = e.OldStartingIndex;
								foreach (var it in e.OldItems)
								{
									Lists.RemoveAt(i);
								}
								Selection = i - 1;
							}
							//throw new NotImplementedException("The Remove action of the event was not processed by the RelayCollectionChange. It is not implemented!");
							break;
						case NotifyCollectionChangedAction.Replace:
							throw new NotImplementedException("The Replace action of the event was not processed by the RelayCollectionChange. It is not implemented!");
						//break;
						case NotifyCollectionChangedAction.Reset:
							Lists.Clear();
							break;
						default:
							break;
					}

				}));
			}
			catch (Exception ex)
			{
				ErrorReport.CollectExceptions(ex);

				//Fall back solution: We recreate the Lists
				UpdateList();
				OnPropertyChanged("Lists");
			}

			//if (e.Action == NotifyCollectionChangedAction.Add)
		}

		protected override void RelayPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Name)
			{
				OnPropertyChanged("Value");
			}
			else if (e.PropertyName == VPName)
			{
				OnPropertyChanged("Visibility");
			}
		}

		private ObservableCollection<object> class_list;
		protected CommandTypeList cmd_list;
		protected CommandType key;

		protected int selection;
		public int Selection
		{
			set
			{
				SetField(ref selection, value);
				OnPropertyChanged("Selected");
			}
			get
			{
				return selection;
			}
		}

		protected T editing;
		public T Editing
		{ set { SetField(ref editing, value); } get { return editing; } }

		public virtual ObservableCollection<object> Lists
		{
			get
			{
				return class_list;
			}
			set { }
		}

		public CommandTypeList Cmds { get { return cmd_list; } }

		public new IList Value
		{
			set
			{
				if (null != pInfo)
				{
					pInfo.SetValue(obj, value);
				}
			}
			get
			{
				if (null != pInfo)
				{
					try
					{
						var v = pInfo.GetValue(obj);
						if (null == v)
						{
							return null;
						}
						if (typeof(IList).IsAssignableFrom(v.GetType()))
						{
							//var vc = (IEnumerable)v;
							//var vct = vc.Cast<object>().ToList();
							return (IList)v;
						}

					}
					catch (Exception e)
					{
						ErrorReport.CollectExceptions(e);
					}

				}
				return new List<object>();
			}
		}

		public virtual void Add(bool after)
		{
			object oo = null;

			try
			{
				if (null != this.GenericParameterType)
				{
					Type gt = this.GenericParameterType;

                    if (null != Editing)
                    {
                        if (gt.IsValueType || typeof(string).IsAssignableFrom(gt))
                        {
                            oo = editing;
                        }
                        else
                        {
                            oo = Activator.CreateInstance(Editing.GetType());
                        }
                    }
                    else
                    {
                        if (typeof(string).IsAssignableFrom(gt))
                        {
                            oo = "";
                        }
                        else
                        {
                            if (gt.IsInterface || gt.IsAbstract)
                            { }
                            else
                            {
                                oo = Activator.CreateInstance(gt);
                            }
                        }
                    }
				}

				if (null != oo)
				{
					var nidx = Selection + (after ? 1 : 0);

					if ((nidx < class_list.Count) && (nidx >= 0))
					{
						class_list.Insert(nidx, DisplayClass.Process(oo));
						Value.Insert(nidx, oo);
						Selection = nidx;
					}
					else
					{
						class_list.Add(DisplayClass.Process(oo));
						Value.Add(oo);
						Selection = class_list.Count - 1;
					}
				}
			}
			catch (Exception e)
			{
				ErrorReport.CollectExceptions(e);
			}
		}
		public virtual void Remove()
		{
			try
			{
				var ridx = -1;

				if ((Selection < class_list.Count) && (Selection >= 0))
				{
					ridx = Selection;
				}
				else if (class_list.Count > 0)
				{
					ridx = 0;
				}

				if (ridx >= 0)
				{
					class_list.RemoveAt(ridx);
					Value.RemoveAt(ridx);
					ridx--;
					Selection = ridx;
				}

				if (class_list.Count == 0)
				{
					Selection = -1;
				}
				//OnPropertyChanged("Lists");
			}
			catch (Exception e)
			{
				ErrorReport.CollectExceptions(e);
			}

		}
		public void Move(int offset)
		{
			if (Selection < 0)
			{
				return;
			}

			var idx = Selection;
			var nidx = Selection + (offset <= 0 ? offset : offset);

			if (offset < 0)
			{
				if (nidx < 0)
				{
					return;
				}
			}
			else
			{
				if (nidx > class_list.Count - 1)
				{
					return;
				}
			}

			var v = Value[idx];
			Value.RemoveAt(idx);
			class_list.RemoveAt(idx);

			Value.Insert(nidx, v);
			class_list.Insert(nidx, DisplayClass.Process(v));
			Selection = nidx;

		}
		public int Count
		{
			get { return Value.Count; }
		}

		public void Select()
		{
			if (Selection < 0)
			{
				return;
			}
			Editing = (T)Value[Selection];
		}

		public void Update()
		{
			if (Selection < 0)
			{
				return;
			}

			var idx = Selection;
			Value[idx] = Editing;
			class_list[idx] = DisplayClass.Process(Value[idx]);
			Selection = idx;
		}
	}

	public class DisplayObjectList : DisplayListBase<object>
	{
		public DisplayObjectList(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o)
		{

		}
	}

	public class DisplayStringList : DisplayListBase<string>
	{
		public DisplayStringList(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o) { }
	}

	public class DisplayIntList : DisplayListBase<int>
	{
		public DisplayIntList(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o) { }
	}

	public class DisplayUIntList : DisplayListBase<uint>
	{
		public DisplayUIntList(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o) { }
	}

	public class DisplayDoubleList : DisplayListBase<double>
	{
		public DisplayDoubleList(PropertyInfo pi, INotifyPropertyChanged o)
			: base(pi, o) { }
	}

	public class DisplayList : List<IDisplayObject> { }

	public class DisplayGroups : Dictionary<string, DisplayList> { }

	public interface ISummary
	{
		string Summary { get; }

	}

	public class DisplayClass : DisplayPropertyBase<INotifyPropertyChanged>
	{
		public DisplayClass(object o)
			: base(o)
		{
			Expanded = true;
			CalculateGroups(o);
		}

		public DisplayClass(PropertyInfo pi, object o)
			: base(pi, o)
		{
			Expanded = true;
			CalculateGroups(Value);
		}

		public static object Process(object o)
		{
			try
			{
				if (typeof(INotifyPropertyChanged).IsAssignableFrom(o.GetType()))
				{
					var oo = o as INotifyPropertyChanged;
					return (object)(new DisplayClass(oo));
				}
				else
				{
					return o;
				}
			}
			catch (Exception e)
			{
				ErrorReport.CollectExceptions(e);
			}
			return null;
		}

		public static List<object> Assign(object l)
		{
			var t = l.GetType();

			if (typeof(IEnumerable<object>).IsAssignableFrom(t))
			{
				var el = (IEnumerable<object>)l;
				return el.Select((o) =>
				{
					return Process(o);
				}).ToList();
			}
			else if (typeof(IEnumerable).IsAssignableFrom(t))
			{
				var el = (IEnumerable)l;
				return el.Cast<object>().Select((o) =>
				{
					return Process(o);
				}).ToList();
			}
			else if (typeof(INotifyPropertyChanged).IsAssignableFrom(t))
			{
				return new List<object>() { new DisplayClass(l) };
			}
			return new List<object>();
		}

		void CalculateGroups(object o)
		{
			if (null == o)
			{
				return;
			}
			var oda = (DescriptionAttribute)o.GetType().GetCustomAttribute<DescriptionAttribute>();
			if (null == m_Title)
			{
				m_Title = (null != oda) ? oda.Description + ":" : ((null != this.Label) ? this.Label : string.Format("[{0}]", o.GetType().Name));
			}

			if (typeof(ISummary).IsAssignableFrom(o.GetType()))
			{
				HasSummary = true;
				var cs = (ISummary)o;
				Expanded = false;
			}
			else
			{
				Expanded = true;
			}

			Groups = new DisplayGroups();

			var ownPropDescriptors = o.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
			var allPropDescriptors = o.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public );
			var propDescriptors = allPropDescriptors.Except(ownPropDescriptors).ToList();
			propDescriptors.AddRange(ownPropDescriptors);

			foreach (var p in propDescriptors)
			{
				if (p.IsDefined(typeof(DisplayAttribute), false))
				{
					var dia = p.GetCustomAttribute<DisplayAttribute>();

					var da = p.GetCustomAttribute<CategoryAttribute>();
					var gn = (null != da) ? da.Category : "Default";

					if (!Groups.ContainsKey(gn))
					{
						Groups.Add(gn, new DisplayList());
					}

					var dl = Groups[gn];

					if (typeof(INotifyPropertyChanged).IsAssignableFrom(o.GetType()))
					{
						var oo = o as INotifyPropertyChanged;
						if (typeof(string).IsAssignableFrom(p.PropertyType))
						{
							dl.Add(new DisplayString(p, oo));
						}
						else if (typeof(double).IsAssignableFrom(p.PropertyType))
						{
							dl.Add(new DisplayDouble(p, oo));
						}
						else if (typeof(int).IsAssignableFrom(p.PropertyType))
						{
							dl.Add(new DisplayInt(p, oo));
						}
						else if (typeof(uint).IsAssignableFrom(p.PropertyType))
						{
							dl.Add(new DisplayUInt(p, oo));
						}
						else if (typeof(bool).IsAssignableFrom(p.PropertyType))
						{
							dl.Add(new DisplayBool(p, oo));
						}
						else if (p.PropertyType.IsEnum)
						{
							dl.Add(new DisplayEnum(p, oo));
						}
						else if (typeof(IList).IsAssignableFrom(p.PropertyType))
						{
							var gt = Common.GetGenericParameter(p.PropertyType);

							if (null != gt)
							{
								if (typeof(string).IsAssignableFrom(gt))
								{
									dl.Add(new DisplayStringList(p, oo));
								}
								else if (typeof(int).IsAssignableFrom(gt))
								{
									dl.Add(new DisplayIntList(p, oo));
								}
								else if (typeof(uint).IsAssignableFrom(gt))
								{
									dl.Add(new DisplayUIntList(p, oo));
								}
								else if (typeof(double).IsAssignableFrom(gt))
								{
									dl.Add(new DisplayDoubleList(p, oo));
								}
								else
								{
									dl.Add(new DisplayObjectList(p, oo));
								}

							}
							else
							{
								dl.Add(new DisplayObjectList(p, oo));
							}
						}
						else if (typeof(INotifyPropertyChanged).IsAssignableFrom(p.PropertyType))
						{
							dl.Add(new DisplayClass(p, o));
						}

					}
					else if (typeof(INotifyPropertyChanged).IsAssignableFrom(p.PropertyType))
					{
						dl.Add(new DisplayClass(p, o));
					}
					else
					{
						throw new InvalidOperationException(string.Format("The property type {0} has not been implemented yet!", p.PropertyType.Name));

					}
				}
			}
		}

		protected override void RelayPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.RelayPropertyChanged(sender, e);
			if (HasSummary)
			{
				OnPropertyChanged("Title");
			}
		}

		private string m_Title;
		public string Title
		{
			set
			{
				SetField(ref m_Title, value);
			}
			get
			{
				if (HasSummary && !Expanded)
				{
					var cs = (ISummary)obj;
					return m_Title + " " + cs.Summary;
				}
				else
				{
					return m_Title;
				}

			}
		}


		private bool m_Expanded;
		public bool Expanded
		{
			set
			{
				if (SetField(ref m_Expanded, value))
				{
					OnPropertyChanged("Title");
				}
			}
			get { return m_Expanded; }
		}


		public bool HasSummary { set; get; }


		public DisplayGroups Groups { set; get; }
		public List<DisplayList> Lists
		{
			get
			{
				return Groups?.Values?.ToList();
			}
		}

	}

	public static class EnumExtensions
	{

		/// <summary>
		/// This extension method gives you the ability to use GetAttribute on a value of an enumeration.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Enum value) where T : Attribute
		{
			var type = value.GetType();
			var memberInfo = type.GetMember(value.ToString());
			var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
			if (attributes.Length > 0)
			{
				return (T)attributes[0];
			}
			else
			{
				return null;
			}
		}
	}

	public class EnumDescriptionConverter : IValueConverter
	{
		private string GetEnumDescription(Enum enumObj)
		{
			var desca = enumObj.GetAttribute<DescriptionAttribute>();
			var ss = desca == null ? enumObj.ToString() : desca.Description;
			return ss;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Enum myEnum = (Enum)value;
			string description = GetEnumDescription(myEnum);
			return description;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Empty;
		}
	}

	public class EnumCategoryConverter : IValueConverter
	{
		private string GetEnumDescription(Enum enumObj)
		{
			var cata = enumObj.GetAttribute<CategoryAttribute>();
			var ss = cata == null ? "" : " (" + cata.Category + ")";
			return ss;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Enum myEnum = (Enum)value;
			string description = GetEnumDescription(myEnum);
			return description;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Empty;
		}
	}

	public class StringNullOrEmptyToVisibilityConverter : System.Windows.Markup.MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return string.IsNullOrEmpty(value as string)
				? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}

	public class BooleanToVisibilityShowHideConverter : System.Windows.Markup.MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				var vis = (bool)value;
				return vis
					? Visibility.Visible : Visibility.Hidden;
			}
			catch (Exception e)
			{
				return Visibility.Hidden;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}

}
