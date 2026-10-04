using CalculateurAge.ViewModels;

namespace CalculateurAge;

public partial class MainPage : ContentPage
{
	public MainPage(CalculateurViewModel viewModel)
	{
		InitializeComponent();

		// Objet dans lequel tous les {Binding} de la page vont chercher leurs
		// valeurs. Tout le reste du code-behind est supprimé.
		BindingContext = viewModel;
	}
}