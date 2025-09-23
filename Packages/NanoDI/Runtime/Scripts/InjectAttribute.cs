using System;

namespace NanoDI
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class InjectAttribute : Attribute { }
}