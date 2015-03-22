using System;
using System.Threading;
using OpenTK;

namespace DeadXDown
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			Thread.CurrentThread.Name = "MainThread";
			GameForm window = new GameForm(640, 480);
			window.Run();
		}
	}
}
