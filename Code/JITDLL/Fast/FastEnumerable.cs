using UnityEngine;
using System.Collections;

namespace FastGeneric
{
	public interface FastEnumerable<T>
	{
		void Enumerate (FastList<T> output);
	}
}