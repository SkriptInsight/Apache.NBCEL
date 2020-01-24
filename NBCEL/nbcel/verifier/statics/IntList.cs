/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*/
using Sharpen;

namespace NBCEL.verifier.statics
{
	/// <summary>A small utility class representing a set of basic int values.</summary>
	public class IntList
	{
		/// <summary>The int are stored as Integer objects here.</summary>
		private readonly System.Collections.Generic.List<int> theList;

		/// <summary>This constructor creates an empty list.</summary>
		internal IntList()
		{
			theList = new System.Collections.Generic.List<int>();
		}

		/// <summary>Adds an element to the list.</summary>
		internal virtual void Add(int i)
		{
			theList.Add(i);
		}

		/// <summary>Checks if the specified int is already in the list.</summary>
		internal virtual bool Contains(int i)
		{
			int[] ints = new int[theList.Count];
			Sharpen.Collections.ToArray(theList, ints);
			foreach (int k in ints)
			{
				if (i == k)
				{
					return true;
				}
			}
			return false;
		}
	}
}
