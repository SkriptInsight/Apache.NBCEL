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

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Sharpen;

namespace NBCEL.util
{
	/// <summary>Responsible for loading (class) files from the CLASSPATH.</summary>
	/// <remarks>Responsible for loading (class) files from the CLASSPATH. Inspired by sun.tools.ClassPath.
	/// 	</remarks>
	public class ClassPath : java.io.Closeable
	{
		private abstract class AbstractPathEntry : java.io.Closeable
		{
			/// <exception cref="IOException"/>
			internal abstract NBCEL.util.ClassPath.ClassFile GetClassFile(string name, string
				 suffix);

			internal abstract Uri GetResource(string name);

			internal abstract java.io.InputStream GetResourceAsStream(string name);

			public abstract void Close();
			public void Dispose()
			{
				Close();
			}
		}

		private abstract class AbstractZip : NBCEL.util.ClassPath.AbstractPathEntry
		{
			private readonly ZipArchive zipFile;

			internal AbstractZip(ZipArchive zipFile)
			{
				this.zipFile = (zipFile);
			}

			/// <exception cref="IOException"/>
			public override void Close()
			{
				if (zipFile != null)
				{
					zipFile.Dispose();
				}
			}

			/// <exception cref="IOException"/>
			internal override NBCEL.util.ClassPath.ClassFile GetClassFile(string name, string
				 suffix)
			{ 
				ZipArchiveEntry entry = zipFile.GetEntry(ToEntryName(name, suffix));
				if (entry == null)
				{
					return null;
				}
				return new _ClassFile_82(this, entry);
			}

			private sealed class _ClassFile_82 : NBCEL.util.ClassPath.ClassFile
			{
				public _ClassFile_82(AbstractZip _enclosing, ZipArchiveEntry entry)
				{
					this._enclosing = _enclosing;
					this.entry = entry;
				}

				public string GetBase()
				{
					return "";
				}

				/// <exception cref="IOException"/>
				public java.io.InputStream GetInputStream()
				{
					return entry.Open().ReadFully().ToInputStream();
				}

				public string GetPath()
				{
					return entry.ToString();
				}

				public long GetSize()
				{
					return entry.Length;
				}

				public long GetTime()
				{
					return entry.LastWriteTime.ToUnixTimeMilliseconds();
				}

				private readonly AbstractZip _enclosing;

				private readonly ZipArchiveEntry entry;
			}

			internal override System.Uri GetResource(string name)
			{
				ZipArchiveEntry entry = zipFile.GetEntry(name);
				try
				{
					return entry != null ? new System.Uri("jar:file:" + name + "!/" + 
						name) : null;
				}
				catch (Exception)
				{
					return null;
				}
			}

			internal override java.io.InputStream GetResourceAsStream(string name)
			{
				ZipArchiveEntry entry = zipFile.GetEntry(name);
				try
				{
					return entry != null ? entry.Open().ReadFully().ToInputStream() : null;
				}
				catch (System.IO.IOException)
				{
					return null;
				}
			}

			protected internal abstract string ToEntryName(string name, string suffix);

			public override string ToString()
			{
				return zipFile.ToString();
			}
		}

		/// <summary>Contains information about file/ZIP entry of the Java class.</summary>
		public interface ClassFile
		{
			/// <returns>
			/// base path of found class, i.e. class is contained relative to that path, which may either denote a
			/// directory, or zip file
			/// </returns>
			string GetBase();

			/// <returns>input stream for class file.</returns>
			/// <exception cref="IOException"/>
			java.io.InputStream GetInputStream();

			/// <returns>canonical path to class file.</returns>
			string GetPath();

			/// <returns>size of class file.</returns>
			long GetSize();

			/// <returns>modification time of class file.</returns>
			long GetTime();
		}

		private class Dir : NBCEL.util.ClassPath.AbstractPathEntry
		{
			private readonly string dir;

			internal Dir(string d)
			{
				dir = d;
			}

			/// <exception cref="IOException"/>
			public override void Close()
			{
			}

			// Nothing to do
			/// <exception cref="IOException"/>
			internal override NBCEL.util.ClassPath.ClassFile GetClassFile(string name, string
				 suffix)
			{
				System.IO.FileSystemInfo file = new System.IO.FileInfo(dir + System.IO.Path.DirectorySeparatorChar + name.Replace
					('.', System.IO.Path.DirectorySeparatorChar) + suffix);
				return file.Exists ? new _ClassFile_189(this, file) : null;
			}

			private sealed class _ClassFile_189 : NBCEL.util.ClassPath.ClassFile
			{
				public _ClassFile_189(Dir _enclosing, System.IO.FileSystemInfo file)
				{
					this._enclosing = _enclosing;
					this.file = (FileInfo) file;
				}

				public string GetBase()
				{
					return this._enclosing.dir;
				}

				/// <exception cref="IOException"/>
				public java.io.InputStream GetInputStream()
				{
					return File.ReadAllBytes(file.FullName).ToInputStream();
				}

				public string GetPath()
				{
					try
					{
						return file.FullName;
					}
					catch (System.IO.IOException)
					{
						return null;
					}
				}

				public long GetSize()
				{
					return file.Length;
				}

				public long GetTime()
				{
					return (long) (Sharpen.System.Epoch - file.LastWriteTime).TotalMilliseconds;
				}

				private readonly Dir _enclosing;

				private readonly FileInfo file;
			}

			internal override System.Uri GetResource(string name)
			{
				// Resource specification uses '/' whatever the platform
				System.IO.FileSystemInfo file = ToFile(name);
				try
				{
					return file.Exists ? new Uri(file.FullName) : null;
				}
				catch (Exception)
				{
					return null;
				}
			}

			internal override java.io.InputStream GetResourceAsStream(string name)
			{
				// Resource specification uses '/' whatever the platform
				System.IO.FileSystemInfo file = ToFile(name);
				try
				{
					return file.Exists ? File.ReadAllBytes(file.FullName).ToInputStream() : null;
				}
				catch (System.IO.IOException)
				{
					return null;
				}
			}

			private System.IO.FileSystemInfo ToFile(string name)
			{
				return new System.IO.FileInfo(dir + System.IO.Path.DirectorySeparatorChar + name.Replace('/', Path.PathSeparator));
			}

			public override string ToString()
			{
				return dir;
			}
		}

		private class Jar : NBCEL.util.ClassPath.AbstractZip
		{
			internal Jar(ZipArchive zip)
				: base(zip)
			{
			}

			protected internal override string ToEntryName(string name, string suffix)
			{
				return PackageToFolder(name) + suffix;
			}
		}

		private class JrtModule : NBCEL.util.ClassPath.AbstractPathEntry
		{
			private readonly string modulePath;

			public JrtModule(string modulePath)
			{
				this.modulePath = (modulePath);
			}

			/// <exception cref="IOException"/>
			public override void Close()
			{
			}

			// Nothing to do.
			/// <exception cref="IOException"/>
			internal override NBCEL.util.ClassPath.ClassFile GetClassFile(string name, string
				 suffix)
			{
				// java.nio.file.Path resolved = modulePath.Resolve(PackageToFolder(name) + suffix);
				// if (java.nio.file.Files.Exists(resolved))
				// {
					// return new _ClassFile_285(resolved);
				// }
				return null;
			}

			private sealed class _ClassFile_285 : NBCEL.util.ClassPath.ClassFile
			{
				public _ClassFile_285(string resolved)
				{
					this.resolved = resolved;
				}

				public string GetBase()
				{
					return resolved;
				}

				/// <exception cref="IOException"/>
				public java.io.InputStream GetInputStream()
				{
					return File.ReadAllBytes(resolved).ToInputStream();
				}

				public string GetPath()
				{
					return resolved.ToString();
				}

				public long GetSize()
				{
					try
					{
						return new FileInfo(resolved).Length;
					}
					catch (System.IO.IOException)
					{
						return 0;
					}
				}

				public long GetTime()
				{
					try
					{
						return (long) (Sharpen.System.Epoch - new FileInfo(resolved).LastWriteTime).TotalMilliseconds;
					}
					catch (System.IO.IOException)
					{
						return 0;
					}
				}

				private readonly string resolved;
			}

			internal override System.Uri GetResource(string name)
			{
				// java.nio.file.Path resovled = modulePath.Resolve(name);
				// try
				// {
					// return java.nio.file.Files.Exists(resovled) ? new System.Uri("jrt:" + modulePath
						 // + "/" + name) : null;
				// }
				// catch (java.net.MalformedURLException)
				// {
					return null;
				// }
			}

			internal override java.io.InputStream GetResourceAsStream(string name)
			{
				// try
				// {
					// return java.nio.file.Files.NewInputStream(modulePath.Resolve(name));
				// }
				// catch (System.IO.IOException)
				// {
					return null;
				// }
			}

			public override string ToString()
			{
				return modulePath.ToString();
			}
		}

		private class JrtModules : NBCEL.util.ClassPath.AbstractPathEntry
		{

			private readonly NBCEL.util.ClassPath.JrtModule[] modules;

			/// <exception cref="IOException"/>
			public JrtModules(string path)
			{
				this.modules = new NBCEL.util.ClassPath.JrtModule[0];
			}

			/// <exception cref="IOException"/>
			public override void Close()
			{
				if (modules != null)
				{
					// don't use a for each loop to avoid creating an iterator for the GC to collect.
					for (int i = 0; i < modules.Length; i++)
					{
						modules[i].Close();
					}
				}
			}

			/// <exception cref="IOException"/>
			internal override NBCEL.util.ClassPath.ClassFile GetClassFile(string name, string
				 suffix)
			{
				// don't use a for each loop to avoid creating an iterator for the GC to collect.
				for (int i = 0; i < modules.Length; i++)
				{
					NBCEL.util.ClassPath.ClassFile classFile = modules[i].GetClassFile(name, suffix);
					if (classFile != null)
					{
						return classFile;
					}
				}
				return null;
			}

			internal override Uri GetResource(string name)
			{
				// don't use a for each loop to avoid creating an iterator for the GC to collect.
				for (int i = 0; i < modules.Length; i++)
				{
					System.Uri url = modules[i].GetResource(name);
					if (url != null)
					{
						return url;
					}
				}
				return null;
			}

			internal override java.io.InputStream GetResourceAsStream(string name)
			{
				// don't use a for each loop to avoid creating an iterator for the GC to collect.
				for (int i = 0; i < modules.Length; i++)
				{
					java.io.InputStream inputStream = modules[i].GetResourceAsStream(name);
					if (inputStream != null)
					{
						return inputStream;
					}
				}
				return null;
			}

			public override string ToString()
			{
				return Sharpen.Arrays.ToString(modules);
			}
		}

		private class Module : NBCEL.util.ClassPath.AbstractZip
		{
			internal Module(ZipArchive zip)
				: base(zip)
			{
			}

			protected internal override string ToEntryName(string name, string suffix)
			{
				return "classes/" + PackageToFolder(name) + suffix;
			}
		}

		private static readonly Func<FileInfo, string, bool> ARCHIVE_FILTER = (dir
			, name) => 		{
			name = name.ToLower();
			return name.EndsWith(".zip") || name.EndsWith(".jar");
		}
;

		private static readonly Func<FileInfo, string, bool> MODULES_FILTER = (dir
			, name) => 		{
			name = name.ToLower();
			return name.EndsWith(".jmod");
		}
;

		public static readonly NBCEL.util.ClassPath SYSTEM_CLASS_PATH = new NBCEL.util.ClassPath
			(GetClassPath());

		private static void AddJdkModules(string javaHome, System.Collections.Generic.List
			<string> list)
		{
			string modulesPath = Sharpen.Runtime.GetProperty("java.modules.path");
			if (modulesPath == null || (modulesPath.Trim().Length == 0))
			{
				// Default to looking in JAVA_HOME/jmods
				modulesPath = javaHome + System.IO.Path.DirectorySeparatorChar + "jmods";
			}
			DirectoryInfo modulesDir = new DirectoryInfo(modulesPath);
			if (modulesDir.Exists)
			{
				string[] modules = modulesDir.GetFiles().Where(f => MODULES_FILTER.Invoke(f, f.Name)).Select(c => c.FullName).ToArray();
				for (int i = 0; i < modules.Length; i++)
				{
					list.Add(modulesDir.FullName + System.IO.Path.DirectorySeparatorChar + modules[i]);
				}
			}
		}

		/// <summary>
		/// Checks for class path components in the following properties: "java.class.path", "sun.boot.class.path",
		/// "java.ext.dirs"
		/// </summary>
		/// <returns>class path as used by default by BCEL</returns>
		public static string GetClassPath()
		{
			// @since 6.0 no longer final
			string classPathProp = Sharpen.Runtime.GetProperty("java.class.path");
			string bootClassPathProp = Sharpen.Runtime.GetProperty("sun.boot.class.path");
			string extDirs = Sharpen.Runtime.GetProperty("java.ext.dirs");
			// System.out.println("java.version = " + System.getProperty("java.version"));
			// System.out.println("java.class.path = " + classPathProp);
			// System.out.println("sun.boot.class.path=" + bootClassPathProp);
			// System.out.println("java.ext.dirs=" + extDirs);
			string javaHome = Sharpen.Runtime.GetProperty("java.home");
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List
				<string>();
			
			// Starting in JDK 9, .class files are in the jmods directory. Add them to the path.
			AddJdkModules(javaHome, list);
			GetPathComponents(classPathProp, list);
			GetPathComponents(bootClassPathProp, list);
			System.Collections.Generic.List<string> dirs = new System.Collections.Generic.List
				<string>();
			GetPathComponents(extDirs, dirs);
			foreach (string d in dirs)
			{
				DirectoryInfo ext_dir = new DirectoryInfo(d);
				string[] extensions = ext_dir.GetFiles(). Where(f => ARCHIVE_FILTER.Invoke(f, f.Name)).Select(c => c.Extension).Distinct().ToArray();
				if (extensions != null)
				{
					foreach (string extension in extensions)
					{
						list.Add(ext_dir.FullName + System.IO.Path.DirectorySeparatorChar + extension);
					}
				}
			}
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			string separator = string.Empty;
			foreach (string path in list)
			{
				buf.Append(separator);
				separator = Path.DirectorySeparatorChar.ToString();
				buf.Append(path);
			}
			return string.Intern(buf.ToString());
		}

		private static void GetPathComponents(string path, System.Collections.Generic.List
			<string> list)
		{
			if (path != null)
			{
				list.AddRange(path.Split(Path.DirectorySeparatorChar));
			}
		}

		internal static string PackageToFolder(string name)
		{
			return name.Replace('.', '/');
		}

		private readonly string classPath;

		private NBCEL.util.ClassPath parent;

		private readonly NBCEL.util.ClassPath.AbstractPathEntry[] paths;

		/// <summary>Search for classes in CLASSPATH.</summary>
		[System.ObsoleteAttribute(@"Use SYSTEM_CLASS_PATH constant")]
		public ClassPath()
			: this(GetClassPath())
		{
		}

		public ClassPath(NBCEL.util.ClassPath parent, string classPath)
			: this(classPath)
		{
			this.parent = parent;
		}

		/// <summary>Search for classes in given path.</summary>
		/// <param name="classPath"/>
		public ClassPath(string classPath)
		{
			this.classPath = classPath;
			System.Collections.Generic.List<NBCEL.util.ClassPath.AbstractPathEntry> list = new 
				System.Collections.Generic.List<NBCEL.util.ClassPath.AbstractPathEntry>();
			paths = new NBCEL.util.ClassPath.AbstractPathEntry[list.Count];
			Sharpen.Collections.ToArray(list, paths);
		}

		/// <exception cref="IOException"/>
		public virtual void Close()
		{
			if (paths != null)
			{
				foreach (NBCEL.util.ClassPath.AbstractPathEntry path in paths)
				{
					path.Close();
				}
			}
		}

		public override bool Equals(object o)
		{
			if (o is NBCEL.util.ClassPath)
			{
				NBCEL.util.ClassPath cp = (NBCEL.util.ClassPath)o;
				return classPath.Equals(cp.ToString());
			}
			return false;
		}

		/// <returns>byte array for class</returns>
		/// <exception cref="IOException"/>
		public virtual byte[] GetBytes(string name)
		{
			return GetBytes(name, ".class");
		}

		/// <param name="name">fully qualified file name, e.g. java/lang/String</param>
		/// <param name="suffix">file name ends with suffix, e.g. .java</param>
		/// <returns>byte array for file on class path</returns>
		/// <exception cref="IOException"/>
		public virtual byte[] GetBytes(string name, string suffix)
		{
			java.io.DataInputStream dis = null;
			try
			{
				using (java.io.InputStream inputStream = GetInputStream(name, suffix))
				{
					if (inputStream == null)
					{
						throw new System.IO.IOException("Couldn't find: " + name + suffix);
					}
					dis = new java.io.DataInputStream(inputStream);
					byte[] bytes = new byte[inputStream.Available()];
					dis.ReadFully(bytes);
					return bytes;
				}
			}
			finally
			{
				if (dis != null)
				{
					dis.Close();
				}
			}
		}

		/// <param name="name">fully qualified class name, e.g. java.lang.String</param>
		/// <returns>input stream for class</returns>
		/// <exception cref="IOException"/>
		public virtual NBCEL.util.ClassPath.ClassFile GetClassFile(string name)
		{
			return GetClassFile(name, ".class");
		}

		/// <param name="name">fully qualified file name, e.g. java/lang/String</param>
		/// <param name="suffix">file name ends with suff, e.g. .java</param>
		/// <returns>class file for the java class</returns>
		/// <exception cref="IOException"/>
		public virtual NBCEL.util.ClassPath.ClassFile GetClassFile(string name, string suffix
			)
		{
			NBCEL.util.ClassPath.ClassFile cf = null;
			if (parent != null)
			{
				cf = parent.GetClassFileInternal(name, suffix);
			}
			if (cf == null)
			{
				cf = GetClassFileInternal(name, suffix);
			}
			if (cf != null)
			{
				return cf;
			}
			throw new System.IO.IOException("Couldn't find: " + name + suffix);
		}

		/// <exception cref="IOException"/>
		private NBCEL.util.ClassPath.ClassFile GetClassFileInternal(string name, string suffix
			)
		{
			foreach (NBCEL.util.ClassPath.AbstractPathEntry path in paths)
			{
				NBCEL.util.ClassPath.ClassFile cf = path.GetClassFile(name, suffix);
				if (cf != null)
				{
					return cf;
				}
			}
			return null;
		}

		/// <param name="name">fully qualified class name, e.g. java.lang.String</param>
		/// <returns>input stream for class</returns>
		/// <exception cref="IOException"/>
		public virtual java.io.InputStream GetInputStream(string name)
		{
			return GetInputStream(PackageToFolder(name), ".class");
		}

		/// <summary>Return stream for class or resource on CLASSPATH.</summary>
		/// <param name="name">fully qualified file name, e.g. java/lang/String</param>
		/// <param name="suffix">file name ends with suff, e.g. .java</param>
		/// <returns>input stream for file on class path</returns>
		/// <exception cref="IOException"/>
		public virtual java.io.InputStream GetInputStream(string name, string suffix)
		{
			java.io.InputStream inputStream = null;
			try
			{
				// inputStream = GetType().GetClassLoader().GetResourceAsStream(name + suffix);
			}
			catch (System.Exception)
			{
			}
			// may return null
			// ignored
			if (inputStream != null)
			{
				return inputStream;
			}
			return GetClassFile(name, suffix).GetInputStream();
		}

		/// <param name="name">name of file to search for, e.g. java/lang/String.java</param>
		/// <returns>full (canonical) path for file</returns>
		/// <exception cref="IOException"/>
		public virtual string GetPath(string name)
		{
			int index = name.LastIndexOf('.');
			string suffix = string.Empty;
			if (index > 0)
			{
				suffix = Sharpen.Runtime.Substring(name, index);
				name = Sharpen.Runtime.Substring(name, 0, index);
			}
			return GetPath(name, suffix);
		}

		/// <param name="name">name of file to search for, e.g. java/lang/String</param>
		/// <param name="suffix">file name suffix, e.g. .java</param>
		/// <returns>full (canonical) path for file, if it exists</returns>
		/// <exception cref="IOException"/>
		public virtual string GetPath(string name, string suffix)
		{
			return GetClassFile(name, suffix).GetPath();
		}

		/// <param name="name">fully qualified resource name, e.g. java/lang/String.class</param>
		/// <returns>URL supplying the resource, or null if no resource with that name.</returns>
		/// <since>6.0</since>
		public virtual System.Uri GetResource(string name)
		{
			foreach (NBCEL.util.ClassPath.AbstractPathEntry path in paths)
			{
				System.Uri url;
				if ((url = path.GetResource(name)) != null)
				{
					return url;
				}
			}
			return null;
		}

		/// <param name="name">fully qualified resource name, e.g. java/lang/String.class</param>
		/// <returns>InputStream supplying the resource, or null if no resource with that name.
		/// 	</returns>
		/// <since>6.0</since>
		public virtual java.io.InputStream GetResourceAsStream(string name)
		{
			foreach (NBCEL.util.ClassPath.AbstractPathEntry path in paths)
			{
				java.io.InputStream @is;
				if ((@is = path.GetResourceAsStream(name)) != null)
				{
					return @is;
				}
			}
			return null;
		}

		/*
		/// <param name="name">fully qualified resource name, e.g. java/lang/String.class</param>
		/// <returns>An Enumeration of URLs supplying the resource, or an empty Enumeration if no resource with that name.
		/// 	</returns>
		/// <since>6.0</since>
		public virtual java.util.Enumeration<System.Uri> GetResources(string name)
		{
			java.util.Vector<System.Uri> results = new java.util.Vector<System.Uri>();
			foreach (NBCEL.util.ClassPath.AbstractPathEntry path in paths)
			{
				System.Uri url;
				if ((url = path.GetResource(name)) != null)
				{
					results.Add(url);
				}
			}
			return results.GetEnumerator();
		}
		*/

		public override int GetHashCode()
		{
			if (parent != null)
			{
				return classPath.GetHashCode() + parent.GetHashCode();
			}
			return classPath.GetHashCode();
		}

		/// <returns>used class path string</returns>
		public override string ToString()
		{
			if (parent != null)
			{
				return parent.ToString() + Path.PathSeparator + classPath;
			}
			return classPath;
		}

		public void Dispose()
		{
			Close();
		}
	}
}
