namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.ElementAdder
{
	
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class ElementAdderMenuCommandAttribute : Attribute
	{

		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		public ElementAdderMenuCommandAttribute(Type contractType)
		{
			this.ContractType = contractType;
		}

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public Type ContractType { get; private set; }
	}

}
