using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.IO;
//using Outlook = Microsoft.Office.Interop.Outlook;
//using System.Deployment.Application;
//using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Numerics;



namespace UserInterfaces
{

	/// <summary>
	/// This class contains some common static methods. 
	/// </summary>
	public class Common
	{
		private Common()
		{
		}

		/// <summary>
		/// This is the static constructor of this class. It gives default values for the following properties: RootName, ApplicationNamespace, ConfigID.
		/// </summary>
		/// <remarks>
		/// It is not recommended to change those values. 
		/// </remarks>
		static Common()
		{
			RootName = "Root";
			try
			{
				ApplicationNamespace = XElementNormalizer.Replace(GetApplicationNamespace(), "_");
			}
			catch (Exception e)
			{
				// ??? 
				ApplicationNamespace = "SensorCommon";

			}
			ConfigID = "Default";

			Me = new Common();
			try
			{
				TB = new TextBoxConfig();
			}
			catch (Exception e)
			{
				// We are in a MTA thread!?
			}

		}

		static Common Me;
		static TextBoxConfig TB;

		private static MethodBase GetCallingMethod()
		{
			return new System.Diagnostics.StackFrame(2, false).GetMethod();
		}

		/// <summary>
		/// This is the name of the xml root tag. 
		/// </summary>
		public static string RootName { set; get; }

		/// <summary>
		/// This is an extra separation based the application name for the configuration in order to have one configuration file for multiple application.
		/// </summary>
		public static string ApplicationNamespace { set; get; }

		/// <summary>
		/// This is the default configuration id. This is used on the case the parameter configID is null.
		/// </summary>
		public static string ConfigID { set; get; }

		/// <summary>
		/// This regular expression is used to remove characters are not conform to xml tag names.
		/// </summary>
		public static Regex XElementNormalizer = new Regex(@"[\.+]");

		/// <summary>
		/// The empty namespace for xml.
		/// </summary>
		public static XmlSerializerNamespaces EmptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

		/// <summary>
		/// This method returns the namespace of the calling application. 
		/// </summary>
		/// <returns>The namespace of the calling application.</returns>
		public static string GetApplicationNamespace()
		{
			return Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace;
		}

		public static String GetVersionInfo()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
			var prod = fileVersionInfo.ProductVersion;

			// FIXME: ApplicationDeploymenr is not available here for some riesen  
			//return ApplicationDeployment.IsNetworkDeployed
			//				   ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
			//				   : prod;
			return "";

		}

		/// <summary>
		/// This method returns the complete xpath to the configuration item based on the type.
		/// </summary>
		/// <param name="t">This is the type.</param>
		/// <param name="configID">This  is the configuration identifier.</param>
		/// <param name="itemID">This is the item identifier.</param>
		/// <returns>The complete xpath to the configuration item based on the type.</returns>
		public static string GetConfigPathByType(Type t, string configID, string itemID)
		{
			var ns = XElementNormalizer.Replace(t.Namespace, "_"); ;
			var n = XElementNormalizer.Replace(t.Name, "_");

			var configText = string.Format("Config[@ID = '{0}']", (null == configID) ? ConfigID : configID);
			var itemText = (null == itemID) ? "" : string.Format("/Item[@ID = '{0}']", itemID);

			var p = String.Format("/{0}/{1}/{2}/{3}{4}/{5}", RootName, ApplicationNamespace, configText, ns, itemText, n);
			return p;
		}

		/// <summary>
		/// This method returns the xpath to the configuration item based on the type.
		/// </summary>
		/// <param name="t">This is the type.</param>
		/// <param name="configID">This  is the configuration identifier.</param>
		/// <param name="itemID">This is the item identifier.</param>
		/// <returns>The xpath to the configuration item based on the type.</returns>
		public static string GetConfigRootByType(Type t, string configID, string itemID)
		{
			var ns = XElementNormalizer.Replace(t.Namespace, "_"); ;
			//var n = XElementNormalizer.Replace(t.Name, "_");

			var configText = string.Format("Config[@ID = '{0}']", (null == configID) ? ConfigID : configID);
			var itemText = (null == itemID) ? "" : string.Format("/Item[@ID = '{0}']", itemID);

			var p = String.Format("/{0}/{1}/{2}/{3}{4}", RootName, ApplicationNamespace, configText, ns, itemText);
			return p;
		}

		/// <summary>
		/// Makes the X path. Use a format like //configuration/appSettings/add[@key='name']/@value
		/// </summary>
		/// <param name="nav">This is the xpath navigator for the xml document we use.</param>
		/// <param name="xpath">This is the xpath we have to create.</param>
		public static void createNodeFromXPath(XPathNavigator nav, string xpath)
		{
			// Create a new Regex object
			var r = new Regex(@"/+([\w]+)(\[\s*@([\w]+)\s*=\s*'?([^']*)'?\s*\])?|/@([\w]+)");

			// Find matches
			Match m = r.Match(xpath);

			XPathNavigator currentNode = nav;
			var currentPath = new StringBuilder();

			while (m.Success)
			{
				String currentXPath = m.Groups[0].Value;    // "/configuration" or "/appSettings" or "/add"
				String elementName = m.Groups[1].Value;     // "configuration" or "appSettings" or "add"
				String filterName = m.Groups[3].Value;      // "" or "key"
				String filterValue = m.Groups[4].Value;     // "" or "name"
				String attributeName = m.Groups[5].Value;   // "" or "value"

				var builder = new StringBuilder(currentPath.ToString());
				builder.Append("/" + elementName);
				var relativePath = builder.ToString();

				currentPath.Append(currentXPath);
				var relativePath2 = currentPath.ToString();

				var newNode = nav.SelectSingleNode(relativePath2);

				if (newNode == null)
				{
					if (!string.IsNullOrEmpty(attributeName))
					{
						currentNode.CreateAttribute("", attributeName, "", "");
						//newNode = doc.SelectSingleNode(relativePath2);
					}
					else if (!string.IsNullOrEmpty(elementName))
					{
						currentNode.AppendChildElement("", elementName, "", "");
						newNode = nav.SelectSingleNode(relativePath + "[last()]");
						if (!string.IsNullOrEmpty(filterName))
						{
							newNode.CreateAttribute("", filterName, "", filterValue);
						}

					}
					else
					{
						throw new FormatException("Incorrect or not supported XPath at " + relativePath);
					}
				}

				currentNode = nav.SelectSingleNode(relativePath2); ;

				m = m.NextMatch();
			}

			// Assure that the node is found or created
			if (nav.SelectSingleNode(xpath) == null)
			{
				throw new FormatException("Incorrect or not supported XPath: " + xpath);
			}

		}

		/// <summary>
		/// This method creates a new empty xml document with some namespace declaration.
		/// </summary>
		/// <returns>The created empty xml document.</returns>
		public static XmlDocument createEmptyDocument()
		{
			var doc = new XmlDocument();

			var header = doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
			var project = doc.AppendChild(doc.CreateElement(RootName));

			doc.DocumentElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			doc.DocumentElement.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

			return doc;
		}

		/// <summary>
		/// This generic method loads a defined class from the given xpath navigator selected by the configuration identifier and item identifier.
		/// </summary>
		/// <typeparam name="T">This is the base class for the generic.</typeparam>
		/// <param name="nav">The xpath navigator.</param>
		/// <param name="configID">The configuration identifier.</param>
		/// <param name="itemID">The item identifier</param>
		/// <returns></returns>
		public static T Load<T>(XPathNavigator nav, string configID = null, string itemID = null, bool newOnMissing = true) where T : class, new()
		{
			var t = typeof(T);
			T ret = null;
			//var r = GetConfigRootByType(t, configID, itemID);
			var p = GetConfigPathByType(t, configID, itemID);

			var nod = nav.SelectSingleNode(p);
			if (null != nod)
			{
				using (var xr = nod.ReadSubtree())
				{
					var s = new XmlSerializer(t);
					ret = (dynamic)s.Deserialize(xr);
				}
			}
			else if (newOnMissing)
			{
				ret = new T();
			}

			return ret;
		}

		/// <summary>
		/// This generic method saves a defined class from the given xpath navigator based on the configuration identifier and item identifier.
		/// </summary>
		/// <typeparam name="T">This is the base class for the generic.</typeparam>
		/// <param name="nav">The xpath navigator.</param>
		/// <param name="o"></param>
		/// <param name="configID">The configuration identifier.</param>
		/// <param name="itemID">The item identifier</param>
		public static void Save<T>(XPathNavigator nav, T o, string configID = null, string itemID = null) where T : class, new()
		{
			var t = typeof(T);
			var r = GetConfigRootByType(t, configID, itemID);
			var p = GetConfigPathByType(t, configID, itemID);

			var nod = nav.SelectSingleNode(p);

			if (null != nod)
			{
				nod.DeleteSelf();
			}

			nod = nav.SelectSingleNode(r);
			if (null == nod)
			{
				createNodeFromXPath(nav, r);
			}

			nod = nav.SelectSingleNode(r);
			using (var xw = nod.AppendChild())
			{
				//xw.WriteComment("");
				xw.WriteWhitespace("");
				var s = new XmlSerializer(t);
				s.Serialize(xw, o, EmptyNamepsaces);
			}

		}

		/// <summary>
		/// This class was constucted to have a static initialization in order to update the default behavior of the text boxes.  
		/// </summary>
		protected class TextBoxConfig
		{
			/// <summary>
			/// This function adds class event handler to text box classes in order to select text on got focus event.
			/// </summary>
			public TextBoxConfig()
			{
				EventManager.RegisterClassHandler(typeof(TextBox),
					TextBox.GotFocusEvent,
					new RoutedEventHandler(TextBox_GotFocus));

				EventManager.RegisterClassHandler(typeof(TextBox),
					TextBox.GotMouseCaptureEvent,
					new RoutedEventHandler(TextBox_GotFocus));

				EventManager.RegisterClassHandler(typeof(TextBox),
					TextBox.LostFocusEvent,
					new RoutedEventHandler(TextBox_LostFocus));

			}

			private object LastSelect = null;
			private int LastCount = 0;
			/// <summary>
			/// This is the event we use in order to select all text inside the text boxes on got focus event. 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e">Contains state information and event data associated with a routed event. </param>
			/// <remarks>We not always select all the texts witch leaves the opportunity to the user to put the cursor to the end of the line.</remarks>
			private void TextBox_GotFocus(object sender, RoutedEventArgs e)
			{
				if ((LastSelect) != (sender))
				{
					LastCount = 1;
				}
				else
				{
					LastCount++;
				}
				if (LastCount <= 2)
				{
					(sender as TextBox).SelectAll();
				}

				LastSelect = sender;
			}

			private void TextBox_LostFocus(object sender, RoutedEventArgs e)
			{
				LastSelect = null;
				LastCount = 0;

			}


		}

		public static Type GetGenericParameter(Type type)
		{
			var t = type;
			Type gt = null;
			while ((null != t) && (null == gt))
			{
				if ((null != t.GenericTypeArguments) && (t.GenericTypeArguments.Length > 0))
				{
					gt = t.GenericTypeArguments[0];
					break;
				}
				t = t.BaseType;
			}

			return gt;
		}

		/// <summary>
		/// This is only proof of concept. You can show gui from labview too.
		/// </summary>
		/// <param name="o"></param>
		public static bool ShowConfigPromptDialog(object o)
		{
			bool ret = false;
			var t = new Thread((ThreadStart)(() =>
			{

				var fd = new UserInterfaces.ConfigPrompt(o);
				//TB = new TextBoxConfig();
				//var app = new Application();
				//app.Run(fd);
				var dr = fd.ShowDialog();
				ret = dr ?? false;

			}));

			t.SetApartmentState(ApartmentState.STA);
			t.Start();
			t.Join();

			return ret;
		}

#if OUTLOOK == true
        public static void sendEMailThroughOUTLOOK(string address, string subject, string msg)
		{
			try
			{
				// Create the Outlook application.
				Outlook.Application oApp = new Outlook.Application();
				// Create a new mail item.
				Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
				// Set HTMLBody. 
				//add the body of the email
				oMsg.HTMLBody = msg;
				////Add an attachment.
				//String sDisplayName = "MyAttachment";
				//int iPosition = (int)oMsg.Body.Length + 1;
				//int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
				////now attached the file
				//Outlook.Attachment oAttach = oMsg.Attachments.Add(@"C:\\fileName.jpg", iAttachType, iPosition, sDisplayName);

				//Subject line
				oMsg.Subject = subject;
				// Add a recipient.
				Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
				// Change the recipient in the next line if necessary.
				foreach (var s in address.Split(';'))
				{
					// "Laszlo.Scherman@hu.bosch.com"
					Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(s);
					oRecip.Resolve();
					oRecip = null;
				}
				// Send.
				oMsg.Send();
				// Clean up.
				oRecips = null;
				oMsg = null;
				oApp = null;
			}//end of try block
			catch (Exception ex)
			{
			}//end of catch
		}//end of Email Method
#endif

		public static bool ValidateFilename(string fileName)
		{
			System.IO.FileInfo fi = null;
			try
			{
				fi = new System.IO.FileInfo(fileName);
			}
			catch (ArgumentException) { }
			catch (System.IO.PathTooLongException) { }
			catch (NotSupportedException) { }

			if (ReferenceEquals(fi, null))
			{
				return false;
			}
			else
			{
				System.IO.DirectoryInfo di = null;
				try
				{
					di = Directory.CreateDirectory(fi.DirectoryName);
				}
				catch (Exception)
				{
				}

				if (ReferenceEquals(di, null))
				{
					return false;
				}
				else
				{
					return true;

				}

			}

		}


		public static bool ValidateFilenameCreate(string fileName)
		{
			System.IO.FileInfo fi = null;
			try
			{
				fi = new System.IO.FileInfo(fileName);
			}
			catch (Exception e) { ErrorReport.CollectExceptions(e); return false; }



			if (ReferenceEquals(fi, null))
			{
				return false;
			}
			else
			{
				System.IO.DirectoryInfo di = null;
				try
				{
					di = Directory.CreateDirectory(fi.DirectoryName);
				}
				catch (Exception e) { ErrorReport.CollectExceptions(e); return false; }

				if (ReferenceEquals(di, null))
				{
					return false;
				}
				else
				{
					try
					{
						var sw = fi.AppendText();
						sw.Close();
					}
					catch (Exception e) { ErrorReport.CollectExceptions(e); return false; }

					return true;

				}

			}

		}

		public static string GetDisplayName()
		{
			string fullName = null;
			//using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
			//{
			//	using (UserPrincipal user
			//			= UserPrincipal.Current)
			//	{
			//		if (user != null)
			//		{
			//			fullName = user.DisplayName;
			//		}
			//	}
			//}
			return fullName;
		}

	}


	/// <summary>
	/// This static class contains some common extension methods.
	/// </summary>
	public static class CommonExtensions
	{
		/// <summary>
		/// This is an extension method it evaluates whether the given double number is a finite number on not.
		/// </summary>
		/// <param name="value">This is the double value on the evaluation applies to.</param>
		/// <returns>This function returns the 'true' if the double value is finite.</returns>
		public static bool IsFinite(this double value)
		{
			return !(double.IsInfinity(value) || double.IsNaN(value));
		}

#region Extension Methods for flags

		/// <summary>
		/// Includes an enumerated type and returns the new value
		/// </summary>
		public static T Attach<T>(this Enum value, T flag)
		{
			//determine the values
			var res = Convert.ToUInt64(value) | Convert.ToUInt64(flag);
			value = (Enum)Enum.ToObject(value.GetType(), res); ;
			return (T)(dynamic)value;
		}

		/// <summary>
		/// Removes an enumerated type and returns the new value
		/// </summary>
		public static T Detach<T>(this Enum value, T flag)
		{
			//determine the values
			var res = Convert.ToUInt64(value) &  ~Convert.ToUInt64(flag);
			value = (Enum)Enum.ToObject(value.GetType(), res); ;
			return (T)(dynamic)value;
		}

		/// <summary>
		/// Assignes a flag to an enumerated
		/// </summary>
		public static T Assign<T>(this Enum value, T flag, bool condition)
		{
			var res = condition ? value.Attach(flag) : value.Detach(flag);
			value = (dynamic)res;
			return res;
		}

		/// <summary>
		/// Checks if an enumerated type is missing a value
		/// </summary>
		public static bool Missing<T>(this Enum obj, T flag)
		{
			return !obj.HasFlag((dynamic)flag);
		}

#endregion


	}


	static class TypeSwitch
	{
		public class CaseInfo
		{
			public bool IsDefault { get; set; }
			public Type Target { get; set; }
			public Action<object> Action { get; set; }
		}

		public static void Do(object source, params CaseInfo[] cases)
		{
			var type = source.GetType();
			foreach (var entry in cases)
			{
				if (entry.IsDefault || entry.Target.IsAssignableFrom(type))
				{
					entry.Action(source);
					break;
				}
			}
		}

		public static CaseInfo Case<T>(Action action)
		{
			return new CaseInfo()
			{
				Action = x => action(),
				Target = typeof(T)
			};
		}

		public static CaseInfo Case<T>(Action<T> action)
		{
			return new CaseInfo()
			{
				Action = (x) => action((T)x),
				Target = typeof(T)
			};
		}

		public static CaseInfo Default(Action action)
		{
			return new CaseInfo()
			{
				Action = x => action(),
				IsDefault = true
			};
		}
	}

	/// <summary>
	/// This is the base implementation os the INotifyPropertyChanged. It is also used in the NetBSI Library in order to have a common and easy to use implementation.  
	/// </summary>
	public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler propertychanged;

		/// <summary>
		/// Occurs when a property value changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (NewSubscriber(value.GetMethodInfo().DeclaringType))
				{
					propertychanged += value;
				}
			}
			remove { propertychanged += value; }
		}

		/// <summary>
		/// This method is called on the eatch new subscrioption of our PropertyChanged event. 
		/// </summary>
		/// <param name="t">The subscriber class</param>
		/// <returns>Returns whether the class is allowed to subscribe.</returns>
		protected virtual bool NewSubscriber(Type t)
		{
			return true;
		}

		/// <summary>
		/// This method raise the event.
		/// </summary>
		/// <param name="propertyName">The name of the property has changed</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = propertychanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// This generic method is used to update the field behind the property.  
		/// </summary>
		/// <typeparam name="T">The type of the property. This is evaluated automatically</typeparam>
		/// <param name="field">This is the reference to the variable.</param>
		/// <param name="value">The value.</param>
		/// <param name="propertyName">The name of the property. This is evaluated automatically</param>
		/// <returns>This method return true on the case the field was updated.</returns>
		protected bool SetField<T>(ref T field, T value,
			[CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		/// This generic method is used to update the field behind the property.  
		/// </summary>
		/// <typeparam name="T">The type of the property. This is evaluated automatically</typeparam>
		/// <param name="field">This is the reference to the variable.</param>
		/// <param name="value">The value.</param>
		/// <param name="Hi">The high limit.</param>
		/// <param name="Lo">The low limit.</param>
		/// <param name="propertyName">The name of the property. This is evaluated automatically</param>
		/// <returns>This method return true on the case the field was updated.</returns>
		protected bool SetFieldHiLo<T>(ref T field, T value, T Hi, T Lo,
			[CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			if (Comparer<T>.Default.Compare(value, Hi) > 0)
			{
				throw new ArgumentException(string.Format("The value has to be lower or equal to  {0}", Hi));
			}
			if (Comparer<T>.Default.Compare(value, Lo) < 0)
			{
				throw new ArgumentException(string.Format("The value has to be greater or equal to {0}", Lo));
			}

			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

	}

	public enum ErrorLevel
	{
		Normal = 0,
		Warning = 1,
		Error = 2,
		FatalError = 3,
		UnknowError = 4,
	}


	/// <summary>
	///     This class provides the ability to encapsulate error messages in the operations
	///     performed in the BSI library. An error report contains a error level denoting
	///     whether the operation completed successfully (normal) or unsuccessfully with
	///     error or fatal error or with negligible problems (warning). It contains also
	///     error code denoting the possible cause of error or fatal error and a message
	///     related to the error or warning.
	/// </summary>
	public class ErrorReport
	{
		public enum ERRORCODE
		{
			NO_ERROR_ERRCODE = 1,
			SLOTIO_WSASTARTUP_FAILED = 1000,
			SLOTIO_GETADDRINFO_FAILED = 1001,
			SLOTIO_SOCKET_FAILED = 1002,
			SLOTIO_SERVER_DOWN = 1003,
			SLOTIO_UNABLE_TO_CONNECT_TO_SERVER = 1004,
			SLOTIO_UNABLE_TO_SHUTDOWN_SKT = 1005,
			SLOTIO_UNABLE_TO_SEND = 1006,
			SLOTIO_IOCTL_FAILURE = 1007,
			CMDPAR_CREATE_SEMA_FAILED = 1008,
			CMDPAR_FILE_OPEN_FAILED = 1009,
			CMDPAR_STATS_BOARD_Q_FAIL = 1010,
			MSG_INVALID_MNEMONIC = 1011,
			MSG_MISSING_SEPARATOR = 1012,
			MSG_INVAILD_CMD_ID = 1013,
			MSG_CMD_LEN_EXCEEDS_MAX = 1014,
			MSG_MEM_ALLOCATION_FAIL = 1015,
			MSG_MISSING_CR = 1016,
			MSG_DUPLICATE_CMD_ID = 1017,
			RESP_INVALID_MNEMONIC = 1018,
			RESP_MISSING_SEPARATOR = 1019,
			RESP_INVAILD_CMD_ID = 1020,
			RESP_CMD_LEN_EXCEEDS_MAX = 1021,
			RESP_MEM_ALLOCATION_FAIL = 1022,
			RESP_MISSING_CR = 1023,
			RESP_INVALID_SERIAL_NUM = 1024,
			RESP_DUPLICATE_RESP_ID = 1025,

			UNKNOWN_ERRCODE = -1,
			WSA_STARTUP_FAILED_ERRCODE = -1001,
			WSA_GETADDRINFO_FAILED_ERRCODE = -1101,
			WSA_INVALIDSOCKET_ERRCODE = -1201,
			WSA_CONNECTFAILED_ERRCODE = -1301,
			WSA_SEND_FAILED_ERRCODE = -1501,
			WSA_RECV_FAILED_ERRCODE = -1601,
			BSI_CMDEXECUTION_FAILED_ERRCODE = -2101,
			BSI_CMDRETERR_ERRCODE = -2001

		}



		/// <summary>
		///     Default constructor. Error level is set to Normal, error code is a positive
		///     number denoting absence of error and contains a error message denoting no
		///     error.
		/// </summary>
		public ErrorReport()
		{
			this.errorLevel = ErrorLevel.Normal;
			this.errorCode = (int)ERRORCODE.NO_ERROR_ERRCODE;
			this.message = "No error";
		}
		/// <summary>
		/// Constructor that allows to construct error report with a given error code,
		/// error level and message.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="errLevel">The error level.</param>
		/// <param name="message">The message.</param>
		public ErrorReport(int errorCode, ErrorLevel errLevel, string message)
		{
			this.errorCode = errorCode;
			this.errorLevel = errLevel;
			this.message = message;

			ErrorReport.CollectExceptions(this);
		}

		/// <summary>
		/// Constructor that allows to construct error report with a given error code,
		/// error level and message.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="errLevel">The error level.</param>
		/// <param name="message">The message.</param>
		public ErrorReport(ERRORCODE errorCode, ErrorLevel errLevel, string message)
		{
			this.errorCode = (int)errorCode;
			this.errorLevel = errLevel;
			this.message = message;

			ErrorReport.CollectExceptions(this);
		}

		private static bool m_IsLogActive = false;
		/// <summary>
		/// This property is used to terminate the log threads. 
		/// </summary>
		public static bool IsLogActive { get { return m_IsLogActive; } private set { m_IsLogActive = value; } }

		/// <summary>
		/// We collect the exceptions using this method
		/// </summary>
		/// <param name="o">This is the exception was reaised</param>
		public static void CollectExceptions(object o)
		{
			ExceptionCollection.Enqueue(o);

			if (null == o)
			{
				return;
			}

			var t = o.GetType();
			if ((typeof(Exception).IsAssignableFrom(t)))
			{
				LastException = (Exception)o;
			}

		}


		/// <summary>
		/// Gets error code.
		/// </summary>
		/// <returns>The error code.</returns>
		public int getErrorCode()
		{
			return this.errorCode;
		}

		/// <summary>
		/// Gets error level.
		/// </summary>
		/// <returns>The error level</returns>
		public ErrorLevel getErrorLevel()
		{
			return this.errorLevel;
		}

		/// <summary>
		/// Gets error message.
		/// </summary>
		/// <returns>null if it fails, else the error message.</returns>
		public string getErrorMessage()
		{
			return this.message;
		}

		/// <summary>
		/// You can ask for the last exception was raised with this function.
		/// </summary>
		/// <remarks>
		/// Use this only if we are mostly single threaded...
		/// </remarks>
		/// <returns>This function returns the last exception was raised.</returns>
		public static Exception GetLastError()
		{
			var last = LastException;
			LastException = null;
			return last;
		}
		/// <summary>
		/// Copys the error report content to the current error report.
		/// </summary>
		/// <param name="fileName"></param>
		public static void InitLog(string fileName)
		{
			if (null == fileName)
			{
				m_LogFileName = "NetBSI.log";
			}
			else
			{
				m_LogFileName = fileName;
			}

			if (null == LogThread)
			{
				IsLogActive = true;
				LogThread = new Thread(new ParameterizedThreadStart(ErrorReport.ErrorLog));
				LogThread.Start(null);
			}

		}

		public static void ExitLog()
		{
			IsLogActive = false;
			if (null != LogThread)
			{
				Thread.Sleep(200);


				switch (LogThread.ThreadState)
				{
					case ThreadState.Background:
					case ThreadState.Running:
					case ThreadState.SuspendRequested:
					case ThreadState.Suspended:
					case ThreadState.WaitSleepJoin:
						LogThread.Abort();
						break;
					case ThreadState.Unstarted:
					case ThreadState.AbortRequested:
					case ThreadState.Aborted:
					case ThreadState.StopRequested:
					case ThreadState.Stopped:
					default:
						break;
				}

			}
		}

		public static void Log(string msg)
		{
			new ErrorReport(0, ErrorLevel.Normal, msg);
		}

		public static void Log(string text, params object[] args)
		{
			ErrorReport.Log(string.Format(text, args));
		}

		public static void Clear()
		{
			object o;
			while (ExceptionCollection.TryDequeue(out o))
			{
			}
		}

		public static void Warning(string warn)
		{
			new ErrorReport(0, ErrorLevel.Warning, warn);
		}

		public static void Warning(string text, params object[] args)
		{
			ErrorReport.Warning(string.Format(text, args));
		}

		private static Thread LogThread = null;
		private static ConcurrentQueue<Object> ExceptionCollection = new ConcurrentQueue<Object>();
		private static Exception LastException = null;


		private static String m_LogFileName = null;
		private static void ErrorLog(Object obj)
		{
			var me = (ErrorReport)obj;
			// var path = @"C:\Temp\LogFile.txt";
			String path = ErrorReport.m_LogFileName;
			try
			{
				while (ErrorReport.IsLogActive)
				{
					if (ExceptionCollection.Count > 0)
					{
						StreamWriter w = null;
						try
						{
							w = File.AppendText(path);
							Object o = null;
							while (ExceptionCollection.TryDequeue(out o))
							{
								if (null == o)
								{
									w.WriteLine("{0}\t{1}\tM:\t{2}", System.DateTime.Now.ToString("yyyy.MM.dd. hh:mm:ss"), Environment.UserName, "null");
									continue;
								}

								var t = o.GetType();
								if (typeof(Exception).IsAssignableFrom(t))
								{
									var e = (Exception)o;

									w.WriteLine("{0}\t{1}\tE:\t{2}", System.DateTime.Now.ToString("yyyy.MM.dd. hh:mm:ss"), Environment.UserName, e.Message);
								}
								else if ((typeof(ErrorReport).IsAssignableFrom(t)))
								{
									var er = (ErrorReport)o;
									String s = "U";
									switch (er.errorLevel)
									{
										case ErrorLevel.Normal:
											s = "M";
											break;
										case ErrorLevel.Warning:
											s = "W";
											break;
										case ErrorLevel.Error:
											s = "E";
											break;
										case ErrorLevel.FatalError:
											s = "F";
											break;
										default:
											break;
									}

									w.WriteLine("{0}\t{1}\t{2}:\t{3}", System.DateTime.Now.ToString("yyyy.MM.dd. hh:mm:ss"), Environment.UserName, s, er.message);

								}
								else if (typeof(String).IsAssignableFrom(t))
								{
									var s = (String)o;
									// Log message
									w.WriteLine("{0}\t{1}\tM:\t{2}", System.DateTime.Now.ToString("yyyy.MM.dd. hh:mm:ss"), Environment.UserName, s);
								}
								else
								{
									w.WriteLine("{0}\t{1}\tO:\t{2}", System.DateTime.Now.ToString("yyyy.MM.dd. hh:mm:ss"), Environment.UserName, o.ToString());
								}

							}

							// Update the underlying file.
							w.Flush();
							// Close the writer and underlying file.
							w.Close();
							w = null;
						}
						catch (Exception e)
						{
							ErrorReport.CollectExceptions(e);
						}
						finally
						{
							if (null != w)
							{
								w.Close();

							}
						}
					}

					Thread.Sleep(100);

				}
			}
			catch (IOException ioe)
			{
				
				System.Windows.MessageBox.Show(ioe.Message, "Fatal Error!");

				// Sorry we are not able to write the log into file!!
				// Exiting...

			}

			IsLogActive = false;

		}

		private int errorCode;
		private ErrorLevel errorLevel;
		private String message;



	}

	public class Timestamp
	{
		private static BigInteger bmul;
		private static BigInteger bfreq;

		// Constructor
		private Timestamp()
		{
		}

		static Timestamp()
		{
			bfreq = new BigInteger(System.Diagnostics.Stopwatch.Frequency);
			bmul = new BigInteger(1E6);
		}

		public static long GetMicroseconds()
		{
			var btick = new BigInteger(System.Diagnostics.Stopwatch.GetTimestamp());
			var btmp1 = BigInteger.Multiply(btick, bmul);
			var bts = BigInteger.Divide(btmp1, bfreq);
			var r3 = (long)bts;

			return r3;

		}
	}

}
