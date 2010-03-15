/**
 * GENERATED CODE
 **/
using System;
using IronJS.Ast.Nodes;
 
namespace IronJS.Runtime.Js {
	public partial class ClosureCtx {
        public static Type GetType(Lambda func, Lambda target) {
			int count = target.Scope.Enclosed.Count;
			
            if (count == 0) {
                return typeof(ClosureCtx);
            } else if(count == 1) {
				return typeof(ClosureCtx<>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type  
				);
			} else if(count == 2) {
				return typeof(ClosureCtx<,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type  
				);
			} else if(count == 3) {
				return typeof(ClosureCtx<,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type  
				);
			} else if(count == 4) {
				return typeof(ClosureCtx<,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type  
				);
			} else if(count == 5) {
				return typeof(ClosureCtx<,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type  
				);
			} else if(count == 6) {
				return typeof(ClosureCtx<,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type  
				);
			} else if(count == 7) {
				return typeof(ClosureCtx<,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type  
				);
			} else if(count == 8) {
				return typeof(ClosureCtx<,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type  
				);
			} else if(count == 9) {
				return typeof(ClosureCtx<,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type  
				);
			} else if(count == 10) {
				return typeof(ClosureCtx<,,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[9].Name).Type  
				);
			} else if(count == 11) {
				return typeof(ClosureCtx<,,,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[9].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[10].Name).Type  
				);
			} else if(count == 12) {
				return typeof(ClosureCtx<,,,,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[9].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[10].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[11].Name).Type  
				);
			} else if(count == 13) {
				return typeof(ClosureCtx<,,,,,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[9].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[10].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[11].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[12].Name).Type  
				);
			} else if(count == 14) {
				return typeof(ClosureCtx<,,,,,,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[9].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[10].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[11].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[12].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[13].Name).Type  
				);
			} else if(count == 15) {
				return typeof(ClosureCtx<,,,,,,,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[9].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[10].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[11].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[12].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[13].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[14].Name).Type  
				);
			} else if(count == 16) {
				return typeof(ClosureCtx<,,,,,,,,,,,,,,,>).MakeGenericType( 
					func.Scope.Get(target.Scope.Enclosed[0].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[1].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[2].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[3].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[4].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[5].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[6].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[7].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[8].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[9].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[10].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[11].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[12].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[13].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[14].Name).Type,  
					func.Scope.Get(target.Scope.Enclosed[15].Name).Type  
				);
			} else {
                return typeof(ClosureCtxN);
            }
		}
	}
 
	public class ClosureCtx<T0> : ClosureCtx {
		public T0 Item0;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
		public T9 Item9;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
		public T9 Item9;
		public T10 Item10;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
		public T9 Item9;
		public T10 Item10;
		public T11 Item11;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
		public T9 Item9;
		public T10 Item10;
		public T11 Item11;
		public T12 Item12;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
		public T9 Item9;
		public T10 Item10;
		public T11 Item11;
		public T12 Item12;
		public T13 Item13;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
		public T9 Item9;
		public T10 Item10;
		public T11 Item11;
		public T12 Item12;
		public T13 Item13;
		public T14 Item14;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
 
	public class ClosureCtx<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> : ClosureCtx {
		public T0 Item0;
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;
		public T5 Item5;
		public T6 Item6;
		public T7 Item7;
		public T8 Item8;
		public T9 Item9;
		public T10 Item10;
		public T11 Item11;
		public T12 Item12;
		public T13 Item13;
		public T14 Item14;
		public T15 Item15;
	
		public ClosureCtx(RuntimeCtx runtime, Obj globals)
				: base(runtime, globals) { }
	}
}