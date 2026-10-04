namespace CalculateurAge;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCalculerClicked(object? sender, EventArgs e)
	{
		// Validation : on refuse un nom vide.
		if (string.IsNullOrWhiteSpace(entryNom.Text))
		{
			await DisplayAlert("Erreur", "Entrez un nom", "OK");

			return; // on sort sans rien calculer
		}

		DateTime d = pickerDate.Date;

		int age = DateTime.Today.Year - d.Year;

		// Si l'anniversaire n'est pas encore passé cette année, on retire une année.
		if (d.Date > DateTime.Today.AddYears(-age))
		{
			age--;
		}

		// Phase B : le résultat est transporté dans l'URL de navigation.
		string nom = Uri.EscapeDataString(entryNom.Text);

		await Shell.Current.GoToAsync($"{nameof(Views.ResultatPage)}?nom={nom}&age={age}");
	}
}