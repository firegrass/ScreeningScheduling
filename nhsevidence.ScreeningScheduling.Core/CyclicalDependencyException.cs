using System;

namespace nhsevidence.ScreeningScheduling.Core
{
	public class CyclicalDependencyException : Exception
	{
		public CyclicalDependencyException () : base ()
		{
		}

		public CyclicalDependencyException (string message) : base (message)
		{
		}
	}
}

