using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Geometry.Shapes;

namespace InternalSection {
	public class OverloadBinder : Binder
    {
        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args,
            ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] names, out object state)
        {
            if (match == null)
                throw new ArgumentNullException("Match is null.");
            state = null;
			List<int> indices = null;
            foreach (MethodBase mb in match)
            {
                ParameterInfo[] parameters = mb.GetParameters();


				indices = ParametersMatch(parameters, args);
                if (indices != null)
                {
					for (int i = 0; i < args.Length - 1; i++) {
						if (i != indices[i]) {
							object o = args[i];
							args[i] = args[indices[i]];
							args[indices[i]] = o;
						}
					}
                    return mb;
                }
            }
            return null;
        }
        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        private List<int> ParametersMatch(ParameterInfo[] parameters, object[] args)
        {
			bool re = false;
			int count = 0;
			bool exchange = false;
			List<int> indeces = new List<int>();
			if (parameters.Length == args.Length) {
				int i = 0, j = 0;
	            for (i = 0; i < parameters.Length; i++) {
					while (j < parameters.Length) {
						if (parameters[i].ParameterType == args[j].GetType()) {
							count++;
							if (i != j)
								exchange = true;
							if (j < parameters.Length) 
								j++;
							else j = 0;
							break;
						}
						if (j < parameters.Length)
							j++;
					}
					if (exchange || count == 0)
						j = 0;
	            }
				if (count == 1) {
					if (parameters[0].ParameterType == typeof(IShape) || parameters[1].ParameterType == typeof(IShape)) {
						re = true;
					}
				}
				if ((count == 2 && exchange) || (re && exchange)) { // count == 1 már fenn teljesül
					indeces.Add(1); // mert ha re = true akkor count is = 1
					indeces.Add(0);
				}
				else if ((!exchange && count == 2) || (re && !exchange))  {
					indeces.Add(0);
					indeces.Add(1);
				}
				if (indeces.Count != args.Length)
					return null;
			}
			else
				return null; //("Ez a Binder MÉG! nincs felkészítve több paraméterszámú túlterhelésre.");
            return indeces;
        }
        public override void ReorderArgumentArray(ref object[] args, object state)
        {
            
        }
        public override object ChangeType(object value, Type type, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

    }
}

