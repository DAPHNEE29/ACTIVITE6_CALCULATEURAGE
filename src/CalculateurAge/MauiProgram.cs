using CalculateurAge.Services;
using CalculateurAge.ViewModels;
using CalculateurAge.Views;
using Microsoft.Extensions.Logging;

namespace CalculateurAge;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiApp.CreateBuilder();

		builder.UseMauiApp<App>();

		// La navigation est abstraite : le ViewModel dépend de l'interface et
		// jamais de Shell, ce qui le rend testable sans interface graphique.
		builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

		// ViewModels
		builder.Services.AddSingleton<CalculateurViewModel>();
		builder.Services.AddSingleton<ResultatPageViewModel>();

		// Les pages sont résolues par injection : leur ViewModel arrive par le
		// constructeur, jamais par une instanciation dans le code-behind.
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<ResultatPage>();

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