using DAL.Entities;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            StudentModel context = new StudentModel();
            return context.Students.ToList();
        }

        public List<Student> GetAllHasNoMajor()
        {
            StudentModel context = new StudentModel();
            return context.Students.Where(p => p.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentModel context = new StudentModel();
            return context.Students.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }
        public Student FindById(string studentId)
        {
            StudentModel context = new StudentModel();
            return context.Students.FirstOrDefault(p => p.StudentID == studentId);
        }
        public void InsertUpdate(Student s)
        {
            StudentModel context = new StudentModel();
            context.Students.AddOrUpdate(s);
            context.SaveChanges();
        }
        public void DeleteById(string studentId)
        {
            StudentModel context = new StudentModel();
            Student student =  context.Students.FirstOrDefault(p => p.StudentID == studentId);
            if (student == null)
                throw new System.Exception("Không tìm thấy MSSV cần xóa");
            context.Students.Remove(student);
            context.SaveChanges();
        }
    }
}
