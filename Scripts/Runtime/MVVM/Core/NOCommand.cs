using System;

namespace NiqonNO.UI.MVVM
{
	public sealed class NOCommand : INOCommand
	{
		readonly Action _execute;
		readonly Func<bool> _canExecute;

		public bool CanExecute => _canExecute?.Invoke() ?? true;
		public event Action CanExecuteChanged;

		public NOCommand(Action execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public void Execute()
		{
			if (CanExecute) _execute();
		}
	}
}