using System.Windows.Input;

namespace CalculateurAge.ViewModels;

/// <summary>
/// Variante asynchrone de <see cref="RelayCommand"/>, indispensable pour la
/// navigation : <c>Shell.GoToAsync</c> retourne une <see cref="Task"/> et un
/// <see cref="Action"/> ne peut pas l'attendre. Le corps est encapsulé dans un
/// <c>try/catch</c> : le <c>async void</c> imposé par <see cref="ICommand"/>
/// reste ainsi sans risque, ce qu'un <c>async void</c> écrit directement dans
/// le code-behind ne serait pas.
/// </summary>
public class AsyncRelayCommand : ICommand
{
	private readonly Func<Task> _executerAsync;
	private readonly Func<bool>? _peutExecuter;

	public AsyncRelayCommand(Func<Task> executerAsync, Func<bool>? peutExecuter = null)
	{
		_executerAsync = executerAsync ?? throw new ArgumentNullException(nameof(executerAsync));
		_peutExecuter = peutExecuter;
	}

	public bool CanExecute(object? parameter) => _peutExecuter?.Invoke() ?? true;

	public async void Execute(object? parameter)
	{
		try
		{
			await _executerAsync();
		}
		catch (Exception)
		{
			// Une navigation refusée (shell absent, route déjà affichée) ne doit
			// pas faire tomber l'application.
		}
	}

	public event EventHandler? CanExecuteChanged;

	/// <summary>À appeler pour forcer les boutons liés à réévaluer leur état.</summary>
	public void Rafraichir() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}