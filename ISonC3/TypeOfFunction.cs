using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISonC3
{
    class TypeOfFunction
    {
        public TypeOfFunction(
            string name, 
            Func<List<XY>, string> funcstring,
            Func<double, double, double,
            double> function)
        {
            Name = name;
            GetFuncString = funcstring;
            Func = function;
        }
        public Func<List<XY>, string> GetFuncString;
        public Func<double, double, double,
            double> Func;
        public double Value { get; set; }
        public string Name { get; set; }
    }
}
