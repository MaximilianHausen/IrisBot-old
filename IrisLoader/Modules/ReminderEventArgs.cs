using Emzi0767.Utilities;
using System;

namespace IrisLoader.Modules;

public class ReminderEventArgs : AsyncEventArgs
{
    public string[] Values { get; internal set; } = Array.Empty<string>();
}
