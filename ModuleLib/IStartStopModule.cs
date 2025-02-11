using ResultTypeLib;

namespace ModuleLib
{
	public interface IStartStopModule:IModule
	{
		

		IResult<bool> Start();
		IResult<bool> Stop();

	}
}