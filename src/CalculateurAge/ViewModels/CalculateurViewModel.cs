using CalculateurAge.Services;
using CalculateurAge.Views;
using System.Globalization;

namespace CalculateurAge.ViewModels;

/// <summary>
/// Contient l'état de l'écran et les actions possibles. Ni <c>Label</c>, ni
/// <c>Entry</c>, ni <c>Button</c>, ni <c>DisplayAlert</c> : si un de ces mots
/// apparaît ici, ce n'est pas du MVVM.
/// </summary>
public sealed class CalculateurViewModel : BaseViewModel
{
	private const int EtapeFormulaire = 1;
	private const int EtapeResultat = 2;

	private readonly INavigationService _navigation;

	private string _nom = string.Empty;
	private DateTime _dateNaissance = DateTime.Today;
	private string _resultat = string.Empty;
	private bool _resultatVisible;
	private string _message = string.Empty;
	private bool _messageVisible;
	private string _champsRestants = string.Empty;
	private bool _champsRestantsVisible;
	private int _age;
	private int _etape = EtapeFormulaire;

	public CalculateurViewModel(INavigationService navigation)
	{
		_navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

		// Le bouton Calculer se grise tant qu'un champ manque, et se réactive dès
		// que la saisie est complète : CanExecute est réévalué par Rafraichir.
		CalculerCommand = new RelayCommand(Calculer, () => FormulaireValide);

		// Activité 6 : une commande écrit dans la propriété Message, affichée par
		// un Label. Aucune ligne d'interface dans ce fichier.
		MessageCommand = new RelayCommand(AfficherMessage);

		// Activité 6 : remise à zéro de tous les champs.
		ResetCommand = new RelayCommand(
			Reinitialiser,
			() => ResultatVisible || MessageVisible || ChampsRestantsVisible);

		// Activité 6 : affiche le nom des champs restants, puis passe à l'étape
		// suivante lorsque plus rien ne manque.
		SuivantCommand = new AsyncRelayCommand(SuivantAsync);

		// Ouvre la page de résultat en passant le nom et l'âge en paramètres
		// d'URL (routing du Shell).
		FicheCommand = new AsyncRelayCommand(OuvrirFicheAsync, () => ResultatVisible);
	}

	// ------------------------------------------------------------------
	// Champs privés : la vraie donnée.
	// Propriétés publiques : ce que le XAML voit.
	// ------------------------------------------------------------------

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

	/// <summary>
	/// Date de naissance choisie dans le DatePicker. Tant qu'elle vaut aujourd'hui
	/// ou est dans le futur, le champ est considéré comme non renseigné.
	/// </summary>
	public DateTime DateNaissance
	{
		get => _dateNaissance;
		set
		{
			if (SetField(ref _dateNaissance, value))
			{
				CalculerCommand.Rafraichir();
			}
		}
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
				RafraichirCommandes();
			}
		}
	}

	/// <summary>
	/// Message d'aide, produit par <see cref="MessageCommand"/> et affiché par un
	/// simple Label lié à cette propriété.
	/// </summary>
	public string Message
	{
		get => _message;
		private set => SetField(ref _message, value);
	}

	/// <summary>Commande l'affichage du message.</summary>
	public bool MessageVisible
	{
		get => _messageVisible;
		private set
		{
			if (SetField(ref _messageVisible, value))
			{
				RafraichirCommandes();
			}
		}
	}

	/// <summary>
	/// Nom des champs qui manquent encore, écrit par <see cref="SuivantCommand"/>
	/// juste avant de tenter l'étape suivante.
	/// </summary>
	public string ChampsRestants
	{
		get => _champsRestants;
		private set => SetField(ref _champsRestants, value);
	}

	/// <summary>Commande l'affichage de la liste des champs restants.</summary>
	public bool ChampsRestantsVisible
	{
		get => _champsRestantsVisible;
		private set
		{
			if (SetField(ref _champsRestantsVisible, value))
			{
				ResetCommand.Rafraichir();
			}
		}
	}

	/// <summary>Âge calculé, en années pleines.</summary>
	public int Age
	{
		get => _age;
		private set => SetField(ref _age, value);
	}

	/// <summary>Numéro de l'étape courante.</summary>
	public int Etape
	{
		get => _etape;
		private set
		{
			if (SetField(ref _etape, value))
			{
				OnPropertyChanged(nameof(Progression));
			}
		}
	}

	/// <summary>Intitulé de l'étape courante, affiché en haut du formulaire.</summary>
	public string Progression => Etape == EtapeResultat
		? $"Étape {Etape} sur 2 — Résultat"
		: $"Étape {Etape} sur 2 — Saisie";

	public RelayCommand CalculerCommand { get; }

	public RelayCommand MessageCommand { get; }

	public RelayCommand ResetCommand { get; }

	public AsyncRelayCommand SuivantCommand { get; }

	public AsyncRelayCommand FicheCommand { get; }

	/// <summary>Vrai quand les deux champs du formulaire sont exploitables.</summary>
	private bool FormulaireValide =>
		!string.IsNullOrWhiteSpace(Nom) && DateNaissance.Date < DateTime.Today;

	// ------------------------------------------------------------------
	// Logique métier
	// ------------------------------------------------------------------

	private void Calculer()
	{
		if (!FormulaireValide)
		{
			Message = $"Complétez avant de calculer : {string.Join(", ", GetChampsRestants())}.";
			MessageVisible = true;

			return;
		}

		Age = CalculerAge(DateNaissance);
		Resultat = $"{Nom}, vous avez {Age} ans";
		ResultatVisible = true;
	}

	private void AfficherMessage()
	{
		Message = ResultatVisible
			? $"{Nom}, né(e) le {DateNaissance:dd/MM/yyyy}, vous avez {Age} ans aujourd'hui. "
				+ "Le bouton « Fiche détaillée » ouvre la page de résultat via le routing du Shell."
			: "Renseignez votre nom et votre date de naissance, puis appuyez sur Calculer.";

		MessageVisible = true;
	}

	private void Reinitialiser()
	{
		Nom = string.Empty;
		DateNaissance = DateTime.Today;
		Age = 0;
		Etape = EtapeFormulaire;
		Resultat = string.Empty;
		ResultatVisible = false;
		Message = string.Empty;
		MessageVisible = false;
		ChampsRestants = string.Empty;
		ChampsRestantsVisible = false;
	}

	/// <summary>
	/// Affiche le nom des champs restants. Si plus rien ne manque, le calcul est
	/// lancé et l'étape suivante — la page de résultat — est atteinte.
	/// </summary>
	private async Task SuivantAsync()
	{
		IReadOnlyList<string> restants = GetChampsRestants();

		ChampsRestants = restants.Count == 0
			? "Tous les champs sont renseignés."
			: $"Champs restants : {string.Join(", ", restants)}.";

		ChampsRestantsVisible = true;

		if (restants.Count > 0)
		{
			Message = $"Complétez {string.Join(" et ", restants)} avant de passer à l'étape suivante.";
			MessageVisible = true;

			return;
		}

		Calculer();
		Etape = EtapeResultat;

		await OuvrirFicheAsync();
	}

	private async Task OuvrirFicheAsync()
	{
		string nom = Uri.EscapeDataString(Nom);
		string age = Uri.EscapeDataString(Age.ToString(CultureInfo.InvariantCulture));

		await _navigation.AllerAsync($"{nameof(ResultatPage)}?nom={nom}&age={age}");
	}

	/// <summary>Champs du formulaire dont la valeur n'est pas encore exploitable.</summary>
	private IReadOnlyList<string> GetChampsRestants()
	{
		List<string> restants = new(2);

		if (string.IsNullOrWhiteSpace(Nom))
		{
			restants.Add("Nom");
		}

		if (DateNaissance.Date >= DateTime.Today)
		{
			restants.Add("Date de naissance");
		}

		return restants;
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

	/// <summary>Les commandes concernées doivent reposeur la question à CanExecute.</summary>
	private void RafraichirCommandes()
	{
		ResetCommand.Rafraichir();
		FicheCommand.Rafraichir();
	}
}