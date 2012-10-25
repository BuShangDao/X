﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NewLife.Serialization;

namespace NewLife.Core.Test.Serialization
{
    class DictionaryObj : Obj
    {
        private Dictionary<Int32, SimpleObj> _Objs;
        /// <summary>属性说明</summary>
        public Dictionary<Int32, SimpleObj> Objs { get { return _Objs; } set { _Objs = value; } }

        public DictionaryObj()
        {
            var n = Rnd.Next(10) - 1;
            if (n >= 0)
            {
                Objs = new Dictionary<Int32, SimpleObj>();
                for (int i = 0; i < n; i++)
                {
                    Objs.Add(i, SimpleObj.Create());
                }
            }
        }

        public override void Write(BinaryWriter writer, BinarySettings set)
        {
            if (Objs == null) return;

            var n = Objs.Count;
            if (!set.EncodeInt && set.SizeFormat != TypeCode.UInt16 && set.SizeFormat != TypeCode.UInt32 && set.SizeFormat != TypeCode.UInt64)
                writer.Write(n);
            else
                writer.Write(WriteEncoded(n));

            foreach (var item in Objs)
            {
                if (!set.EncodeInt)
                    writer.Write(item.Key);
                else
                    writer.Write(WriteEncoded(item.Key));

                item.Value.Write(writer, set);
            }
        }

        public override bool CompareTo(Obj obj)
        {
            //return base.CompareTo(obj);
            var arr = obj as DictionaryObj;
            if (arr == null) return false;

            if ((Objs == null || Objs.Count == 0) && (arr.Objs == null || arr.Objs.Count == 0)) return true;

            if (Objs.Count != arr.Objs.Count) return false;

            foreach (var item in Objs)
            {
                SimpleObj sb;
                // 不存在？
                if (!arr.Objs.TryGetValue(item.Key, out sb)) return false;

                // 很小的可能相等，两者可能都是null
                if (sb == item.Value) continue;

                if (!sb.CompareTo(item.Value)) return false;
            }

            return true;
        }
    }
}
