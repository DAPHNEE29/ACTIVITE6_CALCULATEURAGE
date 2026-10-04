using System.Windows.Input;

namespace CalculateurAge.ViewModels;

/// <summary>
/// Transforme une méthode en objet liable à un <c>Button.Command</c>.
/// <see cref="CanExecute"/> répond à la question « cette action est-elle
/// possible ? » : le Button interroge cette méthode et se désactive tout seul
/// quand la réponse est non. On n'écrit donc jamais <c>btn.IsEnabled</c> à la
/// main. <see cref="Rafraichir"/> sert à forcer le bouton à reposer la question.
/// </summary>
public class RelayCommand : ICommand
{
	private readonly Action _executer;
	private readonly Func<bool>? _peutExecuter;

	public RelayCommand(Action executer, Func<bool>? peutExecuter = null)
	{
		_executer = executer ?? throw new ArgumentNullException(nameof(executer));
		_peutExecuter = peutExecuter;
	}

	public bool CanExecute(object? parameter) => _peutExecuter?.Invoke() ?? true;

	public void Execute(object? parameter) => _executer();

	public event EventHandler? CanExecuteChanged;

	/// <summary>À appeler pour forcer les boutons liés à réévaluer leur état.</summary>
	public void Rafraichir() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}