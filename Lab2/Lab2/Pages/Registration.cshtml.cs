using AcademicManagement;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
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
    
}