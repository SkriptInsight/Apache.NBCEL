using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Java.Nio;

namespace Apache.NBCEL
{
    public abstract class EnumBase : IComparable<EnumBase>, IComparable
    {
        private static readonly Dictionary<Type, EnumBase[]> ValuesMap = new Dictionary<Type, EnumBase[]>();

        private readonly int _ordinal;
        private readonly string _name;

        protected EnumBase(int ordinal, string name)
        {
            _ordinal = ordinal;
            _name = name;
        }

        public int ordinal()
        {
            return _ordinal;
        }

        public string name()
        {
            return _name;
        }

        public override string ToString()
        {
            return _name;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((EnumBase) obj);
        }

        public int CompareTo(EnumBase other)
        {
            return this._ordinal - other._ordinal;
        }

        public static bool IsEnum(Type t)
        {
            return ValuesMap.ContainsKey(t);
        }

        protected static void RegisterValues<T>(EnumBase[] values) where T : EnumBase
        {
            ValuesMap[typeof(T)] = values;
        }

        public static EnumBase[] GetEnumValues(Type enumType)
        {
            EnumBase[] result;
            if (ValuesMap.TryGetValue(enumType, out result))
            {
                return result;
            }
            else
            {
                RuntimeHelpers.RunClassConstructor(enumType.TypeHandle);
                return ValuesMap[enumType];
            }
        }


        public static T FindByName<T>(string name) where T : EnumBase
        {
            return name == null ? null : (T) GetEnumValues(typeof(T)).FirstOrDefault(val => val.name() == name);
        }

        public static T GetByName<T>(string name) where T : EnumBase
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return (T) GetEnumValues(typeof(T)).First(val => val.name() == name);
        }
    }

    public class System
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int Compare(int x, int y)
        {
            return (x < y) ? -1 : ((x == y) ? 0 : 1);
        }

        public static int Compare(long x, long y)
        {
            return (x < y) ? -1 : ((x == y) ? 0 : 1);
        }

        public static long CurrentTimeMillis()
        {
            return (long) (DateTime.UtcNow - Epoch).TotalMilliseconds;
        }

        public static int FloorDiv(int x, int y)
        {
            int r = x / y;
            if ((x ^ y) < 0 && (r * y != x))
            {
                r--;
            }

            return r;
        }

        public static int Round(float v)
        {
            return (int) Math.Floor(v + 0.5f);
        }

        public static long Round(double v)
        {
            return (long) Math.Floor(v + 0.5d);
        }

        public static int HighestOneBit(int i)
        {
            uint u = (uint) i;
            u |= (u >> 1);
            u |= (u >> 2);
            u |= (u >> 4);
            u |= (u >> 8);
            u |= (u >> 16);
            return (int) (u - (u >> 1));
        }

        public static bool GetBoolean(string name)
        {
            return Environment.GetEnvironmentVariable(name)?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public static int GetInteger(string name, int i)
        {
            var @var = Environment.GetEnvironmentVariable(name);
            int result;
            if (!int.TryParse(@var, out result))
            {
                result = i;
            }

            return result;
        }
    }

    public class Arrays
    {
        public static List<T> AsList<T>(params T[] a)
        {
            return a.ToList();
        }

        public static void Fill<T>(T[] a, T val)
        {
            Fill(a, 0, a.Length, val);
        }

        public static void Fill<T>(T[] a, int from, int to, T val)
        {
            for (int i = from; i < to; i++)
            {
                a[i] = val;
            }
        }

        public static T[] CopyOf<T>(T[] a, int newSize)
        {
            T[] result = new T[newSize];
            a.CopyTo(result, 0);
            return result;
        }

        public static int HashCode<T>(T[] a)
        {
            if (a == null)
            {
                return 0;
            }

            int result = 1;
            foreach (var element in a)
            {
                result = 31 * result + element.GetHashCode();
            }

            return result;
        }

        public static string ToString<T>(T[] a)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (var i = 0; i < a.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(a[i]);
            }

            sb.Append("]");
            return sb.ToString();
        }
    }

    public static class Collections
    {
        public static T Next<T>(this IEnumerator<T> enumerator)
        {
            return new EnumeratorAdapter<T>(enumerator).Next();
        }

        public static bool HasNext<T>(this IEnumerator<T> enumerator)
        {
            return new EnumeratorAdapter<T>(enumerator).HasNext;
        }

        public static void PutIfAbsent<TK, TV>(this Dictionary<TK, TV> map, TK key, TV value)
        {
            if (!map.ContainsKey(key))
                map.Add(key, value);
        }

        public static TV ComputeIfAbsent<TK, TV>(this Dictionary<TK, TV> map, TK key, Func<TK, TV> value)
        {
            if (!map.ContainsKey(key))
            {
                var newValue = value(key);
                map.Add(key, newValue);
                return newValue;
            }

            return map[key];
        }

        public static object Put(IDictionary map, object key, object value)
        {
            object result = map.Contains(key) ? map[key] : null;
            map[key] = value;
            return result;
        }

        public const string NullStringKey = "__null__";

        public static TV Put<TK, TV>(IDictionary<TK, TV> map, TK key, TV value)
        {
            if (key == null && map is IDictionary<string, TV> newMap)
            {
                TV newResult;
                if (!newMap.TryGetValue(NullStringKey, out newResult))
                {
                    newResult = default(TV);
                }

                newMap[NullStringKey] = value;
                return newResult;
            }

            TV result;
            if (!map.TryGetValue(key, out result))
            {
                result = default(TV);
            }

            map[key] = value;
            return result;
        }

        public static TV Remove<TK, TV>(IDictionary<TK, TV> map, TK key)
        {
            TV result;
            if (map.TryGetValue(key, out result))
            {
                map.Remove(key);
                return result;
            }

            return default(TV);
        }

        public static T RemoveFirst<T>(this LinkedList<T> linkedList)
        {
            var result = linkedList.First.Value;
            linkedList.RemoveFirst();
            return result;
        }

        public static T RemoveFirst<T>(this List<T> linkedList)
        {
            var result = linkedList.First();
            linkedList.RemoveAt(0);
            return result;
        }

        public static void AddFirst<T>(this List<T> linkedList, T value)
        {
            linkedList.Insert(0, value);
         }

        public static T RemoveLast<T>(this LinkedList<T> linkedList)
        {
            var result = linkedList.Last.Value;
            linkedList.RemoveLast();
            return result;
        }

        public static void PutAll<TCk, TCv, TIk, TIv>(IDictionary<TCk, TCv> collection, IDictionary<TIk, TIv> items)
            where TIk : TCk where TIv : TCv
        {
            foreach (var e in items)
            {
                collection[e.Key] = e.Value;
            }
        }

        public static void AddAll<T>(ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static T[] ToArray<T>(ICollection<T> collection)
        {
            return ToArray(collection, new T[collection.Count]);
        }

        public static T[] ToArray<T>(ICollection<T> collection, T[] array)
        {
            int i = 0;
            foreach (var item in collection)
            {
                array[i++] = item;
            }

            return array;
        }
    }

    public static class Runtime
    {

        public static Type GetType(string name)
        {
            return GetJavaType(name);
        }
        public static string Substring(this StringBuilder source, int from)
        {
            return Substring(source.ToString(), from);
        }
        
        public static string Substring(this StringBuilder source, int from, int to)
        {
            return Substring(source.ToString(), from, to);
        }
        
        public static InputStream ToInputStream(this byte[] source)
        {
            return new MemoryInputStream(new MemoryStream(source));
        }
        
        public static OutputStream ToOutputStream(this byte[] source)
        {
            return new MemoryOutputStream(new MemoryStream(source));
        }
        
        public static byte[] ReadFully(this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static void RemoveIf<TK, TV>(this Dictionary<TK, TV> source, Func<KeyValuePair<TK, TV>, bool> predicate)
        {
            foreach (var keyValuePair in source.Where(predicate))
            {
                source.Remove(keyValuePair.Key);
            }
        }

        public static bool IsAssignableFrom(Type baseType, Type type)
        {
            if (baseType.IsAssignableFrom(type))
            {
                return true;
            }

            var baseTypeGeneric = baseType.IsGenericType ? baseType.GetGenericTypeDefinition() : baseType;
            var typeGeneric = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            return baseTypeGeneric.IsAssignableFrom(typeGeneric);
        }

        public static bool InstanceOf(object o, Type type)
        {
            if (o == null)
            {
                return false;
            }

            if (type.IsInstanceOfType(o))
            {
                return true;
            }

            return o.GetType().IsGenericType && o.GetType().GetGenericTypeDefinition().IsAssignableFrom(type);
        }

        public static string Substring(string s, int from)
        {
            return Substring(s, from, s.Length);
        }

        public static string Substring(string s, int from, int to)
        {
            return s.Substring(from, to - from);
        }

        public static string GetSimpleName(this Type t)
        {
            string name = t.Name;
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        public static FieldInfo[] GetDeclaredFields(Type clazz)
        {
            return clazz.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly |
                                   BindingFlags.Instance);
        }

        public static bool HasAttribute(FieldAttributes attributes, FieldAttributes flag)
        {
            return (attributes & flag) != 0;
        }

        public static CustomAttributeData GetCustomAttribute(MemberInfo info, Type attributeType)
        {
            foreach (var a in CustomAttributeData.GetCustomAttributes(info))
            {
                if (a.Constructor.DeclaringType == attributeType)
                {
                    return a;
                }
            }

            return null;
        }

        public static string ToHexString(int p0)
        {
            return p0.ToString("x8").ToUpper();
        }

        public static float IntBitsToFloat(int readInt)
        {
            var bytes = BitConverter.GetBytes(readInt);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double LongBitsToDouble(long l)
        {
            return BitConverter.Int64BitsToDouble(l);
        }

        public static int FloatToIntBits(in float f)
        {
            return BitConverter.SingleToInt32Bits(f);
        }

        public static int FloatToRawIntBits(in float value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long DoubleToRawLongBits(in double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        public static List<Type> GetParameterTypes(this ConstructorInfo info)
        {
            return info.GetParameters().Select(c => c.ParameterType).ToList();
        }

        public static List<Type> GetParameterTypes(this MethodInfo info)
        {
            return info.GetParameters().Select(c => c.ParameterType).ToList();
        }

        public static int NumberOfTrailingZeros(in long i)
        {
            int x, y;
            if (i == 0) return 64;
            int n = 63;
            y = (int) i;
            if (y != 0)
            {
                n = n - 32;
                x = y;
            }
            else x = (int) (i >> 32);

            y = x << 16;
            if (y != 0)
            {
                n = n - 16;
                x = y;
            }

            y = x << 8;
            if (y != 0)
            {
                n = n - 8;
                x = y;
            }

            y = x << 4;
            if (y != 0)
            {
                n = n - 4;
                x = y;
            }

            y = x << 2;
            if (y != 0)
            {
                n = n - 2;
                x = y;
            }

            return n - ((x << 1) >> 31);
        }

        public static string ReplaceAll(this string source, string pattern, string replacement)
        {
            return Regex.Replace(source, pattern, replacement);
        }

        public static int BitCount(long i)
        {
            // HD, Figure 5-14
            i = i - ((i >> 1) & 0x5555555555555555L);
            i = (i & 0x3333333333333333L) + ((i >> 2) & 0x3333333333333333L);
            i = (i + (i >> 4)) & 0x0f0f0f0f0f0f0f0fL;
            i = i + (i >> 8);
            i = i + (i >> 16);
            i = i + (i >> 32);
            return (int) i & 0x7f;
        }

        public static int NumberOfLeadingZeros(long i)
        {
            // HD, Figure 5-6
            if (i == 0)
                return 64;
            var n = 1;
            var x = (int) (i >> 32);
            if (x == 0)
            {
                n += 32;
                x = (int) i;
            }

            if (x >> 16 == 0)
            {
                n += 16;
                x <<= 16;
            }

            if (x >> 24 == 0)
            {
                n += 8;
                x <<= 8;
            }

            if (x >> 28 == 0)
            {
                n += 4;
                x <<= 4;
            }

            if (x >> 30 == 0)
            {
                n += 2;
                x <<= 2;
            }

            n -= x >> 31;
            return n;
        }

        public static int RotateLeft(this int value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        public static int RotateRight(this int value, int count)
        {
            return (value >> count) | (value << (32 - count));
        }

        public static byte[] GetBytesForString(string value, string encoding = "UTF-8")
        {
            return Encoding.GetEncoding(encoding).GetBytes(value);
        }

        public static bool EqualsIgnoreCase(string text, string other)
        {
            return text.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetStringForBytes(byte[] value, string encoding = "UTF8")
        {
            return Encoding.GetEncoding(encoding).GetString(value);
        }

        public static void PrintStackTrace(Exception exception)
        {
            PrintStackTrace(exception, Console.Error);
        }

        public static void PrintStackTrace(Exception exception, TextWriter stream)
        {
            stream.WriteLine(exception.ToString());
        }

        public static long CurrentTimeMillis()
        {
            return (long) (DateTime.UtcNow - System.Epoch).TotalMilliseconds;
        }

        public static bool IsJavaIdentifierPart(in char c)
        {
            var s = c.ToString();
            const string start = @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl})";
            const string extend = @"(\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\p{Cf})";
            var ident = new Regex(string.Format("{0}({0}|{1})*", start, extend));
            s = s.Normalize();
            return ident.IsMatch(s);
        }

        public static bool Matches(this string source, string pattern)
        {
            return Regex.IsMatch(source, pattern);
        }

        public static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }
        
        /**
         * Returns the signum function of the specified {@code int} value.  (The
         * return value is -1 if the specified value is negative; 0 if the
         * specified value is zero; and 1 if the specified value is positive.)
         *
         * @param i the value whose signum is to be computed
         * @return the signum function of the specified {@code int} value.
         * @since 1.5
     */
        public static int Signum(int i)
        {
            // HD, Section 2-7
            return (i >> 31) | (-i >> 31);
        }

        public static string GetProperty(string prop, string defaultVal = "")
        {
            return Environment.GetEnvironmentVariable(prop) ?? defaultVal;
        }

        public static Type GetJavaType(string name)
        {
            return Type.GetType(name);
        }
    }

    public class IdentityHashMap<TK, TV> : Dictionary<TK, TV>
    {
        public IdentityHashMap() : base(new IdentityEqualityComparer<TK>())
        {
        }
    }

    public class IdentityEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

    public class Uuid : IEquatable<Uuid>, IComparable<Uuid>
    {
        private readonly long mostSigBits;
        private readonly long leastSigBits;

        public Uuid(long mostSigBits, long leastSigBits)
        {
            this.mostSigBits = mostSigBits;
            this.leastSigBits = leastSigBits;
        }

        public long GetMostSignificantBits()
        {
            return mostSigBits;
        }

        public long GetLeastSignificantBits()
        {
            return leastSigBits;
        }

        public override string ToString()
        {
            return ((mostSigBits >> 32) & 0xFFFFFFFF).ToString("x8") + "-" +
                   ((mostSigBits >> 16) & 0xFFFF).ToString("x4") + "-" +
                   (mostSigBits & 0xFFFF).ToString("x4") + "-" +
                   ((leastSigBits >> 48) & 0xFFFF).ToString("x4") + "-" +
                   (leastSigBits & 0xFFFFFFFFFFFF).ToString("x12");
        }

        public bool Equals(Uuid other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return this == obj as Uuid;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                long hilo = mostSigBits ^ leastSigBits;
                return ((int) (hilo >> 32)) ^ (int) hilo;
            }
        }

        public int CompareTo(Uuid other)
        {
            return (mostSigBits < other.mostSigBits
                ? -1
                : (mostSigBits > other.mostSigBits
                    ? 1
                    : (leastSigBits < other.leastSigBits ? -1 : (leastSigBits > other.leastSigBits ? 1 : 0))));
        }

        public static bool operator ==(Uuid first, Uuid second)
        {
            return ReferenceEquals(first, second) || !ReferenceEquals(first, null) && !ReferenceEquals(second, null) &&
                   first.mostSigBits == second.mostSigBits && first.leastSigBits == second.leastSigBits;
        }

        public static bool operator !=(Uuid first, Uuid second)
        {
            return !(first == second);
        }
    }


    public static class Lists
    {
        public static void Add<T>(this List<T> list, int index, T value)
        {
            list.Insert(index, value);
        }

        public static List<T> SubList<T>(this List<T> list, int fromIndex, int toIndex)
        {
            return list.GetRange(fromIndex, toIndex - fromIndex);
        }

        public static T RemoveAtReturningValue<T>(this IList<T> list, int index)
        {
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }

        public static T RemoveAtReturningValue<T>(this LinkedList<T> list, int index)
        {
            T value = list.ElementAt(index);
            list.RemoveLast();
            return value;
        }

        public static bool ContainsAll<T>(this ICollection<T> list, ICollection<T> other)
        {
            return other.All(list.Contains);
        }

        public static void RemoveAll<T>(this HashSet<T> list, HashSet<T> other)
        {
            list.ExceptWith(other);
        }

        public static void RemoveAll<T>(this ICollection<T> list, ICollection<T> other)
        {
            foreach (var x1 in other)
            {
                list.Remove(x1);
            }
        }

        public static void RetainAll<T>(this HashSet<T> list, HashSet<T> other)
        {
            list.IntersectWith(other);
        }

        public static void RetainAll<T>(this HashSet<T> list, ICollection<T> other)
        {
            list.IntersectWith(other);
        }
    }

    public static class Maps
    {
        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> map, TK key, TV defaultValue)
        {
            TV result;
            return map.TryGetValue(key, out result) ? result : defaultValue;
        }

        public static TV GetOrNull<TK, TV>(this IDictionary<TK, TV> map, TK key) where TV : class
        {
            TV result;
            if (key == null)
            {
                if (!(typeof(TK) == typeof(string))) return null;
                if (map is IDictionary<string, TV> newMap)
                {
                    return newMap.TryGetValue(Collections.NullStringKey, out result) ? result : null;
                }
            }


            return map.TryGetValue(key, out result) ? result : null;
        }

        public static TV? GetOrNullable<TK, TV>(this IDictionary<TK, TV> map, TK key) where TV : struct
        {
            TV result;
            return map.TryGetValue(key, out result) ? result : new TV?();
        }
    }
}