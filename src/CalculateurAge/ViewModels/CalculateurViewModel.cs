using CalculateurAge.Services;
using CalculateurAge.Views;
using System.Globalization;

namespace CalculateurAge.ViewModels;

/// <summary>
/// Contient l'état de l'écran et les actions possibles. Ni Label, ni Entry, ni
/// Button, ni DisplayAlert : si un de ces mots apparaît ici, ce n'est pas du
/// MVVM.
/// </summary>
public sealed class CalculateurViewModel : BaseViewModel
{
	private readonly INavigationService _navigation;

	private string _nom = string.Empty;
	private DateTime _dateNaissance = DateTime.Today.AddYears(-20);
	private string _resultat = string.Empty;
	private bool _resultatVisible;
	private int _age;

	public CalculateurViewModel(INavigationService navigation)
	{
		_navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

		// Le bouton Calculer se grise tant que le nom est vide, et se réactive dès
		// la première lettre : CanExecute est réévalué par Rafraichir.
		CalculerCommand = new RelayCommand(Calculer, () => !string.IsNullOrWhiteSpace(Nom));

		// Ouvre la page de résultat en passant le nom et l'âge en paramètres
		// d'URL (routing du Shell).
		FicheCommand = new AsyncRelayCommand(OuvrirFicheAsync, () => ResultatVisible);
	}

	/// <summary>Nom saisi, en lecture depuis l'Entry en mode TwoWay.</summary>
	public string Nom
	{
		get => _nom;
		set
		{
			if (SetField(ref _nom, value))
			{
				// Sans cet appel, CalculerCommand ne repose jamais sa question.
				CalculerCommand.Rafraichir();
			}
		}
	}

	/// <summary>Date de naissance choisie dans le DatePicker.</summary>
	public DateTime DateNaissance
	{
		get => _dateNaissance;
		set => SetField(ref _dateNaissance, value);
	}

	/// <summary>Phrase affichée à l'écran, calculée par la commande.</summary>
	public string Resultat
	{
		get => _resultat;
		private set => SetField(ref _resultat, value);
	}

	/// <summary>Commande l'affichage du résultat.</summary>
	public bool ResultatVisible
	{
		get => _resultatVisible;
		private set
		{
			if (SetField(ref _resultatVisible, value))
			{
				FicheCommand.Rafraichir();
			}
		}
	}

	/// <summary>Âge calculé, en années pleines.</summary>
	public int Age
	{
		get => _age;
		private set => SetField(ref _age, value);
	}

	public RelayCommand CalculerCommand { get; }

	public AsyncRelayCommand FicheCommand { get; }

	// ------------------------------------------------------------------
	// Logique métier
	// ------------------------------------------------------------------

	private void Calculer()
	{
		Age = CalculerAge(DateNaissance);
		Resultat = $"{Nom}, vous avez {Age} ans";
		ResultatVisible = true;
	}

	private async Task OuvrirFicheAsync()
	{
		string nom = Uri.EscapeDataString(Nom);
		string age = Uri.EscapeDataString(Age.ToString(CultureInfo.InvariantCulture));

		await _navigation.AllerAsync($"{nameof(ResultatPage)}?nom={nom}&age={age}");
	}

	/// <summary>
	/// Âge en années pleines. Si l'anniversaire n'est pas encore passé cette
	/// année, on retire une année.
	/// </summary>
	private static int CalculerAge(DateTime dateNaissance)
	{
		int age = DateTime.Today.Year - dateNaissance.Year;

		if (dateNaissance.Date > DateTime.Today.AddYears(-age))
		{
			age--;
		}

		return age;
	}
}