namespace CalculateurAge.Services;

/// <summary>
/// Unique point du programme qui touche au <see cref="Shell"/>. Isoler
/// <c>Shell.Current</c> dans cette classe permet de tester les ViewModels sans
/// interface graphique et de changer de mécanisme de navigation sans y toucher.
/// </summary>
public sealed class ShellNavigationService : INavigationService
{
	public Task AllerAsync(string route)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(route);

		Shell? shell = Shell.Current;

		return shell is null ? Task.CompletedTask : shell.GoToAsync(route);
	}

	public Task RetourAsync() => AllerAsync("..");
}