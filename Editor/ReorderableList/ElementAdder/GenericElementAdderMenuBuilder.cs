namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.ElementAdder
{

	using System;
	using System.Collections.Generic;
	using Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.Contracts;
	using UnityEditor;
	using UnityEngine;

	internal sealed class GenericElementAdderMenuBuilder<TContext> : IElementAdderMenuBuilder<TContext>
	{
		private readonly List<Func<Type, bool>> typeFilters;
		private readonly List<IElementAdderMenuCommand<TContext>> customCommands;

		private Type contractType;
		private IElementAdder<TContext> elementAdder;
		private Func<Type, string> typeDisplayNameFormatter;

		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		public GenericElementAdderMenuBuilder()
		{
			this.typeFilters = new List<Func<Type, bool>>();
			this.customCommands = new List<IElementAdderMenuCommand<TContext>>();
			
			this.typeDisplayNameFormatter = NicifyTypeName;
		}

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public void SetContractType(Type newType)
		{
			this.contractType = newType;
		}

		public void SetElementAdder(IElementAdder<TContext> adder)
		{
			this.elementAdder = adder;
		}

		public void SetTypeDisplayNameFormatter(Func<Type, string> formatter)
		{
			this.typeDisplayNameFormatter = formatter ?? NicifyTypeName;
		}

		public void AddTypeFilter(Func<Type, bool> typeFilter)
		{
			if (typeFilter == null)
			{
				throw new ArgumentNullException("typeFilter");
			}

			this.typeFilters.Add(typeFilter);
		}

		public void AddCustomCommand(IElementAdderMenuCommand<TContext> command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			this.customCommands.Add(command);
		}

		public IElementAdderMenu GetMenu()
		{
			var menu = new GenericElementAdderMenu();

			this.AddCommandsToMenu(menu, this.customCommands);

			if (this.contractType != null)
			{
				this.AddCommandsToMenu(menu, ElementAdderMeta.GetMenuCommands<TContext>(this.contractType));
				this.AddConcreteTypesToMenu(menu,
					ElementAdderMeta.GetConcreteElementTypes(this.contractType, this.typeFilters.ToArray()));
			}

			return menu;
		}
		
		// -------------------------------------------------------------------
		// Private
		// -------------------------------------------------------------------
		private static string NicifyTypeName(Type type)
		{
			return ObjectNames.NicifyVariableName(type.Name);
		}

		private void AddCommandsToMenu(GenericElementAdderMenu menu, IList<IElementAdderMenuCommand<TContext>> commands)
		{
			if (commands.Count == 0)
			{
				return;
			}

			if (!menu.IsEmpty)
			{
				menu.AddSeparator();
			}

			foreach (var command in commands)
			{
				if (this.elementAdder != null && command.CanExecute(this.elementAdder))
				{
					var closure = command;
					menu.AddItem(command.Content, () => closure.Execute(this.elementAdder));
				}
				else
				{
					menu.AddDisabledItem(command.Content);
				}
			}
		}

		private void AddConcreteTypesToMenu(GenericElementAdderMenu menu, Type[] concreteTypes)
		{
			if (concreteTypes.Length == 0)
			{
				return;
			}

			if (!menu.IsEmpty)
			{
				menu.AddSeparator();
			}

			foreach (var concreteType in concreteTypes)
			{
				var content = new GUIContent(this.typeDisplayNameFormatter(concreteType));
				if (this.elementAdder != null && this.elementAdder.CanAddElement(concreteType))
				{
					menu.AddItem(content, () =>
					{
						if (this.elementAdder.CanAddElement(concreteType))
						{
							this.elementAdder.AddElement(concreteType);
						}
					});
				}
				else
				{
					menu.AddDisabledItem(content);
				}
			}
		}
	}
}
