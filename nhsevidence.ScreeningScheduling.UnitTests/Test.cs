using System;
using NUnit.Framework;
using nhsevidence.ScreeningScheduling.Core;

namespace nhsevidence.ScreeningScheduling.UnitTests
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void Case1 ()
		{
			// tasks : []
			// dependencies: []
			// result: []

			var container = new Container ();
			var tasks = container.Resolve ();
			Assert.AreEqual (0, tasks.Count);
		}

		[Test]
		public void Case2 ()
		{
			// tasks: [a,b]
			// dependencies: []
			// result: [a,b]

			var tasks = new string[] { "a", "b" };

			var container = new Container ();
			container.AddTasks (tasks);

			var resolvedTasks = container.Resolve ();
			Assert.AreEqual ("a", resolvedTasks[0].Name);
			Assert.AreEqual ("b", resolvedTasks[1].Name);
		}

		[Test]
		public void Case3 ()
		{
			// tasks: [a,b]
			// dependencies: [a => b]
			// result: [b,a]

			var tasks = new string[] { "a", "b" };

			var container = new Container ();
			container.AddTasks (tasks);
			container.AddDependencies ("a", "b");

			var resolvedTasks = container.Resolve ();

			Assert.AreEqual ("b", resolvedTasks[0].Name);
			Assert.AreEqual ("a", resolvedTasks[1].Name);
		}

		[Test]
		public void Case4 ()
		{
			// tasks: [a,b,c,d]
			// dependencies: [a => b,c => d]
			// result: [b,a,d,c]

			var tasks = new string[] { "a", "b", "c", "d" };

			var container = new Container ();
			container.AddTasks (tasks);
			container.AddDependencies ("a", "b");
			container.AddDependencies ("c", "d");

			var resolvedTasks = container.Resolve ();

			Assert.AreEqual ("b", resolvedTasks[0].Name);
			Assert.AreEqual ("a", resolvedTasks[1].Name);
			Assert.AreEqual ("d", resolvedTasks[2].Name);
			Assert.AreEqual ("c", resolvedTasks[3].Name);
		}

		[Test]
		public void Case5 ()
		{
			// tasks: [a,b,c]
			// dependencies: [a => b,b => c]
			// result: [c,b,a]

			var tasks = new string[] { "a", "b", "c" };

			var container = new Container ();
			container.AddTasks (tasks);
			container.AddDependencies ("a", "b");
			container.AddDependencies ("b", "c");

			var resolvedTasks = container.Resolve ();

			Assert.AreEqual ("c", resolvedTasks[0].Name);
			Assert.AreEqual ("b", resolvedTasks[1].Name);
			Assert.AreEqual ("a", resolvedTasks[2].Name);
		}

		[Test]
		[ExpectedException (typeof(CyclicalDependencyException))]
		public void Case6 ()
		{
			// tasks: [a,b,c,d]
			// dependencies: [a => b,b => c,c => a]
			// result: Error - this is a cyclic dependency

			var tasks = new string[] { "a", "b", "c", "d" };

			var container = new Container ();
			container.AddTasks (tasks);
			container.AddDependencies ("a", "b");
			container.AddDependencies ("b", "c");
			container.AddDependencies ("c", "a");
		}

		#region AdditionalTestCase

		[Test]
		public void Duplication ()
		{
			//Tasks: a,b,c,f,r,s
			//Dependencies: a=>b,b=>c,r=>s,s=>b,b=>f,f=>c
			//Result: c,f,b,a,s,r

			var tasks = new string[] { "a", "b", "c", "f", "r", "s" };

			var container = new Container ();
			container.AddTasks (tasks);
			container.AddDependencies ("a", "b");
			container.AddDependencies ("b", "c");
			container.AddDependencies ("r", "s");
			container.AddDependencies ("s", "b");
			container.AddDependencies ("b", "f");
			container.AddDependencies ("f", "c");

			var resolvedTasks = container.Resolve ();

			Assert.AreEqual ("c", resolvedTasks[0].Name);
			Assert.AreEqual ("f", resolvedTasks[1].Name);
			Assert.AreEqual ("b", resolvedTasks[2].Name);
			Assert.AreEqual ("a", resolvedTasks[3].Name);
			Assert.AreEqual ("s", resolvedTasks[4].Name);
			Assert.AreEqual ("r", resolvedTasks[5].Name);
		}

		#endregion
	}
}

