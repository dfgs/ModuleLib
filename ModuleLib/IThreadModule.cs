namespace ModuleLib
{
	public interface IThreadModule:IModule
	{
		ModuleStates State
		{
			get;
		}


		bool Start();
		bool Stop();

	}
}