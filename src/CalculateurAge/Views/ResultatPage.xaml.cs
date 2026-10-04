using CalculateurAge.ViewModels;
using Microsoft.Maui.Controls;

namespace CalculateurAge.Views;

public partial class ResultatPage : ContentPage, IQueryAttributable
{
	private readonly ResultatPageViewModel _viewModel;

	public ResultatPage(ResultatPageViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	/// <summary>
	/// Le Shell dépose ici les paramètres de l'URL de navigation. La page se
	/// contente de les transmettre à son ViewModel : l'analyse des paramètres et
	/// la mise en forme restent dans le ViewModel.
	/// </summary>
	public void ApplyQueryAttributes(IDictionary<string, object> query) =>
		_viewModel.ApplyQueryAttributes(query);

	protected override void OnAppearing()
	{
		base.OnAppearing();

		// Appelé à chaque affichage de la page, et non dans le constructeur.
		_viewModel.Rafraichir();
	}
}