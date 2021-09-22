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
		private struct ReminderModel : IEquatable<ReminderModel>
		{
			public DateTime Time { get; set; }
			public string ModuleName { get; set; }
			public string[] Values { get; set; }

			public bool Equals(ReminderModel other)
			{
				return !(!Time.Equals(other.Time) || !ModuleName.Equals(other.ModuleName) || !Values.SequenceEqual(other.Values));
			}

			public static bool operator ==(ReminderModel reminder1, ReminderModel reminder2)
			{
				return !(!reminder1.Time.Equals(reminder2.Time) || !reminder1.ModuleName.Equals(reminder2.ModuleName) || !reminder1.Values.SequenceEqual(reminder2.Values));
			}
			public static bool operator !=(ReminderModel reminder1, ReminderModel reminder2)
			{
				return !reminder1.Time.Equals(reminder2.Time) || !reminder1.ModuleName.Equals(reminder2.ModuleName) || !reminder1.Values.SequenceEqual(reminder2.Values);
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
			reminders.RemoveAll(r => r == reminder);
			File.WriteAllText(filePath, JsonSerializer.Serialize(reminders));

			if (reminder.ModuleName == "Loader")
				_ = Loader.ReminderRecieved(reminder.Values);
			else
				_ = Loader.GetModuleByName(reminder.ModuleName).InvokeEvent(reminder.Values);
		}
	}
}
