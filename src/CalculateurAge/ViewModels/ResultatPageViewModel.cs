using CalculateurAge.Services;
using Microsoft.Maui.Controls;

namespace CalculateurAge.ViewModels;

/// <summary>
/// ViewModel de la page de résultat. Le Shell remplit <see cref="Nom"/> et
/// <see cref="Age"/> à partir des paramètres de l'URL de navigation
/// (<c>?nom=…&amp;age=…</c>) : ce sont ces propriétés qui sont affichées.
/// </summary>
[QueryProperty(nameof(Nom), "nom")]
[QueryProperty(nameof(Age), "age")]
public sealed class ResultatPageViewModel : BaseViewModel, IQueryAttributable
{
	private readonly INavigationService _navigation;

	private string _nom = string.Empty;
	private string _age = string.Empty;

	public ResultatPageViewModel(INavigationService navigation)
	{
		_navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

		RetourCommand = new AsyncRelayCommand(() => _navigation.RetourAsync());
	}

	/// <summary>Nom transmis par la page précédente.</summary>
	public string Nom
	{
		get => _nom;
		set => SetField(ref _nom, value);
	}

	/// <summary>Âge transmis par la page précédente, sous forme de texte.</summary>
	public string Age
	{
		get => _age;
		set => SetField(ref _age, value);
	}

	/// <summary>Âge mis en forme, prêt à afficher.</summary>
	public string AgeLibelle => string.IsNullOrWhiteSpace(Age) ? "—" : $"{Age} ans";

	/// <summary>Phrase de synthèse reconstruite à partir des paramètres reçus.</summary>
	public string Repere => string.IsNullOrWhiteSpace(Nom) && string.IsNullOrWhiteSpace(Age)
		? "Aucune donnée reçue : revenez à la page principale et lancez le calcul."
		: $"Paramètres reçus du routing : nom = « {Nom} », âge = {Age} ans.";

	public AsyncRelayCommand RetourCommand { get; }

	/// <summary>
	/// Appelé par le <see cref="Shell"/> dès que les paramètres de l'URL sont
	/// connus.
	/// </summary>
	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		// Les propriétés [QueryProperty] ont déjà été remplies : il ne reste qu'à
		// reconstruire le message qui en dépend.
		OnPropertyChanged(nameof(Repere));
		OnPropertyChanged(nameof(AgeLibelle));
	}

	/// <summary>À appeler à chaque affichage de la page.</summary>
	public void Rafraichir()
	{
		OnPropertyChanged(nameof(Repere));
		OnPropertyChanged(nameof(AgeLibelle));
	}
}