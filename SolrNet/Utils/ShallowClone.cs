using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SolrNet.Utils
{
	/// <summary>
	/// Class for performing a simple/shallow object clone.
	/// </summary>
	public static class ShallowClone
	{

		/// <summary>
		/// Performs a shallow clone.
		/// </summary>
		/// <param name="copyFromObject">Source object</param>
		/// <typeparam name="T"></typeparam>
		/// <returns>A copy of the source</returns>
		public static T Clone<T>(T copyFromObject) {
			var copyToObject = Activator.CreateInstance<T>();

			foreach (PropertyInfo sourcePropertyInfo in copyFromObject.GetType().GetProperties())
			{
				PropertyInfo destPropertyInfo = copyToObject.GetType().GetProperty(sourcePropertyInfo.Name);

				destPropertyInfo.SetValue(
						copyToObject,
						sourcePropertyInfo.GetValue(copyFromObject, null),
						null);
			}

			return copyToObject;
		}
	}
}
