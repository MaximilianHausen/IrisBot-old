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
		protected event Action<int, object[]> ReminderRecieved;
		internal void InvokeEvent(int id, object[] values) => ReminderRecieved.Invoke(id, values);
		public void AddReminder(TimeSpan delay, int id, object[] values) => Reminder.AddReminder(delay, this, id, values);
		#endregion
	}
}
