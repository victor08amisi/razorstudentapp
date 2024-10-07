using AcademicManagement;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
namespace Lab2.Pages;


public class Registration : PageModel
{
    [BindProperty]
    public List<string> courseSelected { get; set; } = new List<string>();
    public bool isStudentRegistered { get; set; }
    public bool ShowDiv { get; set; } = true;
    public bool ShowDiv2 { get; set; }
    [BindProperty] 
    public string? studentID { get; set; }
    [BindProperty]
    public string? alertMessage { get; set; }
    [BindProperty]
    public string? studentGrade { get; set; }
    public List<AcademicRecord> studentRecord { get; set; }
    [BindProperty]
    public List<string>  studentGrades { get; set; }

    public void OnGet()
    {
        isStudentRegistered = true;
    }
    
    public void OnPostRegister()
    {
        
        //check if a student exist based on user input
        var userList = DataAccess.GetAllStudents()
            .Where(student => student.StudentId == studentID)
            .ToList();
        if (userList.Count == 0)
        {
            
            alertMessage = "You must select a student!";
            ShowDiv2 = false;
            isStudentRegistered = true;
        }
        else
        {
            var resultList = new List<AcademicManagement.AcademicRecord>();
            resultList = DataAccess.GetAcademicRecordsByStudentId(studentID);
            if (resultList.Count == 0)
            {
                alertMessage = "The student has not registered to any courses. Select the course(s) to register";
                isStudentRegistered = false;
                ShowDiv2 = false;
                
            }
            else
            {
                alertMessage = "The student has registered for the following courses:";
                ShowDiv = false;
                ShowDiv2 = true;
            }
        }
    }

    public void OnPostStudentSelected()
    {
       
        
        if (courseSelected.Count == 0)
        {
            alertMessage = "You must select at least one course!";
            ShowDiv2 = false;

        }
        else
        {
            alertMessage = "";
            foreach (var courseCode in courseSelected)
            {
                var courseToRegister = new AcademicRecord(studentID, courseCode);
                DataAccess.AddAcademicRecord(courseToRegister);
            }
            alertMessage = "The student has registered for the following courses:";
            ShowDiv = false;
            ShowDiv2 = true;
            

        }
        
    }

    public void OnPostGradeSubmission()
    {
        // Retrieve all form values
        var formData = Request.Form;
        foreach (var item in DataAccess.GetAcademicRecordsByStudentId(studentID))
        {
            if (formData.ContainsKey(item.CourseCode))
            {
                // Try to parse the value as a double
                if (double.TryParse(formData[item.CourseCode], out double grade))
                {
                    item.Grade = grade; // Assign the parsed grade to item.Grade
                    Console.WriteLine(item.Grade);
                }
            }
            
        }
        
    

    }

    public IActionResult OnPostCourseSort()
    {
        var studentCoursesSorted = DataAccess.GetAcademicRecordsByStudentId(studentID)
            .OrderBy(record => record.CourseCode)
            .ToList();
        HttpContext.Session.SetString($"{studentID}", JsonSerializer.Serialize(studentCoursesSorted));
        return RedirectToPage();
        
    }

    public IActionResult OnPostTitleSort()
    {
        var studentRecords = DataAccess.GetAcademicRecordsByStudentId(studentID);
        
        var sortedRecords = studentRecords.OrderBy(record => 
                DataAccess.GetAllCourses()
                    .FirstOrDefault(c => c.CourseCode == record.CourseCode)?.CourseTitle)
            .ToList();
        
        HttpContext.Session.SetString($"{studentID}", JsonSerializer.Serialize(sortedRecords));
        return RedirectToPage();


    }

    public IActionResult OnPostGradeSort()
    {
        var studentRecords = DataAccess.GetAcademicRecordsByStudentId(studentID);
        
        var sortedRecords = studentRecords.OrderBy(record => record.Grade).ToList();
        
        HttpContext.Session.SetString($"{studentID}", JsonSerializer.Serialize(sortedRecords));
        return RedirectToPage();
    }
    
    
}