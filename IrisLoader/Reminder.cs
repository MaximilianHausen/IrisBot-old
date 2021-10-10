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
		private class ReminderModel
		{
			public DateTime Time { get; set; }
			public string ModuleName { get; set; }
			public string[] Values { get; set; }

			public override bool Equals(object obj)
			{
				if (obj is not ReminderModel model) return false;
				return obj is ReminderModel r && Time == model.Time && ModuleName == model.ModuleName && Values.SequenceEqual(model.Values);
			}
		}

		private static readonly string filePath = Directory.GetCurrentDirectory() + "/ModuleFiles/reminders.json";

		static Reminder()
		{
			if (!File.Exists(filePath))
				File.WriteAllText(filePath, JsonSerializer.Serialize(new List<ReminderModel>()));
		}

		internal static void LoadRemainingTasks()
		{
			var reminders = JsonSerializer.Deserialize<List<ReminderModel>>(File.ReadAllText(filePath));

			reminders.ForEach(r => _ = RunReminder(r));
		}

		/// <summary> Sends a reminder to the module with a given delay and continues even after restarting the application. This should be used for something like "remove this role in 1 Week" </summary>
		internal static void AddReminder(TimeSpan delay, string moduleName, string[] values)
		{
			DateTime time = DateTime.Now + delay;

			// Add to file
			var reminder = new ReminderModel() { Time = time, ModuleName = moduleName, Values = values };
			var reminders = JsonSerializer.Deserialize<List<ReminderModel>>(File.ReadAllText(filePath));
			reminders.Add(reminder);
			File.WriteAllText(filePath, JsonSerializer.Serialize(reminders));

			_ = RunReminder(reminder);
		}

		private static async Task RunReminder(ReminderModel reminder)
		{
			if (DateTime.Now < reminder.Time)
				await Task.Delay(reminder.Time - DateTime.Now);

			var reminders = JsonSerializer.Deserialize<List<ReminderModel>>(File.ReadAllText(filePath));
			reminders.RemoveAll(r => r.Equals(reminder));
			File.WriteAllText(filePath, JsonSerializer.Serialize(reminders));

			if (reminder.ModuleName == "Loader")
				_ = Loader.ReminderRecieved(reminder.Values);
			else
			{
				BaseIrisModule module = Loader.GetModuleByName(reminder.ModuleName);

				if (module is GlobalIrisModule globalModule)
					_ = globalModule.Connection.InvokeEvent(reminder.Values);
				else
					_ = (module as GuildIrisModule).Connection.InvokeEvent(reminder.Values);
			}
		}
	}
}
