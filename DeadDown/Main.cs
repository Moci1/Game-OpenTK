using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
//using Keira.Graphics;
//using Network;
//using Platform;
//using CodeGen;
//using CodeGen.Web;
using System.Text;
using System.Net;
using System.Windows.Forms;

namespace DeadDown
{
	static class MainClass
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
