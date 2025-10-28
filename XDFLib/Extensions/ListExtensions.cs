﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace XDFLib.Extensions
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this List<T> list, Span<T> span)
        {
            var newCap = list.Count + span.Length;
            if (list.Capacity < newCap)
            {
                list.Capacity = newCap;
            }
            for (int i = 0; i < span.Length; i++)
            {
                list.Add(span[i]);
            }
        }

        public static void AddRange<T>(this List<T> list, ReadOnlySpan<T> span)
        {
            var newCap = list.Count + span.Length;
            if (list.Capacity < newCap)
            {
                list.Capacity = newCap;
            }
            for (int i = 0; i < span.Length; i++)
            {
                list.Add(span[i]);
            }
        }

        public static void Resize<T>(this List<T> list, int newSize, T element = default(T))
        {
            int count = list.Count;

            if (newSize < count)
            {
                list.RemoveRange(newSize, count - newSize);
            }
            else if (newSize > count)
            {
                if (newSize > list.Capacity)   // Optimization
                    list.Capacity = newSize;

                list.AddRange(Enumerable.Repeat(element, newSize - count));
            }
        }

        public static void Resize<T>(this List<T> list, int newSize, Func<T> createNewMember)
        {
            int count = list.Count;

            if (newSize < count)
            {
                list.RemoveRange(newSize, count - newSize);
            }
            else if (newSize > count)
            {
                if (newSize > list.Capacity)   // Optimization
                    list.Capacity = newSize;

                if (createNewMember != null)
                {
                    int countToCreate = newSize - count;
                    for (int i = 0; i < countToCreate; i++)
                    {
                        list.Add(createNewMember());
                    }
                }
                else
                {
                    list.AddRange(Enumerable.Repeat(default(T), newSize - count));
                }
            }
        }

        public static void MoveItem<T>(this List<T> list, int originIndex, int move)
        {
            if (originIndex < 0 || originIndex >= list.Count || move == 0)
            {
                return;
            }
            var validMove = XMath.Clamp(move, -originIndex, list.Count - 1 - originIndex);

            var destIndex = originIndex + validMove;
            if (destIndex >= 0 && destIndex < list.Count)
            {
                var item = list[originIndex];
                list.RemoveAt(originIndex);
                list.Insert(destIndex, item);
            }
        }

        //public static T Pop<T>(this List<T> list)
        //{
        //    if (list.Count > 0)
        //    {
        //        var last = list.Count - 1;
        //        T v = list[last];
        //        list.RemoveAt(last);
        //        return v;
        //    }
        //    else
        //        return default(T);
        //}

        //public static T GetRandomElement<T>(this List<T> list)
        //{
        //    if (list.Count > 0)
        //    {
        //        var currIndex = XMath.RandomRange(0, list.Count);
        //        return list[currIndex];
        //    }
        //    else
        //    {
        //        return default(T);
        //    }
        //}

        //public static void RemoveRandomElement<T>(this List<T> list)
        //{
        //    if (list.Count > 0)
        //    {
        //        var currIndex = XMath.RandomRange(0, list.Count);
        //        list.RemoveAt(currIndex);
        //    }
        //}

        //public static bool IsIndexValid<T>(this List<T> list, int index)
        //{
        //    return index >= 0 && index < list.Count;
        //}

        //public static T GetByLoopedIndex<T>(this List<T> list, int index)
        //{
        //    if (list.Count > 0)
        //    {
        //        var loopedIndex = XMath.Loop(index, 0, list.Count);
        //        return list[loopedIndex];
        //    }
        //    else
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //}

    }
}
