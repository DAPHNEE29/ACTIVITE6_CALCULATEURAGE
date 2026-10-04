using Microsoft.Extensions.Logging;

namespace CalculateurAge;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiApp.CreateBuilder();

		builder.UseMauiApp<App>();

		builder.ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}