using System;

namespace NanoDI
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class InjectAttribute : Attribute
	{
		public InjectAttribute() { }
	}
}