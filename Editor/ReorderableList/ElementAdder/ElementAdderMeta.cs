namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.ElementAdder
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.Contracts;

	public static class ElementAdderMeta
	{
		private static readonly Dictionary<Type, Dictionary<Type, List<Type>>> contextMap =
			new Dictionary<Type, Dictionary<Type, List<Type>>>();

		private static readonly Dictionary<Type, Type[]> concreteElementTypes = new Dictionary<Type, Type[]>();

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public static Type[] GetMenuCommandTypes<TContext>(Type contractType)
		{
			if (contractType == null)
			{
				throw new ArgumentNullException("contractType");
			}

			Dictionary<Type, List<Type>> contractMap;
			List<Type> commandTypes;
			if (contextMap.TryGetValue(typeof(TContext), out contractMap))
			{
				if (contractMap.TryGetValue(contractType, out commandTypes))
				{
					return commandTypes.ToArray();
				}
			}
			else
			{
				contractMap = new Dictionary<Type, List<Type>>();
				contextMap[typeof(TContext)] = contractMap;
			}

			commandTypes = new List<Type>();

			foreach (var commandType in GetMenuCommandTypes<TContext>())
			{
				var attributes =
					(ElementAdderMenuCommandAttribute[]) Attribute.GetCustomAttributes(commandType,
						typeof(ElementAdderMenuCommandAttribute));
				if (attributes.All(a => a.ContractType != contractType))
				{
					continue;
				}

				commandTypes.Add(commandType);
			}

			contractMap[contractType] = commandTypes;
			return commandTypes.ToArray();
		}

		public static IElementAdderMenuCommand<TContext>[] GetMenuCommands<TContext>(Type contractType)
		{
			var commandTypes = GetMenuCommandTypes<TContext>(contractType);
			var commands = new IElementAdderMenuCommand<TContext>[commandTypes.Length];
			for (int i = 0; i < commandTypes.Length; ++i)
			{
				commands[i] = (IElementAdderMenuCommand<TContext>) Activator.CreateInstance(commandTypes[i]);
			}

			return commands;
		}

		public static Type[] GetConcreteElementTypes(Type contractType, Func<Type, bool>[] filters)
		{
			return
			(from concreteType in GetConcreteElementTypesHelper(contractType)
				where IsTypeIncluded(concreteType, filters)
				select concreteType
			).ToArray();
		}

		public static Type[] GetConcreteElementTypes(Type contractType)
		{
			return GetConcreteElementTypesHelper(contractType).ToArray();
		}

		// -------------------------------------------------------------------
		// Private
		// -------------------------------------------------------------------
		private static IEnumerable<Type> GetMenuCommandTypes<TContext>()
		{
			return
				from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in assembly.GetTypes()
				where assemblyType.IsClass && !assemblyType.IsAbstract &&
				      assemblyType.IsDefined(typeof(ElementAdderMenuCommandAttribute), false)
				where typeof(IElementAdderMenuCommand<TContext>).IsAssignableFrom(assemblyType)
				select assemblyType;
		}

		private static IEnumerable<Type> GetConcreteElementTypesHelper(Type contractType)
		{
			if (contractType == null)
			{
				throw new ArgumentNullException("contractType");
			}

			Type[] concreteTypes;
			if (!concreteElementTypes.TryGetValue(contractType, out concreteTypes))
			{
				concreteTypes =
				(from assembly in AppDomain.CurrentDomain.GetAssemblies()
					from assemblyType in assembly.GetTypes()
					where assemblyType.IsClass && !assemblyType.IsAbstract && contractType.IsAssignableFrom(assemblyType)
					orderby assemblyType.Name
					select assemblyType
				).ToArray();
				concreteElementTypes[contractType] = concreteTypes;
			}

			return concreteTypes;
		}

		private static bool IsTypeIncluded(Type concreteType, Func<Type, bool>[] filters)
		{
			if (filters == null)
			{
				return true;
			}

			foreach (var filter in filters)
			{
				if (!filter(concreteType))
				{
					return false;
				}
			}

			return true;
		}
	}
}