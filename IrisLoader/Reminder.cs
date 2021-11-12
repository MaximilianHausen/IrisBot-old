using IrisLoader.Modules;
using MySql.Data.MySqlClient;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace IrisLoader
{
	public static class Reminder
	{
		private static readonly MySqlConnection con = new();
		private static readonly MySqlCommand cmd = new();

		static Reminder()
		{
			string cs = "server=localhost;userid=root;password=" + Loader.config.MySqlPassword;

			con.ConnectionString = cs;
			cmd.Connection = con;
			con.Open();

			cmd.CommandText = "CREATE DATABASE IF NOT EXISTS irisloader";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "USE irisloader";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"CREATE TABLE IF NOT EXISTS reminders(id INTEGER UNSIGNED PRIMARY KEY AUTO_INCREMENT, time VARCHAR(19), module VARCHAR(32), valueList VARCHAR(512))";
			cmd.ExecuteNonQuery();
		}

		internal static void LoadRemainingTasks()
		{
			cmd.CommandText = $"SELECT * FROM reminders";
			using MySqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
				_ = RunReminder(DateTime.Parse(reader.GetString("time")), reader.GetString("module"), JsonSerializer.Deserialize<string[]>(reader.GetString("valueList")));
		}

		/// <summary> Sends a reminder to the module with a given delay and continues even after restarting the application. This should be used for something like "remove this role in 1 Week" </summary>
		internal static void AddReminder(TimeSpan delay, string moduleName, string[] values)
		{
			if (JsonSerializer.Serialize(values).Length > 512) return;

			DateTime time = DateTime.Now + delay;

			// Add to file
			cmd.CommandText = $"INSERT INTO reminders (time, module, valueList) VALUES ('{time}', '{moduleName}', '{JsonSerializer.Serialize(values)}')";
			cmd.ExecuteNonQuery();

			_ = RunReminder(time, moduleName, values);
		}

		private static async Task RunReminder(DateTime time, string moduleName, string[] values)
		{
			if (DateTime.Now < time)
				await Task.Delay(time - DateTime.Now);

			cmd.CommandText = $"DELETE FROM reminders WHERE time = '{time}' AND module = '{moduleName}' AND valueList = '{JsonSerializer.Serialize(values)}'";
			cmd.ExecuteNonQuery();

			if (moduleName == "Loader")
			{
				_ = Loader.ReminderRecieved(values);
			}
			else
			{
				BaseIrisModule module = Loader.GetModuleByName(moduleName);

				if (module is GlobalIrisModule globalModule)
					_ = globalModule.Connection.InvokeEvent(values);
				else
					_ = (module as GuildIrisModule).Connection.InvokeEvent(values);
			}
		}
	}
}
