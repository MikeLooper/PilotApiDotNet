using System.Reflection;

namespace PilotApi.Web.Tests.TestDoubles
{
	internal static class ProxyFactory
	{
		internal static TInterface Create<TInterface>() where TInterface : class
		{
			return DispatchProxy.Create<TInterface, NoOpDispatchProxy>();
		}

		internal static TInterface CreateNullResult<TInterface>() where TInterface : class
		{
			return DispatchProxy.Create<TInterface, NullResultDispatchProxy>();
		}
	}
}
