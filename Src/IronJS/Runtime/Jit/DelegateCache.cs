using System;
using System.Collections.Generic;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Jit {
    public class DelegateCache {
        Dictionary<Type, Delegate> _cached;

        public DelegateCache() {
            _cached = new Dictionary<Type, Delegate>();
        }

        public bool TryGet(Type funcType, out Delegate func) {
            return _cached.TryGetValue(funcType, out func);
        }

        public Delegate Save(Type funcType, Delegate func) {
            return _cached[funcType] = func;
        }

    }
}
