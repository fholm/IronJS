using System;
using System.Collections.Generic;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Jit {
    public class Cache {
        Dictionary<int, Delegate> _cached;

        public Cache() {
            _cached = new Dictionary<int, Delegate>();
        }

        public bool TryGet(Type funcType, Closure closure, out Delegate func) {
            return TryGet(funcType, closure.ContextType, out func);
        }

        public bool TryGet(Type funcType, Type closureType, out Delegate func) {
            return _cached.TryGetValue(CombineHashCode(funcType, closureType), out func);
        }

        public Delegate Save(Type funcType, Type closureType, Delegate func) {
            return _cached[CombineHashCode(funcType, closureType)] = func;
        }

        public Delegate Save(Type funcType, Type closureType, object func) {
            return Save(funcType, closureType, (Delegate)func);
        }

        public static int CombineHashCode(Type funcType, Type closureType) {
            int hash = 17;

            hash = hash * 31 + funcType.GetHashCode();
            hash = hash * 31 + closureType.GetHashCode();

            return hash;
        }

    }
}
