namespace ModuleLib
{
	public interface IStartStopModule:IModule
	{
		ModuleStates State
		{
			get;
		}


		IResult<bool> Start();
		IResult<bool> Stop();

	}
}