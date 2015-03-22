using System;
using Worker;

namespace InternalSection 
{
	public delegate bool TransformHandler(object sender, EventArgs e);
	public delegate void DefaultDelegate();
	public delegate void ArgsDelegate(params object[] args);
}