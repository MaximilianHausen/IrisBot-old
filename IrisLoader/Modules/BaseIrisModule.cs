using Emzi0767.Utilities;
using System;
using System.Threading.Tasks;

namespace IrisLoader.Modules
{
	public abstract class BaseIrisModule
	{
		/// <summary> The name of this module, read from the assembly </summary>
		public string Name
		{
			get { return GetType().Assembly.GetName().Name; }
		}

		/// <summary> Is called after the module was loaded. </summary>
		public abstract Task Load();
		/// <summary> Called after the GuildDownloadCompleted event is called </summary>
		public abstract Task Ready();
		/// <summary> Is called before the module is unloaded. </summary>
		public abstract Task Unload();

		#region Reminder
		protected event AsyncEventHandler<BaseIrisModule, ReminderEventArgs> ReminderRecieved;
		internal Task InvokeEvent(string[] values) => ReminderRecieved.Invoke(this, new ReminderEventArgs() { Values = values });
		public void AddReminder(TimeSpan delay, string[] values) => Reminder.AddReminder(delay, Name, values);
		#endregion
	}
}
