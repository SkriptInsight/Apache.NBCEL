/*
* Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
* ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*/

using System.Collections.Generic;

namespace ObjectWeb.Misc.Java.Util
{
    public static class SpliteratorConstants
    {
	    /// <summary>
	    ///     Characteristic value signifying that an encounter order is defined for
	    ///     elements.
	    /// </summary>
	    /// <remarks>
	    ///     Characteristic value signifying that an encounter order is defined for
	    ///     elements. If so, this Spliterator guarantees that method
	    ///     <see cref="Spliterator{T}.TrySplit()" />
	    ///     splits a strict prefix of elements, that method
	    ///     <see cref="Spliterator{T}.TryAdvance(Java.Util.Function.Consumer{T})" />
	    ///     steps by one element in prefix order, and that
	    ///     <see cref="Spliterator{T}.ForEachRemaining(Java.Util.Function.Consumer{T})" />
	    ///     performs actions in encounter order.
	    ///     <p>
	    ///         A
	    ///         <see cref="ICollection{T}" />
	    ///         has an encounter order if the corresponding
	    ///         <see cref="ICollection{E}.GetEnumerator()" />
	    ///         documents an order. If so, the encounter
	    ///         order is the same as the documented order. Otherwise, a collection does
	    ///         not have an encounter order.
	    /// </remarks>
	    /// <apiNote>
	    ///     Encounter order is guaranteed to be ascending index order for
	    ///     any
	    ///     <see cref="IList{E}" />
	    ///     . But no order is guaranteed for hash-based collections
	    ///     such as
	    ///     <see cref="HashSet
	    ///     
	    ///     <object>
	    ///         {E}"/>
	    ///         . Clients of a Spliterator that reports
	    ///         <c>ORDERED</c>
	    ///         are expected to preserve ordering constraints in
	    ///         non-commutative parallel computations.
	    /// </apiNote>
	    public const int Ordered = 0x00000010;

	    /// <summary>
	    ///     Characteristic value signifying that, for each pair of
	    ///     encountered elements
	    ///     <c>x, y</c>
	    ///     ,
	    ///     <c>!x.equals(y)</c>
	    ///     . This
	    ///     applies for example, to a Spliterator based on a
	    ///     <see cref="HashSet
	    ///     
	    ///     <object>
	    ///         {E}"/>
	    ///         .
	    /// </summary>
	    public const int Distinct = 0x00000001;

	    /// <summary>
	    ///     Characteristic value signifying that encounter order follows a defined
	    ///     sort order.
	    /// </summary>
	    /// <remarks>
	    ///     Characteristic value signifying that encounter order follows a defined
	    ///     sort order. If so, method
	    ///     <see cref="Spliterator{T}.GetComparator()" />
	    ///     returns the associated
	    ///     Comparator, or
	    ///     <see langword="null" />
	    ///     if all elements are
	    ///     <see cref="System.IComparable{T}" />
	    ///     and
	    ///     are sorted by their natural ordering.
	    ///     <p>
	    ///         A Spliterator that reports
	    ///         <c>SORTED</c>
	    ///         must also report
	    ///         <c>ORDERED</c>
	    ///         .
	    /// </remarks>
	    /// <apiNote>
	    ///     The spliterators for
	    ///     <c>Collection</c>
	    ///     classes in the JDK that
	    ///     implement
	    ///     <see cref="NavigableSet{E}" />
	    ///     or
	    ///     <see cref="SortedSet{E}" />
	    ///     report
	    ///     <c>SORTED</c>
	    ///     .
	    /// </apiNote>
	    public const int Sorted = 0x00000004;

	    /// <summary>
	    ///     Characteristic value signifying that the value returned from
	    ///     <c>estimateSize()</c>
	    ///     prior to traversal or splitting represents a
	    ///     finite size that, in the absence of structural source modification,
	    ///     represents an exact count of the number of elements that would be
	    ///     encountered by a complete traversal.
	    /// </summary>
	    /// <apiNote>
	    ///     Most Spliterators for Collections, that cover all elements of a
	    ///     <c>Collection</c>
	    ///     report this characteristic. Sub-spliterators, such as
	    ///     those for
	    ///     <see cref="HashSet
	    ///     
	    ///     <object>
	    ///         {E}"/>
	    ///         , that cover a sub-set of elements and
	    ///         approximate their reported size do not.
	    /// </apiNote>
	    public const int Sized = 0x00000040;

	    /// <summary>
	    ///     Characteristic value signifying that the source guarantees that
	    ///     encountered elements will not be
	    ///     <see langword="null" />
	    ///     . (This applies,
	    ///     for example, to most concurrent collections, queues, and maps.)
	    /// </summary>
	    public const int Nonnull = 0x00000100;

	    /// <summary>
	    ///     Characteristic value signifying that the element source cannot be
	    ///     structurally modified; that is, elements cannot be added, replaced, or
	    ///     removed, so such changes cannot occur during traversal.
	    /// </summary>
	    /// <remarks>
	    ///     Characteristic value signifying that the element source cannot be
	    ///     structurally modified; that is, elements cannot be added, replaced, or
	    ///     removed, so such changes cannot occur during traversal. A Spliterator
	    ///     that does not report
	    ///     <c>IMMUTABLE</c>
	    ///     or
	    ///     <c>CONCURRENT</c>
	    ///     is expected
	    ///     to have a documented policy (for example throwing
	    ///     <see cref="ConcurrentModificationException" />
	    ///     ) concerning structural
	    ///     interference detected during traversal.
	    /// </remarks>
	    public const int Immutable = 0x00000400;

	    /// <summary>
	    ///     Characteristic value signifying that the element source may be safely
	    ///     concurrently modified (allowing additions, replacements, and/or removals)
	    ///     by multiple threads without external synchronization.
	    /// </summary>
	    /// <remarks>
	    ///     Characteristic value signifying that the element source may be safely
	    ///     concurrently modified (allowing additions, replacements, and/or removals)
	    ///     by multiple threads without external synchronization. If so, the
	    ///     Spliterator is expected to have a documented policy concerning the impact
	    ///     of modifications during traversal.
	    ///     <p>
	    ///         A top-level Spliterator should not report both
	    ///         <c>CONCURRENT</c>
	    ///         and
	    ///         <c>SIZED</c>
	    ///         , since the finite size, if known, may change if the source
	    ///         is concurrently modified during traversal. Such a Spliterator is
	    ///         inconsistent and no guarantees can be made about any computation using
	    ///         that Spliterator. Sub-spliterators may report
	    ///         <c>SIZED</c>
	    ///         if the
	    ///         sub-split size is known and additions or removals to the source are not
	    ///         reflected when traversing.
	    /// </remarks>
	    /// <apiNote>
	    ///     Most concurrent collections maintain a consistency policy
	    ///     guaranteeing accuracy with respect to elements present at the point of
	    ///     Spliterator construction, but possibly not reflecting subsequent
	    ///     additions or removals.
	    /// </apiNote>
	    public const int Concurrent = 0x00001000;

	    /// <summary>
	    ///     Characteristic value signifying that all Spliterators resulting from
	    ///     <c>trySplit()</c>
	    ///     will be both
	    ///     <see cref="Spliterator{T}.Sized" />
	    ///     and
	    ///     <see cref="Spliterator{T}.Subsized" />
	    ///     .
	    ///     (This means that all child Spliterators, whether direct or indirect, will
	    ///     be
	    ///     <c>SIZED</c>
	    ///     .)
	    ///     <p>
	    ///         A Spliterator that does not report
	    ///         <c>SIZED</c>
	    ///         as required by
	    ///         <c>SUBSIZED</c>
	    ///         is inconsistent and no guarantees can be made about any
	    ///         computation using that Spliterator.
	    /// </summary>
	    /// <apiNote>
	    ///     Some spliterators, such as the top-level spliterator for an
	    ///     approximately balanced binary tree, will report
	    ///     <c>SIZED</c>
	    ///     but not
	    ///     <c>SUBSIZED</c>
	    ///     , since it is common to know the size of the entire tree
	    ///     but not the exact sizes of subtrees.
	    /// </apiNote>
	    public const int Subsized = 0x00004000;
    }
}