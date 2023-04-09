using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using Dapper;
using DapperBuilder;

SqlMapper.SetTypeMap(typeof(Student), new CustomPropertyTypeMap(typeof(Student), (type, columnName) =>
    type.GetProperties().FirstOrDefault(prop =>
        prop.GetCustomAttributes(false)
            .OfType<ColumnAttribute>()
            .Any(attr => attr.Name == columnName))));

//var query = "SELECT * FROM Student";
var query = new QueryBuilder<Student>()
    .Select(s => new Student { Id = s.Id, Name = s.Name })
    .Build();

Console.WriteLine(query);

using var connection = new SqlConnection("Server=.;Database=ApuSchool;Trusted_Connection=True;");
var students = await connection.QueryAsync<Student>(query);

foreach (var student in students)
{
    Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}");
}

Console.ReadLine();