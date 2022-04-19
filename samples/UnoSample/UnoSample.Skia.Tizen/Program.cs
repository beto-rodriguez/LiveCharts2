using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace UnoSample.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new UnoSample.App(), args);
		host.Run();
	}
}
}
