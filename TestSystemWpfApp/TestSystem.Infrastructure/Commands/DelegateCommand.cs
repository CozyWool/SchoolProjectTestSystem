namespace TestSystem.Infrastructure.Commands;

public sealed class DelegateCommand : Command
{
    private static readonly Func<object, bool> DefaultCanExecute = _ => true;
    private readonly Func<object, bool> _canExecuteFunc;
    private readonly Action<object> _executeAction;

    public DelegateCommand(Action<object> executeAction) : this(executeAction, DefaultCanExecute)
    {
    }

    public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
    {
        _executeAction = executeAction;
        _canExecuteFunc = canExecuteAction;
    }

    protected override void ExecuteCmd(object parameter)
    {
        _executeAction(parameter);
    }

    protected override bool CanExecuteCmd(object parameter)
    {
        return _canExecuteFunc(parameter);
    }
}