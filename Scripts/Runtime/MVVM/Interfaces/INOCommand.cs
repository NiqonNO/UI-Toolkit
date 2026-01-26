using System;

namespace NiqonNO.UI.MVVM
{
	public interface INOCommand
	{
		void Execute();
		bool CanExecute { get; }
		event Action CanExecuteChanged;
	}
}