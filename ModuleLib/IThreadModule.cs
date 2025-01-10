namespace ModuleLib
{
	public interface IThreadModule:IStartStopModule
	{
		ModuleStates State
		{
			get;
		}

	}
}