using IrisLoader.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace IrisLoader
{
	public static class Reminder
	{
		private struct ReminderModel
		{
			public DateTime Time { get; set; }
			public string ModuleName { get; set; }
			public int Id { get; set; }
			public object[] Values { get; set; }
		}

		private static readonly string filePath = Directory.GetCurrentDirectory() + "/tasks.json";

		internal static void LoadRemainingTasks()
		{
			if (!File.Exists(filePath))
				File.WriteAllText(filePath, JsonSerializer.Serialize(new List<ReminderModel>()));
		}

		/// <summary> Sends a reminder to the module with a given delay and continues even after restarting the application. This should be used for something like "remove this role in 1 Week" </summary>
		internal static void AddReminder(TimeSpan delay, BaseIrisModule module, int id, object[] values)
		{
			if (values.Any(o => !o.GetType().IsValueType)) return;

			DateTime time = DateTime.Now + delay;

			// Add to file
			var reminder = new ReminderModel() { Time = time, ModuleName = module.Name, Id = id, Values = values };
			var reminders = JsonSerializer.Deserialize<List<ReminderModel>>(File.ReadAllText(filePath));
			reminders.Add(reminder);
			File.WriteAllText(filePath, JsonSerializer.Serialize(reminders));

			_ = StartReminder(reminder);
		}

		private static async Task StartReminder(ReminderModel reminder)
		{
			await Task.Delay(reminder.Time - DateTime.Now);

			var taskList = JsonSerializer.Deserialize<List<ReminderModel>>(File.ReadAllText(filePath));
			taskList.RemoveAll(t => t.Equals(reminder));
			File.WriteAllText(filePath, JsonSerializer.Serialize(taskList));
			
			Loader.GetModuleByName(reminder.ModuleName).InvokeEvent(reminder.Id, reminder.Values);
		}
	}
}
