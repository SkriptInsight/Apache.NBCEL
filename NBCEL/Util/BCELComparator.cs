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

namespace Apache.NBCEL.Util
{
	/// <summary>Used for BCEL comparison strategy</summary>
	/// <since>5.2</since>
	public interface BCELComparator
    {
	    /// <summary>Compare two objects and return what THIS.equals(THAT) should return</summary>
	    /// <param name="THIS" />
	    /// <param name="THAT" />
	    /// <returns>true if and only if THIS equals THAT</returns>
	    bool Equals(object THIS, object THAT);

	    /// <summary>Return hashcode for THIS.hashCode()</summary>
	    /// <param name="THIS" />
	    /// <returns>hashcode for THIS.hashCode()</returns>
	    int HashCode(object THIS);
    }
}