using Emzi0767.Utilities;
using System;
using System.Threading.Tasks;

namespace IrisLoader.Modules;

public class BaseIrisConnection
{
    protected BaseIrisModule module;
    internal BaseIrisConnection(BaseIrisModule module)
    {
        this.module = module;
    }

    public event AsyncEventHandler<BaseIrisModule, ReminderEventArgs> ReminderRecieved;
    internal Task InvokeEvent(string[] values)
    {
        return ReminderRecieved.Invoke(module, new ReminderEventArgs() { Values = values });
    }

    public void AddReminder(TimeSpan delay, string[] values)
    {
        Reminder.AddReminder(delay, module.Name, values);
    }
}
