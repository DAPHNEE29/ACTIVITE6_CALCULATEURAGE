namespace CalculateurAge.Services;

/// <summary>
/// Abstraction de la navigation. Le ViewModel dépend de cette interface et non
/// de <c>Shell.Current</c> : il peut ainsi déclencher une navigation sans
/// connaître la vue, et les pages restent sans la moindre ligne de logique.
/// </summary>
public interface INavigationService
{
	/// <summary>Navigue vers une route Shell, avec ses paramètres éventuels.</summary>
	Task AllerAsync(string route);

	/// <summary>Revient à la page précédente.</summary>
	Task RetourAsync();
}