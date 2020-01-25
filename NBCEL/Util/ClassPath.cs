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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.Util
{
	/// <summary>Responsible for loading (class) files from the CLASSPATH.</summary>
	/// <remarks>
	///     Responsible for loading (class) files from the CLASSPATH. Inspired by sun.tools.ClassPath.
	/// </remarks>
	public class ClassPath : Closeable
    {
        private static readonly Func<FileInfo, string, bool> ARCHIVE_FILTER = (dir
            , name) =>
        {
            name = name.ToLower();
            return name.EndsWith(".zip") || name.EndsWith(".jar");
        };

        private static readonly Func<FileInfo, string, bool> MODULES_FILTER = (dir
            , name) =>
        {
            name = name.ToLower();
            return name.EndsWith(".jmod");
        };

        public static readonly ClassPath SYSTEM_CLASS_PATH = new ClassPath
            (GetClassPath());

        private readonly string classPath;

        private readonly AbstractPathEntry[] paths;

        private readonly ClassPath parent;

        /// <summary>Search for classes in CLASSPATH.</summary>
        [Obsolete(@"Use SYSTEM_CLASS_PATH constant")]
        public ClassPath()
            : this(GetClassPath())
        {
        }

        public ClassPath(ClassPath parent, string classPath)
            : this(classPath)
        {
            this.parent = parent;
        }

        /// <summary>Search for classes in given path.</summary>
        /// <param name="classPath" />
        public ClassPath(string classPath)
        {
            this.classPath = classPath;
            var list = new
                List<AbstractPathEntry>();
            paths = new AbstractPathEntry[list.Count];
            Collections.ToArray(list, paths);
        }

        /// <exception cref="IOException" />
        public virtual void Close()
        {
            if (paths != null)
                foreach (var path in paths)
                    path.Close();
        }

        public void Dispose()
        {
            Close();
        }

        private static void AddJdkModules(string javaHome, List
            <string> list)
        {
            try
            {
                var modulesPath = Runtime.GetProperty("java.modules.path");
                if (modulesPath == null || modulesPath.Trim().Length == 0)
                    // Default to looking in JAVA_HOME/jmods
                    modulesPath = javaHome + Path.DirectorySeparatorChar + "jmods";
                var modulesDir = new DirectoryInfo(modulesPath);
                if (modulesDir.Exists)
                {
                    var modules = modulesDir.GetFiles().Where(f => MODULES_FILTER.Invoke(f, f.Name)).Select(c => c.FullName)
                        .ToArray();
                    for (var i = 0; i < modules.Length; i++)
                        list.Add(modulesDir.FullName + Path.DirectorySeparatorChar + modules[i]);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        ///     Checks for class path components in the following properties: "java.class.path", "sun.boot.class.path",
        ///     "java.ext.dirs"
        /// </summary>
        /// <returns>class path as used by default by BCEL</returns>
        public static string GetClassPath()
        {
            // @since 6.0 no longer final
            var classPathProp = Runtime.GetProperty("java.class.path");
            var bootClassPathProp = Runtime.GetProperty("sun.boot.class.path");
            var extDirs = Runtime.GetProperty("java.ext.dirs");
            // System.out.println("java.version = " + System.getProperty("java.version"));
            // System.out.println("java.class.path = " + classPathProp);
            // System.out.println("sun.boot.class.path=" + bootClassPathProp);
            // System.out.println("java.ext.dirs=" + extDirs);
            var javaHome = Runtime.GetProperty("java.home");
            var list = new List
                <string>();

            // Starting in JDK 9, .class files are in the jmods directory. Add them to the path.
            AddJdkModules(javaHome, list);
            GetPathComponents(classPathProp, list);
            GetPathComponents(bootClassPathProp, list);
            var dirs = new List
                <string>();
            GetPathComponents(extDirs, dirs);
            foreach (var d in dirs)
            {
                try
                {
                    var ext_dir = new DirectoryInfo(d);
                    var extensions = ext_dir.GetFiles().Where(f => ARCHIVE_FILTER.Invoke(f, f.Name))
                        .Select(c => c.Extension).Distinct().ToArray();
                    if (extensions != null)
                        foreach (var extension in extensions)
                            list.Add(ext_dir.FullName + Path.DirectorySeparatorChar + extension);
                }
                catch
                {
                    // ignored
                }
            }

            var buf = new StringBuilder();
            var separator = string.Empty;
            foreach (var path in list)
            {
                buf.Append(separator);
                separator = Path.DirectorySeparatorChar.ToString();
                buf.Append(path);
            }

            return string.Intern(buf.ToString());
        }

        private static void GetPathComponents(string path, List
            <string> list)
        {
            if (path != null) list.AddRange(path.Split(Path.DirectorySeparatorChar));
        }

        internal static string PackageToFolder(string name)
        {
            return name.Replace('.', '/');
        }

        public override bool Equals(object o)
        {
            if (o is ClassPath)
            {
                var cp = (ClassPath) o;
                return classPath.Equals(cp.ToString());
            }

            return false;
        }

        /// <returns>byte array for class</returns>
        /// <exception cref="IOException" />
        public virtual byte[] GetBytes(string name)
        {
            return GetBytes(name, ".class");
        }

        /// <param name="name">fully qualified file name, e.g. java/lang/String</param>
        /// <param name="suffix">file name ends with suffix, e.g. .java</param>
        /// <returns>byte array for file on class path</returns>
        /// <exception cref="IOException" />
        public virtual byte[] GetBytes(string name, string suffix)
        {
            DataInputStream dis = null;
            try
            {
                using (var inputStream = GetInputStream(name, suffix))
                {
                    if (inputStream == null) throw new IOException("Couldn't find: " + name + suffix);
                    dis = new DataInputStream(inputStream);
                    var bytes = new byte[inputStream.Available()];
                    dis.ReadFully(bytes);
                    return bytes;
                }
            }
            finally
            {
                if (dis != null) dis.Close();
            }
        }

        /// <param name="name">fully qualified class name, e.g. java.lang.String</param>
        /// <returns>input stream for class</returns>
        /// <exception cref="IOException" />
        public virtual ClassFile GetClassFile(string name)
        {
            return GetClassFile(name, ".class");
        }

        /// <param name="name">fully qualified file name, e.g. java/lang/String</param>
        /// <param name="suffix">file name ends with suff, e.g. .java</param>
        /// <returns>class file for the java class</returns>
        /// <exception cref="IOException" />
        public virtual ClassFile GetClassFile(string name, string suffix
        )
        {
            ClassFile cf = null;
            if (parent != null) cf = parent.GetClassFileInternal(name, suffix);
            if (cf == null) cf = GetClassFileInternal(name, suffix);
            if (cf != null) return cf;
            throw new IOException("Couldn't find: " + name + suffix);
        }

        /// <exception cref="IOException" />
        private ClassFile GetClassFileInternal(string name, string suffix
        )
        {
            foreach (var path in paths)
            {
                var cf = path.GetClassFile(name, suffix);
                if (cf != null) return cf;
            }

            return null;
        }

        /// <param name="name">fully qualified class name, e.g. java.lang.String</param>
        /// <returns>input stream for class</returns>
        /// <exception cref="IOException" />
        public virtual InputStream GetInputStream(string name)
        {
            return GetInputStream(PackageToFolder(name), ".class");
        }

        /// <summary>Return stream for class or resource on CLASSPATH.</summary>
        /// <param name="name">fully qualified file name, e.g. java/lang/String</param>
        /// <param name="suffix">file name ends with suff, e.g. .java</param>
        /// <returns>input stream for file on class path</returns>
        /// <exception cref="IOException" />
        public virtual InputStream GetInputStream(string name, string suffix)
        {
            InputStream inputStream = null;
            try
            {
                // inputStream = GetType().GetClassLoader().GetResourceAsStream(name + suffix);
            }
            catch (Exception)
            {
            }

            // may return null
            // ignored
            if (inputStream != null) return inputStream;
            return GetClassFile(name, suffix).GetInputStream();
        }

        /// <param name="name">name of file to search for, e.g. java/lang/String.java</param>
        /// <returns>full (canonical) path for file</returns>
        /// <exception cref="IOException" />
        public virtual string GetPath(string name)
        {
            var index = name.LastIndexOf('.');
            var suffix = string.Empty;
            if (index > 0)
            {
                suffix = Runtime.Substring(name, index);
                name = Runtime.Substring(name, 0, index);
            }

            return GetPath(name, suffix);
        }

        /// <param name="name">name of file to search for, e.g. java/lang/String</param>
        /// <param name="suffix">file name suffix, e.g. .java</param>
        /// <returns>full (canonical) path for file, if it exists</returns>
        /// <exception cref="IOException" />
        public virtual string GetPath(string name, string suffix)
        {
            return GetClassFile(name, suffix).GetPath();
        }

        /// <param name="name">fully qualified resource name, e.g. java/lang/String.class</param>
        /// <returns>URL supplying the resource, or null if no resource with that name.</returns>
        /// <since>6.0</since>
        public virtual Uri GetResource(string name)
        {
            foreach (var path in paths)
            {
                Uri url;
                if ((url = path.GetResource(name)) != null) return url;
            }

            return null;
        }

        /// <param name="name">fully qualified resource name, e.g. java/lang/String.class</param>
        /// <returns>
        ///     InputStream supplying the resource, or null if no resource with that name.
        /// </returns>
        /// <since>6.0</since>
        public virtual InputStream GetResourceAsStream(string name)
        {
            foreach (var path in paths)
            {
                InputStream @is;
                if ((@is = path.GetResourceAsStream(name)) != null) return @is;
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
            if (parent != null) return classPath.GetHashCode() + parent.GetHashCode();
            return classPath.GetHashCode();
        }

        /// <returns>used class path string</returns>
        public override string ToString()
        {
            if (parent != null) return parent.ToString() + Path.PathSeparator + classPath;
            return classPath;
        }

        private abstract class AbstractPathEntry : Closeable
        {
            public abstract void Close();

            public void Dispose()
            {
                Close();
            }

            /// <exception cref="IOException" />
            internal abstract ClassFile GetClassFile(string name, string
                suffix);

            internal abstract Uri GetResource(string name);

            internal abstract InputStream GetResourceAsStream(string name);
        }

        private abstract class AbstractZip : AbstractPathEntry
        {
            private readonly ZipArchive zipFile;

            internal AbstractZip(ZipArchive zipFile)
            {
                this.zipFile = zipFile;
            }

            /// <exception cref="IOException" />
            public override void Close()
            {
                if (zipFile != null) zipFile.Dispose();
            }

            /// <exception cref="IOException" />
            internal override ClassFile GetClassFile(string name, string
                suffix)
            {
                var entry = zipFile.GetEntry(ToEntryName(name, suffix));
                if (entry == null) return null;
                return new _ClassFile_82(this, entry);
            }

            internal override Uri GetResource(string name)
            {
                var entry = zipFile.GetEntry(name);
                try
                {
                    return entry != null
                        ? new Uri("jar:file:" + name + "!/" +
                                  name)
                        : null;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            internal override InputStream GetResourceAsStream(string name)
            {
                var entry = zipFile.GetEntry(name);
                try
                {
                    return entry != null ? entry.Open().ReadFully().ToInputStream() : null;
                }
                catch (IOException)
                {
                    return null;
                }
            }

            protected internal abstract string ToEntryName(string name, string suffix);

            public override string ToString()
            {
                return zipFile.ToString();
            }

            private sealed class _ClassFile_82 : ClassFile
            {
                private readonly AbstractZip _enclosing;

                private readonly ZipArchiveEntry entry;

                public _ClassFile_82(AbstractZip _enclosing, ZipArchiveEntry entry)
                {
                    this._enclosing = _enclosing;
                    this.entry = entry;
                }

                public string GetBase()
                {
                    return "";
                }

                /// <exception cref="IOException" />
                public InputStream GetInputStream()
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
            }
        }

        /// <summary>Contains information about file/ZIP entry of the Java class.</summary>
        public interface ClassFile
        {
	        /// <returns>
	        ///     base path of found class, i.e. class is contained relative to that path, which may either denote a
	        ///     directory, or zip file
	        /// </returns>
	        string GetBase();

	        /// <returns>input stream for class file.</returns>
	        /// <exception cref="IOException" />
	        InputStream GetInputStream();

            /// <returns>canonical path to class file.</returns>
            string GetPath();

            /// <returns>size of class file.</returns>
            long GetSize();

            /// <returns>modification time of class file.</returns>
            long GetTime();
        }

        private class Dir : AbstractPathEntry
        {
            private readonly string dir;

            internal Dir(string d)
            {
                dir = d;
            }

            /// <exception cref="IOException" />
            public override void Close()
            {
            }

            // Nothing to do
            /// <exception cref="IOException" />
            internal override ClassFile GetClassFile(string name, string
                suffix)
            {
                FileSystemInfo file = new FileInfo(dir + Path.DirectorySeparatorChar + name.Replace
                                                       ('.', Path.DirectorySeparatorChar) + suffix);
                return file.Exists ? new _ClassFile_189(this, file) : null;
            }

            internal override Uri GetResource(string name)
            {
                // Resource specification uses '/' whatever the platform
                var file = ToFile(name);
                try
                {
                    return file.Exists ? new Uri(file.FullName) : null;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            internal override InputStream GetResourceAsStream(string name)
            {
                // Resource specification uses '/' whatever the platform
                var file = ToFile(name);
                try
                {
                    return file.Exists ? File.ReadAllBytes(file.FullName).ToInputStream() : null;
                }
                catch (IOException)
                {
                    return null;
                }
            }

            private FileSystemInfo ToFile(string name)
            {
                return new FileInfo(dir + Path.DirectorySeparatorChar + name.Replace('/', Path.PathSeparator));
            }

            public override string ToString()
            {
                return dir;
            }

            private sealed class _ClassFile_189 : ClassFile
            {
                private readonly Dir _enclosing;

                private readonly FileInfo file;

                public _ClassFile_189(Dir _enclosing, FileSystemInfo file)
                {
                    this._enclosing = _enclosing;
                    this.file = (FileInfo) file;
                }

                public string GetBase()
                {
                    return _enclosing.dir;
                }

                /// <exception cref="IOException" />
                public InputStream GetInputStream()
                {
                    return File.ReadAllBytes(file.FullName).ToInputStream();
                }

                public string GetPath()
                {
                    try
                    {
                        return file.FullName;
                    }
                    catch (IOException)
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
                    return (long) (System.Epoch - file.LastWriteTime).TotalMilliseconds;
                }
            }
        }

        private class Jar : AbstractZip
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

        private class JrtModule : AbstractPathEntry
        {
            private readonly string modulePath;

            public JrtModule(string modulePath)
            {
                this.modulePath = modulePath;
            }

            /// <exception cref="IOException" />
            public override void Close()
            {
            }

            // Nothing to do.
            /// <exception cref="IOException" />
            internal override ClassFile GetClassFile(string name, string
                suffix)
            {
                // java.nio.file.Path resolved = modulePath.Resolve(PackageToFolder(name) + suffix);
                // if (java.nio.file.Files.Exists(resolved))
                // {
                // return new _ClassFile_285(resolved);
                // }
                return null;
            }

            internal override Uri GetResource(string name)
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

            internal override InputStream GetResourceAsStream(string name)
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
                return modulePath;
            }

            private sealed class _ClassFile_285 : ClassFile
            {
                private readonly string resolved;

                public _ClassFile_285(string resolved)
                {
                    this.resolved = resolved;
                }

                public string GetBase()
                {
                    return resolved;
                }

                /// <exception cref="IOException" />
                public InputStream GetInputStream()
                {
                    return File.ReadAllBytes(resolved).ToInputStream();
                }

                public string GetPath()
                {
                    return resolved;
                }

                public long GetSize()
                {
                    try
                    {
                        return new FileInfo(resolved).Length;
                    }
                    catch (IOException)
                    {
                        return 0;
                    }
                }

                public long GetTime()
                {
                    try
                    {
                        return (long) (System.Epoch - new FileInfo(resolved).LastWriteTime).TotalMilliseconds;
                    }
                    catch (IOException)
                    {
                        return 0;
                    }
                }
            }
        }

        private class JrtModules : AbstractPathEntry
        {
            private readonly JrtModule[] modules;

            /// <exception cref="IOException" />
            public JrtModules(string path)
            {
                modules = new JrtModule[0];
            }

            /// <exception cref="IOException" />
            public override void Close()
            {
                if (modules != null)
                    // don't use a for each loop to avoid creating an iterator for the GC to collect.
                    for (var i = 0; i < modules.Length; i++)
                        modules[i].Close();
            }

            /// <exception cref="IOException" />
            internal override ClassFile GetClassFile(string name, string
                suffix)
            {
                // don't use a for each loop to avoid creating an iterator for the GC to collect.
                for (var i = 0; i < modules.Length; i++)
                {
                    var classFile = modules[i].GetClassFile(name, suffix);
                    if (classFile != null) return classFile;
                }

                return null;
            }

            internal override Uri GetResource(string name)
            {
                // don't use a for each loop to avoid creating an iterator for the GC to collect.
                for (var i = 0; i < modules.Length; i++)
                {
                    var url = modules[i].GetResource(name);
                    if (url != null) return url;
                }

                return null;
            }

            internal override InputStream GetResourceAsStream(string name)
            {
                // don't use a for each loop to avoid creating an iterator for the GC to collect.
                for (var i = 0; i < modules.Length; i++)
                {
                    var inputStream = modules[i].GetResourceAsStream(name);
                    if (inputStream != null) return inputStream;
                }

                return null;
            }

            public override string ToString()
            {
                return Arrays.ToString(modules);
            }
        }

        private class Module : AbstractZip
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
    }
}