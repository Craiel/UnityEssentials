namespace Craiel.UnityEssentials.Editor.ReorderableList.Contracts
{
	using System;

	public interface IElementAdderMenuBuilder<TContext>
	{
		void SetContractType(Type contractType);
		
		void SetElementAdder(IElementAdder<TContext> elementAdder);
		
		void SetTypeDisplayNameFormatter(Func<Type, string> formatter);
		
		void AddTypeFilter(Func<Type, bool> typeFilter);
		
		void AddCustomCommand(IElementAdderMenuCommand<TContext> command);
		
		IElementAdderMenu GetMenu();
	}
}