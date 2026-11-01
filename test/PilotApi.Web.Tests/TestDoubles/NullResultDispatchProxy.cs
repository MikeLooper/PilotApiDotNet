using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PilotApi.Web.Tests.TestDoubles
{
	/// <summary>
	/// A proxy whose asynchronous methods complete with a default response, so its Result is Null and no error is set.
	/// </summary>
	internal class NullResultDispatchProxy : DispatchProxy
	{
		private static readonly MethodInfo FromResultMethod = typeof(Task)
			.GetMethods()
			.Single(method => method.Name == nameof(Task.FromResult));

		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
		{
			var responseType = targetMethod!.ReturnType.GetGenericArguments()[0];

			return FromResultMethod
				.MakeGenericMethod(responseType)
				.Invoke(null, [Activator.CreateInstance(responseType)]);
		}
	}
}
