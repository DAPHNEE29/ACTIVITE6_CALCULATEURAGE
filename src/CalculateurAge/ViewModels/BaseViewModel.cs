using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculateurAge.ViewModels;

/// <summary>
/// Classe mère de tous les ViewModels. Elle apporte le contrat
/// <see cref="INotifyPropertyChanged"/> et l helpers qui fait tout le travail :
/// le ViewModel s'engage à signaler ses changements, le moteur de binding y est
/// abonné, et <c>[CallerMemberName]</c> écrit automatiquement le nom de la
/// propriété appelante — donc aucune faute de frappe possible.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
	/// <summary>Levé à chaque modification. Le moteur de binding y est abonné.</summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>Prévient la vue qu'une propriété a changé.</summary>
	protected void OnPropertyChanged([CallerMemberName] string? nom = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));

	/// <summary>
	/// Affecte une valeur et notifie en une seule ligne. Renvoie <c>true</c> si la
	/// valeur a réellement changé : garde-fou qui évite les notifications
	/// inutiles et les boucles infinies en mode TwoWay.
	/// </summary>
	protected bool SetField<T>(ref T champ, T valeur, [CallerMemberName] string? nom = null)
	{
		if (EqualityComparer<T>.Default.Equals(champ, valeur))
		{
			return false;
		}

		champ = valeur;
		OnPropertyChanged(nom);

		return true;
	}
}