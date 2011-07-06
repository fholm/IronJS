using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    using IndexMap = Dictionary<string, int>;
    using IndexStack = Stack<int>;
    using SchemaMap = Dictionary<string, Schema>;

    public class Schema
    {
        public readonly ulong Id;
        public readonly Environment Env;
        public readonly IndexMap IndexMap;
        public readonly SchemaMap SubSchemas;

        public Schema(Environment env, IndexMap indexMap)
        {
            Id = env.NextPropertyMapId();
            Env = env;
            IndexMap = indexMap;
            SubSchemas = new SchemaMap();
        }

        public Schema(Environment env, IndexMap indexMap, SchemaMap subSchemas)
        {
            Id = 1UL;
            Env = env;
            IndexMap = indexMap;
            SubSchemas = subSchemas;
        }

        public virtual DynamicSchema MakeDynamic()
        {
            return new DynamicSchema(Env, IndexMap);
        }

        public virtual Schema Delete(string name)
        {
            return MakeDynamic().Delete(name);
        }

        public virtual Schema SubClass(string name)
        {
            Schema subSchema;

            if (!SubSchemas.TryGetValue(name, out subSchema))
            {
                var newIndexMap = new IndexMap(IndexMap);
                newIndexMap.Add(name, newIndexMap.Count);
                SubSchemas[name] = subSchema = new Schema(Env, newIndexMap);
            }

            return subSchema;
        }

        public virtual Schema SubClass(IEnumerable<string> names)
        {
            Schema schema = this;

            foreach (var name in names)
            {
                schema = schema.SubClass(name);
            }

            return schema;
        }

        public bool TryGetIndex(string name, out int index)
        {
            return IndexMap.TryGetValue(name, out index);
        }

        public static Schema CreateBaseSchema(Environment env)
        {
            return new Schema(env, new IndexMap());
        }
    }

    public class DynamicSchema : Schema
    {
        public readonly IndexStack FreeIndexes
            = new IndexStack();

        public DynamicSchema(Environment env, IndexMap indexMap)
            : base(env, new IndexMap(indexMap), null)
        {

        }

        public override DynamicSchema MakeDynamic()
        {
            return this;
        }

        public override Schema Delete(string name)
        {
            int index;

            if (IndexMap.TryGetValue(name, out index))
            {
                FreeIndexes.Push(index);
                IndexMap.Remove(name);
            }

            return this;
        }

        public override Schema SubClass(string name)
        {
            var index =
                (FreeIndexes.Count > 0) ? (
                    FreeIndexes.Pop()
                ) : ( 
                    IndexMap.Count
                );

            IndexMap.Add(name, index);
            return this;
        }
    }
}
