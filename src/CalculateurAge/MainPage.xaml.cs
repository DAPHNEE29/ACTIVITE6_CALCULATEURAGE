namespace CalculateurAge;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCalculerClicked(object? sender, EventArgs e)
	{
		// Validation : on refuse un nom vide.
		if (string.IsNullOrWhiteSpace(entryNom.Text))
		{
			DisplayAlert("Erreur", "Entrez un nom", "OK");

			return; // on sort sans rien calculer
		}

		DateTime d = pickerDate.Date;

		int age = DateTime.Today.Year - d.Year;

		// Si l'anniversaire n'est pas encore passé cette année, on retire une année.
		if (d.Date > DateTime.Today.AddYears(-age))
		{
			age--;
		}

		// On écrit DIRECTEMENT dans les contrôles : c'est précisément ce que le
		// MVVM va supprimer.
		lblResultat.Text = $"{entryNom.Text}, vous avez {age} ans";
		lblResultat.IsVisible = true;
	}
}