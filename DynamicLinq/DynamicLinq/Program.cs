using DynamicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLinq
{
    public class Patient
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
    }

    public class Doctor
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Program _program = new Program();
            Expression<Func<Patient, bool>> PatientFilter = _program.FilterByNameAndSurname<Patient>();
            Expression<Func<Doctor, bool>> DoctorFilter = _program.FilterByNameAndSurname<Doctor>();

        }
        public Expression<Func<T, bool>> FilterByNameAndSurname<T>() where T : class
        {
            return Expressions.DynamicExpressionCreate<T>(new List<Expressions.Filter>
            {
                        new Expressions.Filter {PropertyName = "Name", Operation = OperationType.Equal, Value = "Emre"},
                        new Expressions.Filter {Operation = OperationType.AndAlso},
                        new Expressions.Filter {PropertyName = "Surname", Operation = OperationType.Equal, Value = "Bayrakdar"},
            });
            // Output --> x => x.Name == "Emre"  && x.Surname == "Bayrakdar"
        }

    }
}
