namespace CalculateurAge.Views;

/// <summary>
/// Relie les paramètres "nom" et "age" de l'URL de navigation aux propriétés
/// homonymes. L'attribut se pose sur la classe, pas sur les propriétés.
/// </summary>
[QueryProperty(nameof(Nom), "nom")]
[QueryProperty(nameof(Age), "age")]
public partial class ResultatPage : ContentPage, IQueryAttributable
{
	// Ces propriétés sont remplies par la navigation.
	public string Nom { get; set; } = string.Empty;

	public string Age { get; set; } = string.Empty;

	public ResultatPage()
	{
		InitializeComponent();
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		// Les propriétés ci-dessus sont déjà remplies par le Shell.
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		// Appelé à chaque affichage de la page : le message n'est pas construit
		// dans le constructeur.
		lblMessage.Text = $"{Nom}, vous avez {Age} ans";
	}

	private async void OnRetourClicked(object? sender, EventArgs e) =>
		await Shell.Current.GoToAsync("..");
}