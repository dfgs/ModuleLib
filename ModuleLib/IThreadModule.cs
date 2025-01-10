namespace ModuleLib
{
	public interface IThreadModule:IModule
	{
		ModuleStates State
		{
			get;
		}


		IResult<bool> Start();
		IResult<bool> Stop();

	}
}