using System;
using System.Collections.Generic;

namespace nhsevidence.ScreeningScheduling.Core
{
	public class Task
	{
		public Task (string name)
		{
			Name = name;
			Dependencies = new List<Task> ();
		}

		public string Name { get; set; }

		public IList<Task> Dependencies { get; set; }
	}
}