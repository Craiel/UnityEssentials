namespace Craiel.UnityEssentials.Editor.ReorderableList.ElementAdder
{
	using System;
	using Contracts;

	public static class ElementAdderMenuBuilder
	{
		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public static IElementAdderMenuBuilder<TContext> For<TContext>()
		{
			return new GenericElementAdderMenuBuilder<TContext>();
		}

		public static IElementAdderMenuBuilder<TContext> For<TContext>(Type contractType)
		{
			var builder = For<TContext>();
			builder.SetContractType(contractType);
			return builder;
		}
	}
}