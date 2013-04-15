using System;
using System.Collections.Generic;
using System.Linq;

namespace nhsevidence.ScreeningScheduling.Core
{
	public class Container
	{
		IList<Task> tasks = new List<Task> ();

		public void AddTasks (string[] names)
		{
			foreach (var name in names) {
				AddTask (name);
			}
		}

		public void AddTask (string name)
		{
			if (tasks.Any (t => t.Name == name))
				throw new Exception ("Could not add task because of existing task with same name");
			if (string.IsNullOrEmpty (name))
				throw new Exception ("Could not add task because name cannot be empty");

			tasks.Add (new Task (name));
		}

		public void AddDependencies (string taskName, string dependsOnName)
		{
			if (taskName == dependsOnName)
				throw new Exception (string.Format ("Error adding dependency: Task and dependency were the same value of '{0}'", dependsOnName));
			if (FindTask (taskName) == null)
				throw new Exception (string.Format ("Error adding dependency: task named '{0}' not found", taskName));
			if (FindTask (dependsOnName) == null)
				throw new Exception (string.Format ("Error adding dependency: task named '{0}' not found", dependsOnName));

			var task = FindTask (taskName);
			var dependentOn = FindTask (dependsOnName);

			// Check this isn't cyclical
			if (!IsResolvable (task, dependentOn))
				throw new CyclicalDependencyException (string.Format ("While adding dependency {0} => {1} cyclic dependency ", taskName, dependsOnName));

			if (!task.Dependencies.Contains (dependentOn))
				task.Dependencies.Add (dependentOn);
		}

		public IList<Task> Resolve ()
		{
			return Resolve (tasks);
		}

		Task FindTask (string name)
		{
			return tasks.Single (t => t.Name == name);
		}


		IList<Task> Resolve (IList<Task> tasks)
		{
			IList<Task> resolved = new List<Task> ();
			foreach (var task in tasks) {
				if (task.Dependencies.Count == 0) {
					if (!resolved.Contains (task))
						resolved.Add (task);
				} else {
					var resolvedDeps = Resolve (task.Dependencies);
					foreach (var t in resolvedDeps) {
						if (!resolved.Contains (t))
							resolved.Add (t);
					}
					if (!resolved.Contains (task))
						resolved.Add (task);
				}
			}
			return resolved;
		}

		bool IsResolvable (Task task, Task dependsOn)
		{
			if (task == dependsOn)
				return false;

			// Check that dependsOn never needs task
			foreach (var t in dependsOn.Dependencies) {
				if (!IsResolvable (task, t))
					return false;
			}

			return true;
		}
	}
}

